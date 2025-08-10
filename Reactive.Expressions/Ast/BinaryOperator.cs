namespace Reactive.Expressions.Ast;

/// <summary>
/// Binary operator for logical expressions.
/// </summary>
public enum BinaryOperator
{
  /// <summary>
  /// And operator, used to combine two conditions where both must be true.
  /// </summary>
  And,

  /// <summary>
  /// Or operator, used to combine two conditions where at least one must be true.
  /// </summary>
  Or,
}
