namespace Reactive.Expressions.Ast.Visitors;

/// <summary>
/// Visitor interface for arithmetic expressions.
/// </summary>
/// <typeparam name="T">Type parameter.</typeparam>
public interface IArithmeticVisitor<T>
{
    /// <summary>
    /// Visits a constant expression in the arithmetic expression tree.
    /// </summary>
    /// <param name="expression">Expression.</param>
    /// <returns>Transformed type.</returns>
    T VisitConstant(ConstantExpression expression);

    /// <summary>
    /// Visits a variable expression in the arithmetic expression tree.
    /// </summary>
    /// <param name="expression">Expression.</param>
    /// <returns>Transformed type.</returns>
    T VisitVariable(VariableExpression expression);

    /// <summary>
    /// Visits a binary expression in the arithmetic expression tree.
    /// </summary>
    /// <param name="expression">Expression.</param>
    /// <returns>Transformed type.</returns>
    T VisitBinaryOperation(BinaryArithmeticExpression expression);
}
