using Dawn;

namespace Reactive.Expressions.Ast;

using System;
using Reactive.Expressions;
using Reactive.Expressions.Ast.Visitors;
using Reactive.Expressions.Models;

/// <summary>
/// Represents a condition node in an expression tree.
/// </summary>
public class ConditionNode : ExpressionNode
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ConditionNode"/> class with the specified condition information.
  /// </summary>
  /// <param name="condition">Condition.</param>
  public ConditionNode(ConditionInfo condition)
  {
    Condition = Guard.Argument(condition, nameof(condition)).NotNull();
    Name = $"{condition.RawExpression}";
  }

  /// <summary>
  /// Gets the condition information that defines the metric comparison logic.
  /// </summary>
  public ConditionInfo Condition { get; private set; }

  /// <inheritdoc/>
  public override string Name { get; }

  /// <inheritdoc/>
  public override IObservable<EvaluationResult> Evaluate(MetricExpressionBuilder builder)
  {
    Guard.Argument(builder, nameof(builder)).NotNull();

    if (Condition.IsAggregation)
    {
      return builder.BuildSlidingWindowAggregation(this);
    }
    else
    {
      return builder.BuildSimpleConditionObservable(this);
    }
  }

  /// <inheritdoc/>
  public override T Accept<T>(IExpressionVisitor<T> visitor)
  {
    Guard.Argument(visitor, nameof(visitor)).NotNull();

    return visitor.VisitCondition(this);
  }
}
