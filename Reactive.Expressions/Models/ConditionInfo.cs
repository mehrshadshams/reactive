namespace Reactive.Expressions.Models;

using System;
using Reactive.Expressions.Ast;

/// <summary>
/// Contains detailed information about a parsed condition from an expression.
/// This class represents both simple conditions (metric > threshold) and
/// complex aggregation conditions (avg(metric, timeWindow) > threshold).
/// </summary>
/// <remarks>
/// ConditionInfo supports:
/// - Simple metric conditions: "cpu > 0.8"
/// - Aggregation conditions: "avg(cpu, 5m) > 0.8"
/// - Variable threshold expressions: "memory > threshold * multiplier"
/// - Different comparison operators: >, >=. <, <=, ==, !=, ~
/// - Time window specifications for aggregations
/// </remarks>
public class ConditionInfo
{
    /// <summary>
    /// Gets or sets the name of the metric being evaluated (e.g., "cpu", "memory").
    /// </summary>
    public required string MetricName { get; set; }

    /// <summary>
    /// Gets or sets the comparison operator used in the condition (>, >=. <, <=, ==, !=, ~)
    /// </summary>
    public required string Operator { get; set; }

    /// <summary>
    /// Gets or sets the numeric threshold value when using constant thresholds.
    /// Used only when ThresholdExpression is null.
    /// </summary>
    public double Threshold { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this condition involves an aggregation function.
    /// </summary>
    public bool IsAggregation { get; set; }

    /// <summary>
    /// Gets or sets the type of aggregation if IsAggregation is true (avg, sum, max, min, count).
    /// </summary>
    public string? AggregationType { get; set; }

    /// <summary>
    /// Gets or sets the time window for aggregation functions (e.g., 5 minutes for "avg(cpu, 5m)").
    /// </summary>
    public TimeSpan TimeWindow { get; set; }

    /// <summary>
    /// Gets or sets optional arithmetic expression for dynamic threshold calculation.
    /// When present, this takes precedence over the Threshold property.
    /// </summary>
    public ArithmeticExpression? ThresholdExpression { get; set; }

    /// <summary>
    /// Gets a value indicating whether this condition uses a variable/expression-based threshold
    /// instead of a constant threshold value.
    /// </summary>
    public bool HasVariableThreshold => ThresholdExpression != null;

    /// <summary>
    /// Gets or sets the original expression text that generated this condition (for debugging/logging).
    /// </summary>
    public string? RawExpression { get; set; }
}
