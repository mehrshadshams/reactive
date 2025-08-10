using Dawn;

namespace Reactive.Expressions.Models;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Comparer for EvaluationResult records.
/// </summary>
public class EvaluationResultComparer : IEqualityComparer<EvaluationResult>
{
    /// <inheritdoc/>
    public bool Equals(EvaluationResult? x, EvaluationResult? y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        return Equals(x.Value, y.Value);
    }

    /// <inheritdoc/>
    public int GetHashCode([DisallowNull] EvaluationResult obj)
    {
        Guard.Argument(obj, nameof(obj)).NotNull();

        return obj.Value.GetHashCode();
    }
}
