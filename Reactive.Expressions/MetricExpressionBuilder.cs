using Dawn;
using Reactive.Ext;

namespace Reactive.Expressions;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Reactive.Expressions.Ast;
using Reactive.Expressions.Models;
using Reactive.Expressions.Parser;
using Microsoft.Extensions.Logging;

/// <summary>
/// Builds and manages reactive streams for metric-based expression evaluation.
/// Orchestrates metric data flow and expression evaluation in real-time streaming scenarios.
/// </summary>
/// <remarks>
/// MetricExpressionBuilder provides:
/// - Reactive stream management for metric data processing
/// - Expression evaluation against streaming metric data
/// - Time-windowed aggregation support for complex conditions
/// - Dynamic metric routing and filtering
/// - Variable resolution for parameterized expressions
///
/// The builder supports:
/// - Real-time metric stream processing
/// - Time-based aggregations (avg, sum, max, min, count over time windows)
/// - Expression evaluation triggering on metric updates
/// - Dynamic expression compilation and execution
/// - Multi-metric expression coordination
///
/// Typical workflow:
/// 1. Create builder with source metric stream and configuration
/// 2. Build expression trees using parser
/// 3. Evaluate expressions against streaming data
/// 4. Subscribe to evaluation results for real-time monitoring.
/// </remarks>
public class MetricExpressionBuilder
{
    private readonly IObservable<MetricData> _sourceStream;
    private readonly HashSet<string> _knownMetrics;
    private readonly ConcurrentDictionary<string, ISubject<MetricData>> _metricStreams;
    private readonly IVariableResolver? _variableResolver;
    private readonly MetricOptions _metricOptions;
    private readonly IExpressionParser _parser;
    private readonly ILogger<MetricExpressionBuilder> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricExpressionBuilder"/> class.
    /// </summary>
    /// <param name="sourceStream">The source stream of metric data to process.</param>
    /// <param name="parser">The expression parser for converting string expressions to AST.</param>
    /// <param name="logger">Logger for tracking builder operations and diagnostics.</param>
    /// <param name="knownMetrics">Optional set of known metric names for validation. If null, all metrics are accepted.</param>
    /// <param name="variableResolver">Optional resolver for dynamic variable substitution in expressions.</param>
    /// <param name="metricOptions">Optional configuration options for metric processing. Uses defaults if null.</param>
    public MetricExpressionBuilder(
        IObservable<MetricData> sourceStream,
        IExpressionParser parser,
        ILogger<MetricExpressionBuilder> logger,
        HashSet<string>? knownMetrics = null,
        IVariableResolver? variableResolver = null,
        MetricOptions? metricOptions = null)
    {
        _sourceStream = Guard.Argument(sourceStream, nameof(sourceStream)).NotNull().Value;
        _parser = Guard.Argument(parser, nameof(parser)).NotNull().Value;
        _logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;

        _knownMetrics = new HashSet<string>(knownMetrics ?? new HashSet<string>(), StringComparer.OrdinalIgnoreCase);
        _metricStreams = new ConcurrentDictionary<string, ISubject<MetricData>>();
        _variableResolver = variableResolver;
        _metricOptions = metricOptions ?? new MetricOptions();
    }

    /// <summary>
    /// Builds a reactive observable stream for evaluating the specified expression against metric data.
    /// Validates the expression syntax and semantics before creating the evaluation stream.
    /// </summary>
    /// <param name="expression">The expression string to parse and evaluate (e.g., "cpu &gt. 0.8 AND memory &lt; 0.9").</param>
    /// <returns>An observable stream of evaluation results that emits when conditions change.</returns>
    /// <exception cref="ArgumentException">Thrown when the expression has validation errors that prevent execution.</exception>
    public IObservable<EvaluationResult> BuildExpression(string expression)
    {
        // Validate the expression first
        var validation = _parser.ValidateExpression(expression, knownMetrics: _knownMetrics, knownVariables: _variableResolver?.Variables);
        if (!validation.IsValid)
        {
            throw new ArgumentException($"Invalid expression: {string.Join(", ", validation.Errors)}");
        }

        if (validation.Warnings.Any())
        {
            _logger.LogWarning(null, "Expression validation warnings: {warnings}", string.Join(", ", validation.Warnings));
        }

        // Parse the expression into an AST using the configured parser
        var ast = _parser.ParseExpression(expression);

        // Evaluate the AST
        return ast.Evaluate(this);
    }

