using Dawn;

namespace Reactive.Expressions.Ast;

using System.Collections.Generic;
using Reactive.Expressions.Ast.Visitors;

/// <summary>
/// Represents a constant numeric value in an arithmetic expression.
/// </summary>
public class ConstantExpression : ArithmeticExpression
{
  /// <summary>
  /// Gets or sets the constant numeric value.
  /// </summary>
  public double Value { get; set; }

  /// <inheritdoc/>
  public override double Evaluate(IVariableResolver? variableResolver = null)
  {
    return Value;
  }

  /// <inheritdoc/>
  public override T Accept<T>(IArithmeticVisitor<T> visitor)
  {
    Guard.Argument(visitor, nameof(visitor)).NotNull();
    return visitor.VisitConstant(this);
  }

  /// <inheritdoc/>
  public override HashSet<string> GetVariables()
  {
    return new HashSet<string>();
  }
}
