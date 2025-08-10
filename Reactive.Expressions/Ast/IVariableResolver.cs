namespace Reactive.Expressions.Ast;

using System.Collections.Generic;

/// <summary>
/// Interface for resolving variable names to numeric values during expression evaluation.
/// Implementations can provide static mappings, database lookups, or runtime calculations.
/// </summary>
public interface IVariableResolver
{
    /// <summary>
    /// Gets the set of all variable names known to this resolver.
    /// </summary>
    ISet<string> Variables { get; }

    /// <summary>
    /// Gets the value of a variable by its name.
    /// </summary>
    /// <param name="variableName">Variable name.</param>
    /// <returns>value.</returns>
    double? GetVariableValue(string variableName);

    /// <summary>
    /// Checks if a variable with the given name exists in the resolver.
    /// </summary>
    /// <param name="variableName">Variable name.</param>
    /// <returns><c>true</c> if variable exists.</returns>
    bool HasVariable(string variableName);
}
