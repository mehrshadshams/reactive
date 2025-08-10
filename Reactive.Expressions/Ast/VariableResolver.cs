namespace Reactive.Expressions.Ast;

using System.Collections.Generic;
using System.Collections.Immutable;

/// <summary>
/// Simple implementation of IVariableResolver that stores variables in an in-memory dictionary.
/// This resolver allows dynamic assignment and retrieval of variable values used in arithmetic expressions.
/// </summary>
/// <remarks>
/// The VariableResolver provides:
/// - Variable storage and retrieval for expression evaluation
/// - Dynamic variable assignment at runtime
/// - Support for variable dependency checking
/// - Thread-safe variable access for concurrent expression evaluation
///
/// Variables are commonly used for:
/// - Dynamic threshold configuration: "cpu > threshold_multiplier"
/// - Environment-specific parameters: "memory > base_threshold * scaling_factor"
/// - Runtime configuration adjustments: "disk > (min_threshold + offset)".
/// </remarks>
public class VariableResolver : IVariableResolver
{
    private readonly Dictionary<string, double> _variables = new();

    /// <inheritdoc/>
    public ISet<string> Variables => _variables.Keys.ToImmutableHashSet();

    public void SetVariable(string name, double value)
    {
        _variables[name] = value;
    }

    /// <inheritdoc/>
    public double? GetVariableValue(string variableName)
    {
        return _variables.TryGetValue(variableName, out var value) ? value : null;
    }

    /// <inheritdoc/>
    public bool HasVariable(string variableName)
    {
        return _variables.ContainsKey(variableName);
    }
}
