using Dawn;

namespace Reactive.Expressions.Ast;

using System;
using System.Collections.Generic;
using Reactive.Expressions.Ast.Visitors;

/// <summary>
/// Binary arithmetic operation expression (e.g., 2*variable, variable+5).
/// </summary>
public class BinaryArithmeticExpression : ArithmeticExpression
{
  /// <summary>
  /// Gets or sets the left operand of the binary arithmetic expression.
  /// </summary>
  public required ArithmeticExpression Left { get; set; }

  /// <summary>
  /// Gets or sets the right operand of the binary arithmetic expression.
  /// </summary>
  public required ArithmeticExpression Right { get; set; }

  /// <summary>
  /// Gets or sets the arithmetic operator (Add, Subtract, Multiply, Divide, Modulo).
  /// </summary>
  public ArithmeticOperator Operator { get; set; }

  /// <inheritdoc/>
  public override double Evaluate(IVariableResolver? variableResolver = null)
  {
    var leftValue = Left.Evaluate(variableResolver);
    var rightValue = Right.Evaluate(variableResolver);

    return Operator switch
    {
      ArithmeticOperator.Add => leftValue + rightValue,
      ArithmeticOperator.Subtract => leftValue - rightValue,
      ArithmeticOperator.Multiply => leftValue * rightValue,
      ArithmeticOperator.Divide => rightValue != 0 ? leftValue / rightValue : throw new DivideByZeroException(),
      _ => throw new NotSupportedException($"Arithmetic operator {Operator} not supported"),
    };
  }

  /// <inheritdoc/>
  public override T Accept<T>(IArithmeticVisitor<T> visitor)
  {
    Guard.Argument(visitor, nameof(visitor)).NotNull();

    return visitor.VisitBinaryOperation(this);
  }

  /// <inheritdoc/>
  public override HashSet<string> GetVariables()
  {
    var variables = Left.GetVariables();
    variables.UnionWith(Right.GetVariables());
    return variables;
  }
}
