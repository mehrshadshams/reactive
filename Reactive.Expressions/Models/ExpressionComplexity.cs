namespace Reactive.Expressions.Models;

/// <summary>
/// Represents complexity metrics for an expression tree, providing insights into expression structure
/// and computational requirements for performance optimization and resource planning.
/// </summary>
/// <remarks>
/// ExpressionComplexity tracks:
/// - NodeCount: Total number of nodes in the expression tree
/// - ConditionCount: Number of leaf condition nodes (actual metric comparisons)
/// - AggregationCount: Number of time-windowed aggregation operations
/// - MaxDepth: Maximum nesting depth of the expression tree
/// - OperatorCount: Number of logical operators (AND/OR)
///
/// These metrics help with:
/// - Performance optimization decisions
/// - Resource allocation planning
/// - Expression complexity limits enforcement
/// - Cost-based query optimization
/// - Debugging and monitoring expression evaluation performance
///
/// The IsHighComplexity property provides a quick check for expressions that may
/// require special handling due to computational or memory requirements.
/// </remarks>
public class ExpressionComplexity
{
    /// <summary>
    /// Gets or sets the total number of nodes in the expression tree.
    /// Includes all nodes: conditions, operators, constants, and variables.
    /// </summary>
    public int NodeCount { get; set; }

    /// <summary>
    /// Gets or sets the number of leaf condition nodes that perform actual metric comparisons.
    /// Each condition represents a comparison between a metric and a threshold value.
    /// </summary>
    public int ConditionCount { get; set; }

    /// <summary>
    /// Gets or sets the number of time-windowed aggregation operations in the expression.
    /// Aggregations include operations like AVG, SUM, MAX, MIN over time windows.
    /// </summary>
    public int AggregationCount { get; set; }

    /// <summary>
    /// Gets or sets the maximum nesting depth of the expression tree.
    /// Represents the longest path from root to any leaf node.
    /// </summary>
    public int MaxDepth { get; set; }

    /// <summary>
    /// Gets or sets the number of logical operators (AND, OR) in the expression.
    /// Does not include arithmetic operators, only boolean logic operators.
    /// </summary>
    public int OperatorCount { get; set; }

    /// <summary>
    /// Gets a value indicating whether this expression is considered high complexity.
    /// Returns true if NodeCount > 20, MaxDepth > 10, or AggregationCount > 5.
    /// High complexity expressions may require special handling or optimization.
    /// </summary>
    public bool IsHighComplexity => NodeCount > 20 || MaxDepth > 10 || AggregationCount > 5;
}
