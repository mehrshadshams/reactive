using Dawn;

namespace Reactive.Expressions;

using System;
using System.Linq;
using System.Reactive.Linq;
using Reactive.Expressions.Ast;
using Reactive.Expressions.Ast.Visitors;
using Reactive.Expressions.Models;

/// <summary>
/// Binary operator node representing operations like AND/OR between two expressions.
/// </summary>
public class BinaryOperatorNode : ExpressionNode
{
  /// <summary>
  /// Initializes a new instance of the <see cref="BinaryOperatorNode"/> class.
  /// </summary>
  /// <param name="left">Left expression.</param>
  /// <param name="right">Right expression.</param>
  /// <param name="op">Operator.</param>
  public BinaryOperatorNode(ExpressionNode left, ExpressionNode right, BinaryOperator op)
  {
    Left = Guard.Argument(left, nameof(left)).NotNull();
    Right = Guard.Argument(right, nameof(right)).NotNull();
    Operator = op;
    Name = $"{op}_{left.Name}_{right.Name}";
  }

  /// <inheritdoc/>
  public override string Name { get; }

  /// <summary>
  /// Gets the left operand expression node.
  /// </summary>
  public ExpressionNode Left { get; private set; }

  /// <summary>
  /// Gets the right operand expression node.
  /// </summary>
  public ExpressionNode Right { get; private set; }

  /// <summary>
  /// Gets the binary operator (And, Or) used to combine the left and right expressions.
  /// </summary>
  public BinaryOperator Operator { get; private set; }

  /// <inheritdoc/>
  public override IObservable<EvaluationResult> Evaluate(MetricExpressionBuilder builder)
  {
    var leftObs = Left.Evaluate(builder);
    var rightObs = Right.Evaluate(builder);

    var result = Operator switch
    {
      BinaryOperator.And => leftObs.CombineLatest(rightObs, (a, b) =>
      {
        return a.And(b);
      }),
      BinaryOperator.Or => leftObs.CombineLatest(rightObs, (a, b) =>
      {
        return a.Or(b);
      }),
      _ => throw new NotSupportedException($"Operator {Operator} not supported"),
    };

    /* TODO: Check if Merge would work?
    var merge = leftObs.Merge(rightObs);

    var result = Operator switch
    {
      BinaryOperator.And => merge.Scan(new EvaluationResult(string.Empty, true, Period.SinglePoint(DateTime.MinValue)),
        (state, acc) =>
        {
          return acc with
          {
            Value = acc.Value & state.Value,
          };
        }),
      BinaryOperator.Or => merge.Scan(new EvaluationResult(string.Empty, false, Period.SinglePoint(DateTime.MinValue)),
        (state, acc) =>
        {
          return acc with
          {
            Value = acc.Value || state.Value,
          };
        }),
      _ => throw new ArgumentOutOfRangeException()
    };
    */

    return result
        .Select(x =>
        {
          return x with { NodeName = Name };
        });
  }

  /// <inheritdoc/>
  public override T Accept<T>(IExpressionVisitor<T> visitor)
  {
    Guard.Argument(visitor, nameof(visitor)).NotNull();
    return visitor.VisitBinaryOperator(this);
  }
}
