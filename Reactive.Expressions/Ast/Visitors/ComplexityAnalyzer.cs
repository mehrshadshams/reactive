using Dawn;

namespace Reactive.Expressions.Ast.Visitors;

using System;
using Reactive.Expressions.Ast;
using Reactive.Expressions.Models;

/// <summary>
/// Analyzer that calculates various complexity metrics for expression trees.
/// Provides insights into expression structure that can be used for performance
/// optimization, cost estimation, and complexity management.
/// </summary>
/// <remarks>
/// The complexity analyzer computes:
/// - Total node count (overall expression size)
/// - Condition count (number of leaf conditions)
/// - Aggregation count (number of time-windowed aggregations)
/// - Maximum depth (nesting level)
/// - Operator count (logical AND/OR operations)
///
/// These metrics help with:
/// - Performance optimization decisions
/// - Resource allocation planning
/// - Expression complexity limits enforcement
/// - Cost-based query optimization
/// - Debugging and monitoring.
/// </remarks>
public class ComplexityAnalyzer : IExpressionVisitor<ExpressionComplexity>
{
    /// <summary>
    /// Analyzes the complexity of an expression tree starting from the root node.
    /// This is the main entry point for complexity analysis.
    /// </summary>
    /// <param name="expression">Root expression node to analyze.</param>
    /// <returns>ExpressionComplexity object containing various complexity metrics.</returns>
    public ExpressionComplexity AnalyzeComplexity(ExpressionNode expression)
    {
        Guard.Argument(expression, nameof(expression)).NotNull();

        return expression.Accept(this);
    }

    /// <summary>
    /// Visits a binary operator node and combines complexity metrics from both operands.
    /// Increments node count, operator count, and depth while combining child metrics.
    /// </summary>
    /// <param name="node">Binary operator node (AND/OR).</param>
    /// <returns>Combined complexity metrics including this operator.</returns>
    public ExpressionComplexity VisitBinaryOperator(BinaryOperatorNode node)
    {
        Guard.Argument(node, nameof(node)).NotNull();

        var leftComplexity = node.Left.Accept(this);
        var rightComplexity = node.Right.Accept(this);

        return new ExpressionComplexity
        {
            NodeCount = leftComplexity.NodeCount + rightComplexity.NodeCount + 1,
            ConditionCount = leftComplexity.ConditionCount + rightComplexity.ConditionCount,
            AggregationCount = leftComplexity.AggregationCount + rightComplexity.AggregationCount,
            MaxDepth = Math.Max(leftComplexity.MaxDepth, rightComplexity.MaxDepth) + 1,
            OperatorCount = leftComplexity.OperatorCount + rightComplexity.OperatorCount + 1,
        };
    }

    /// <summary>
    /// Visits a condition node and returns its complexity metrics.
    /// Leaf nodes contribute 1 to node count and condition count, plus 1 to aggregation
    /// count if the condition involves an aggregation function.
    /// </summary>
    /// <param name="node">Condition node to analyze.</param>
    /// <returns>Complexity metrics for this leaf condition.</returns>
    public ExpressionComplexity VisitCondition(ConditionNode node)
    {
        Guard.Argument(node, nameof(node)).NotNull();

        return new ExpressionComplexity
        {
            NodeCount = 1,
            ConditionCount = 1,
            AggregationCount = node.Condition.IsAggregation ? 1 : 0,
            MaxDepth = 1,
            OperatorCount = 0,
        };
    }
}
