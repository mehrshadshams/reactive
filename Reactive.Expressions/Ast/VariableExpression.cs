using Dawn;

namespace Reactive.Expressions.Ast;

using System;
using System.Collections.Generic;
using Reactive.Expressions.Ast.Visitors;

/// <summary>
/// Represents a variable reference in an arithmetic expression that gets resolved at evaluation time.
/// Variables enable dynamic threshold calculations where values are determined at runtime rather than compile time.
/// </summary>
/// <remarks>
/// VariableExpression supports:
/// - Runtime variable resolution through IVariableResolver
/// - Dynamic threshold configuration without code changes
/// - Environment-specific parameter substitution
/// - Parameterized expression evaluation
///
/// Examples of variable usage:
/// - "cpu > threshold_multiplier" - where threshold_multiplier is resolved at runtime
/// - "memory > base_threshold * scaling_factor" - using multiple variables
/// - "disk > configuration_threshold" - environment-specific thresholds
///
/// Variables must be defined in the IVariableResolver or evaluation will throw an exception.
/// </remarks>
public class VariableExpression : ArithmeticExpression
{
    /// <summary>
    /// Gets or sets the name of the variable to be resolved at evaluation time.
    /// </summary>
    public required string VariableName { get; set; }

    /// <inheritdoc/>
    public override double Evaluate(IVariableResolver? variableResolver = null)
    {
        if (variableResolver == null)
        {
            throw new InvalidOperationException($"Variable '{VariableName}' cannot be resolved: no variable resolver provided");
        }

        var value = variableResolver.GetVariableValue(VariableName);
        if (value == null)
        {
            throw new InvalidOperationException($"Variable '{VariableName}' is not defined");
        }

        return value.Value;
    }

    /// <inheritdoc/>
    public override T Accept<T>(IArithmeticVisitor<T> visitor)
    {
        Guard.Argument(visitor, nameof(visitor)).NotNull();
        return visitor.VisitVariable(this);
    }

    /// <inheritdoc/>
    public override HashSet<string> GetVariables()
    {
        return new HashSet<string> { VariableName };
    }
}
