using Dawn;

namespace Reactive.Expressions.Ast.Visitors;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Reactive.Expressions;
using Reactive.Expressions.Ast;
using Reactive.Expressions.Models;

/// <summary>
/// Expression validator that uses the Visitor pattern to traverse and validate AST nodes.
/// Checks for semantic correctness including known metrics, valid operators, and proper syntax.
/// </summary>
/// <remarks>
/// The validator performs:
/// - Metric name validation against known metrics
/// - Variable name validation against known variables
/// - Operator validity checking
/// - Aggregation function validation
/// - Threshold expression validation
/// - Time window format validation
///
/// Validation results include both errors (which make expressions invalid) and warnings
/// (which indicate potential issues but don't prevent execution).
/// </remarks>
public class ExpressionValidator : IExpressionVisitor<ValidationResult>
{
    /// <summary>
    /// Set of metric names that are considered valid for validation.
    /// </summary>
    private readonly HashSet<string> _knownMetrics;

    /// <summary>
    /// Set of variable names that are considered valid for validation.
    /// </summary>
    private readonly HashSet<string> _knownVariables;

    /// <summary>
    /// Set of valid aggregation function names.
    /// </summary>
    private readonly HashSet<string> _validAggregations = new HashSet<string> { "avg", "sum", "max", "min" };

    /// <summary>
    /// Set of valid comparison operators.
    /// </summary>
    private readonly HashSet<string> _validOperators = new HashSet<string> { ">", ">=", "<", "<=", "==", "!=" };

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionValidator"/> class.
    /// </summary>
    /// <param name="knownMetrics">Set of valid metric names (case-insensitive).</param>
    /// <param name="knownVariables">Set of valid variable names (case-insensitive).</param>
    public ExpressionValidator(ISet<string>? knownMetrics = null, ISet<string>? knownVariables = null)
    {
        _knownMetrics = new HashSet<string>(knownMetrics ?? new HashSet<string>(), StringComparer.OrdinalIgnoreCase);
        _knownVariables = new HashSet<string>(knownVariables ?? new HashSet<string>(), StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Validates an expression tree starting from the root node.
    /// This is the main entry point for expression validation.
    /// </summary>
    /// <param name="expression">Root node of the expression to validate.</param>
    /// <returns>ValidationResult containing errors, warnings, and validity status.</returns>
    public ValidationResult ValidateExpression(ExpressionNode expression)
    {
        Guard.Argument(expression, nameof(expression)).NotNull();

        return expression.Accept(this);
    }

    /// <summary>
    /// Validates a binary operator node (AND/OR operations).
    /// Recursively validates both operands and combines the results.
    /// </summary>
    /// <param name="node">Binary operator node to validate.</param>
    /// <returns>Combined validation result from both operands.</returns>
    public ValidationResult VisitBinaryOperator(BinaryOperatorNode node)
    {
        Guard.Argument(node, nameof(node)).NotNull();

        var result = new ValidationResult { IsValid = true };

        // Validate left and right operands
        var leftResult = node.Left.Accept(this);
        var rightResult = node.Right.Accept(this);

        // Combine results
        result.IsValid = leftResult.IsValid && rightResult.IsValid;
        result.Errors.AddRange(leftResult.Errors);
        result.Errors.AddRange(rightResult.Errors);
        result.Warnings.AddRange(leftResult.Warnings);
        result.Warnings.AddRange(rightResult.Warnings);

        // Validate operator
        if (node.Operator != BinaryOperator.And && node.Operator != BinaryOperator.Or)
        {
            result.AddError($"Unknown binary operator: {node.Operator}");
        }

        return result;
    }

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Ignore exceptions during validation")]
    public ValidationResult VisitCondition(ConditionNode node)
    {
        Guard.Argument(node, nameof(node)).NotNull();

        var result = new ValidationResult { IsValid = true };
        var condition = node.Condition!;

        // Validate metric name
        if (string.IsNullOrWhiteSpace(condition.MetricName))
        {
            result.AddError("Metric name cannot be null or empty");
        }
        else if (_knownMetrics.Any() && !_knownMetrics.Contains(condition.MetricName))
        {
            result.AddError($"Unknown metric: {condition.MetricName}");
        }

        // Validate operator
        if (!_validOperators.Contains(condition.Operator))
        {
            result.AddError($"Invalid operator: {condition.Operator}");
        }

        // Validate aggregation-specific fields
        if (condition.IsAggregation)
        {
            if (string.IsNullOrWhiteSpace(condition.AggregationType))
            {
                result.AddError("Aggregation type cannot be null or empty for aggregation conditions");
            }
            else if (!_validAggregations.Contains(condition.AggregationType, StringComparer.OrdinalIgnoreCase))
            {
                result.AddError($"Invalid aggregation type: {condition.AggregationType}");
            }

            if (condition.TimeWindow <= TimeSpan.Zero)
            {
                result.AddError("Time window must be greater than zero for aggregation conditions");
            }
            else if (condition.TimeWindow > TimeSpan.FromHours(24))
            {
                result.AddWarning("Time window greater than 24 hours may impact performance");
            }
        }
        else
        {
            // For non-aggregation conditions, these should be null/default
            if (!string.IsNullOrEmpty(condition.AggregationType))
            {
                result.AddWarning("Aggregation type is set for non-aggregation condition");
            }

            if (condition.TimeWindow != TimeSpan.Zero)
            {
                result.AddWarning("Time window is set for non-aggregation condition");
            }
        }

        // Validate threshold or threshold expression
        if (condition.HasVariableThreshold)
        {
            // Validate threshold expression and its variables
            var variables = condition.ThresholdExpression!.GetVariables();
            foreach (var variable in variables)
            {
                if (_knownVariables.Any() && !_knownVariables.Contains(variable))
                {
                    result.AddError($"Unknown variable: {variable}");
                }
            }

            // Try to evaluate the expression with default values to check for errors
            try
            {
                var testResolver = new VariableResolver();
                foreach (var variable in variables)
                {
                    testResolver.SetVariable(variable, 1.0); // Use test value
                }

                condition.ThresholdExpression.Evaluate(testResolver);
            }
            catch (Exception ex)
            {
                result.AddError($"Error in threshold expression: {ex.Message}");
            }
        }
        else
        {
            // Validate simple threshold
            if (double.IsNaN(condition.Threshold) || double.IsInfinity(condition.Threshold))
            {
                result.AddError("Threshold cannot be NaN or Infinity");
            }
        }

        return result;
    }
}
