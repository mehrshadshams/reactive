namespace Reactive.Expressions.Ast;

using System;
using System.Collections.Generic;
using Reactive.Expressions.Ast.Visitors;

/// <summary>
/// Abstract base class for arithmetic expressions used in threshold calculations.
/// Supports evaluation with variable resolution and visitor pattern traversal.
/// </summary>
/// <remarks>
/// ArithmeticExpression enables complex threshold calculations like:
/// - Constants: "5.0"
/// - Variables: "threshold_multiplier"
/// - Binary operations: "threshold * 2.0"
/// - Nested expressions: "(base_threshold + offset) * multiplier"
///
/// The expression tree can be evaluated at runtime with variable substitution,
/// allowing for dynamic threshold calculations based on external configuration.
/// </remarks>
public abstract class ArithmeticExpression
{
  /// <summary>
  /// Evaluates this arithmetic expression to a numeric result.
  /// Variables are resolved using the provided resolver, if any.
  /// </summary>
  /// <param name="variableResolver">Optional resolver for variable values.</param>
  /// <returns>The computed numeric result.</returns>
  /// <exception cref="InvalidOperationException">Thrown when variables cannot be resolved.</exception>
  public abstract double Evaluate(IVariableResolver? variableResolver = null);

  /// <summary>
  /// Accepts a visitor for traversing the expression tree using the Visitor pattern.
  /// This enables operations like validation, transformation, or analysis without
  /// modifying the expression classes themselves.
  /// </summary>
  /// <typeparam name="T">Return type of the visitor operation.</typeparam>
  /// <param name="visitor">Visitor implementation to apply.</param>
  /// <returns>Result of the visitor operation.</returns>
  public abstract T Accept<T>(IArithmeticVisitor<T> visitor);

  /// <summary>
  /// Extracts all variable names referenced in this expression and its sub-expressions.
  /// Used for dependency analysis and validation.
  /// </summary>
  /// <returns>Set of unique variable names found in the expression.</returns>
  public abstract HashSet<string> GetVariables();
}
