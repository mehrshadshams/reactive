namespace Reactive.Expressions.Ast.Visitors;

/// <summary>
/// Visitor interface for traversing and processing expression trees using the Visitor pattern.
/// Enables operations like validation, complexity analysis, metric collection, and variable extraction.
/// </summary>
/// <typeparam name="T">The type of result returned by visitor operations.</typeparam>
/// <remarks>
/// The Visitor pattern allows adding new operations to expression trees without modifying
/// the tree node classes themselves. Common implementations include:
/// - Validation visitors that check expression semantic correctness
/// - Complexity analyzers that compute performance metrics
/// - Metric collectors that extract metric dependencies
/// - Variable collectors that identify variable dependencies
/// - Code generators that convert expressions to other formats.
/// </remarks>
public interface IExpressionVisitor<T>
{
    /// <summary>
    /// Visits a binary operator node (AND/OR) in the expression tree.
    /// </summary>
    /// <param name="node">The binary operator node to visit.</param>
    /// <returns>The result of processing the binary operator node.</returns>
    T VisitBinaryOperator(BinaryOperatorNode node);

    /// <summary>
    /// Visits a condition node (leaf node containing metric comparisons) in the expression tree.
    /// </summary>
    /// <param name="node">The condition node to visit.</param>
    /// <returns>The result of processing the condition node.</returns>
    T VisitCondition(ConditionNode node);
}
