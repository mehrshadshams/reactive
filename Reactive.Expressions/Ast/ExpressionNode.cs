namespace Reactive.Expressions.Ast;

using System;
using Reactive.Expressions.Ast.Visitors;
using Reactive.Expressions.Models;

/// <summary>
/// AST Node types for expression tree.
/// </summary>
public abstract class ExpressionNode
{
    /// <summary>
    /// Gets the unique name of this expression node.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Evaluates this expression node within the context of the provided builder.
    /// </summary>
    /// <param name="builder">Expression builder.</param>
    /// <returns>Observable sequence.</returns>
    public abstract IObservable<EvaluationResult> Evaluate(MetricExpressionBuilder builder);

    /// <summary>
    /// Accepts a visitor for traversing the expression tree using the Visitor pattern.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    /// <param name="visitor">Visitor.</param>
    /// <returns>Transformed object.</returns>
    public abstract T Accept<T>(IExpressionVisitor<T> visitor);
}