    /// <summary>
    /// Builds a reactive observable for time-windowed aggregation conditions.
    /// Creates sliding window aggregations (avg, sum, max, min) over specified time periods.
    /// </summary>
    /// <param name="node">The condition node containing aggregation configuration and metric details.</param>
    /// <returns>An observable stream that emits evaluation results when aggregation windows complete.</returns>
    public IObservable<EvaluationResult> BuildSlidingWindowAggregation(ConditionNode node)
    {
        Guard.Argument(node, nameof(node)).NotNull();
        var condition = node.Condition;
        var metricStream = GetMetricStream(condition.MetricName);

        return metricStream
            .Buffer(TimeSpan.FromSeconds(1)) // Sliding window every second
            .Where(buffer => buffer.Any())
            .SelectMany(buffer => buffer.OrderBy(m => m.Timestamp).ToList())
            .WindowByTimestamp(metric => metric.Timestamp.Truncate(condition.TimeWindow).Ticks, condition.TimeWindow)
            .SelectMany(buffer =>
            {
                return buffer.ToList();
            }) // Flatten the buffered windows
            .Where(buffer => buffer.Any())
            .Select(buffer => buffer.OrderBy(m => m.Timestamp).ToList())
            .Select(buffer =>
            {
                var windowStart = buffer.First().Timestamp.Truncate(condition.TimeWindow);
                var windowEnd = windowStart + condition.TimeWindow;

                // Calculate the aggregated value based on the specified aggregation type
                (double aggregatedValue, AggregationType aggregationType) = condition.AggregationType?.ToLower(CultureInfo.InvariantCulture) switch
                {
                    "avg" => (buffer.Average(m => m.Value), AggregationType.Average),
                    "sum" => (buffer.Sum(m => m.Value), AggregationType.Sum),
                    "max" => (buffer.Max(m => m.Value), AggregationType.Max),
                    "min" => (buffer.Min(m => m.Value), AggregationType.Min),
                    _ => throw new NotSupportedException($"Aggregation type '{condition.AggregationType}' not supported"),
                };
                return new AggregationResult(node.Name, aggregationType, new Period(windowStart.DateTime, windowEnd.DateTime), aggregatedValue);
            })
            .Select(aggregatedValue =>
            {
                bool result = EvaluateCondition(aggregatedValue.Value, condition);

                return new EvaluationResult(aggregatedValue.NodeName, result, aggregatedValue.Period);
            });
    }

    /// <summary>
    /// Builds a reactive observable for simple condition evaluation without time-based aggregation.
    /// Evaluates conditions immediately against each incoming metric value.
    /// </summary>
    /// <param name="node">The condition node containing the metric comparison logic.</param>
    /// <returns>An observable stream that emits evaluation results for each metric update.</returns>
    public IObservable<EvaluationResult> BuildSimpleConditionObservable(ConditionNode node)
    {
        Guard.Argument(node, nameof(node)).NotNull();
        var condition = node.Condition;
        var metricStream = GetMetricStream(condition.MetricName);

        return metricStream
            .Select(metric =>
            {
                bool result = EvaluateCondition(metric.Value, condition);

                return new EvaluationResult(condition.MetricName, result, Period.SinglePoint(metric.Timestamp.DateTime));
            });
    }

    private IObservable<MetricData> GetMetricStream(string metricName)
    {
        if (!_metricStreams.ContainsKey(metricName))
        {
            _metricStreams[metricName] = new Subject<MetricData>();

            // Subscribe to source and filter by metric name
            _sourceStream
                .Where(m => m.Name == metricName)
                .Subscribe(_metricStreams[metricName]);
        }

        return _metricStreams[metricName].AsObservable();
    }

    private bool EvaluateCondition(double value, ConditionInfo condition)
    {
        double threshold;

        if (condition.HasVariableThreshold)
        {
            try
            {
                threshold = condition.ThresholdExpression!.Evaluate(_variableResolver);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to evaluate threshold expression for condition '{Condition}', value='{Value}'", condition.RawExpression, value);
                throw;
            }
        }
        else
        {
            threshold = condition.Threshold;
        }

        return condition.Operator switch
        {
            ">" => value > threshold,
            ">=" => value >= threshold,
            "<" => value < threshold,
            "<=" => value <= threshold,
            "==" => Math.Abs(value - threshold) < 0.0001,
            "!=" => Math.Abs(value - threshold) >= 0.0001,
            _ => throw new ArgumentException($"Unknown operator: {condition.Operator}"),
        };
    }
}
