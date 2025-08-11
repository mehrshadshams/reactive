using Dawn;

namespace Reactive.Expressions.Models;

using System.Collections.Generic;
using Reactive.Expressions;

/// <summary>
/// Represents the result of evaluating a node in an expression tree.
/// Contains the node identifier, boolean evaluation result, and the time period over which the evaluation occurred.
/// </summary>
/// <param name="NodeName">The unique identifier for the expression node that was evaluated.</param>
/// <param name="Value">The boolean result of the node evaluation (true if condition was met, false otherwise).</param>
/// <param name="Period">The time period during which this evaluation result is valid.</param>
public record EvaluationResult(string NodeName, bool Value, Period Period)
{
    /// <summary>
    /// Gets the default equality comparer for EvaluationResult instances.
    /// Uses EvaluationResultComparer for consistent comparison logic.
    /// </summary>
    public static readonly IEqualityComparer<EvaluationResult> DefaultComparer = new EvaluationResultComparer();

    /// <summary>
    /// Combines this evaluation result with another using logical AND operation.
    /// The result is true only if both this and the other evaluation are true.
    /// </summary>
    /// <param name="other">The other evaluation result to combine with.</param>
    /// <returns>A new EvaluationResult representing the AND combination of both results.</returns>
    public EvaluationResult And(EvaluationResult other)
    {
        Guard.Argument(other, nameof(other)).NotNull();

        bool value = Value && other.Value;
        string name = $"{NodeName}_and_{other.NodeName}";
        return CombineWith(other, name, value);
    }

    /// <summary>
    /// Combines this evaluation result with another using logical OR operation.
    /// The result is true if either this or the other evaluation (or both) are true.
    /// </summary>
    /// <param name="other">The other evaluation result to combine with.</param>
    /// <returns>A new EvaluationResult representing the OR combination of both results.</returns>
    public EvaluationResult Or(EvaluationResult other)
    {
        Guard.Argument(other, nameof(other)).NotNull();

        bool value = Value || other.Value;
        string name = $"{NodeName}_or_{other.NodeName}";
        return CombineWith(other, name, value);
    }

    private EvaluationResult CombineWith(EvaluationResult other, string name, bool value)
    {
        Guard.Argument(other, nameof(other)).NotNull();

        return new EvaluationResult(name, value, Period.Join(Period, other.Period));
    }
}
