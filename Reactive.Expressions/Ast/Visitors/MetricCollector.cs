using Dawn;

namespace Reactive.Expressions.Ast.Visitors;

using System.Collections.Generic;
using Reactive.Expressions.Ast;

/// <summary>
/// Visitor implementation that traverses an expression tree to collect all metric names.
/// This is useful for dependency analysis, validation, and understanding what metrics
/// an expression requires for evaluation.
/// </summary>
/// <remarks>
/// The collector uses the Visitor pattern to traverse the AST and accumulate
/// metric names from all condition nodes. This enables:
/// - Dependency tracking for expressions
/// - Validation against available metrics
/// - Resource planning for metric collection
/// - Performance optimization through selective metric streaming.
/// </remarks>
public class MetricCollector : IExpressionVisitor<HashSet<string>>
{
    /// <summary>
    /// Collects all metric names referenced in an expression tree.
    /// This is the main entry point for metric collection.
    /// </summary>
    /// <param name="expression">Root expression node to analyze.</param>
    /// <returns>HashSet containing all unique metric names found.</returns>
    public HashSet<string> CollectMetrics(ExpressionNode expression)
    {
        Guard.Argument(expression, nameof(expression)).NotNull();

        return expression.Accept(this);
    }

    /// <summary>
    /// Visits a binary operator node and combines metrics from both operands.
    /// </summary>
    /// <param name="node">Binary operator node (AND/OR).</param>
    /// <returns>Union of metrics from left and right operands.</returns>
    public HashSet<string> VisitBinaryOperator(BinaryOperatorNode node)
    {
        Guard.Argument(node, nameof(node)).NotNull();

        var leftMetrics = node.Left.Accept(this);
        var rightMetrics = node.Right.Accept(this);

        leftMetrics.UnionWith(rightMetrics);
        return leftMetrics;
    }

    /// <summary>
    /// Visits a condition node and extracts the metric name.
    /// </summary>
    /// <param name="node">Condition node containing metric reference.</param>
    /// <returns>HashSet containing the metric name from this condition.</returns>
    public HashSet<string> VisitCondition(ConditionNode node)
    {
        Guard.Argument(node, nameof(node)).NotNull();
        return new HashSet<string> { node.Condition.MetricName };
    }
}
