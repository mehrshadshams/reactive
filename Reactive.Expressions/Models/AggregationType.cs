namespace Reactive.Expressions.Models;

/// <summary>
/// Type of aggregation to perform on metric data.
/// </summary>
public enum AggregationType
{
    /// <summary>
    /// Average value over the specified time period.
    /// </summary>
    Average,

    /// <summary>
    /// Sum of all values over the specified time period.
    /// </summary>
    Sum,

    /// <summary>
    /// Maximum value observed over the specified time period.
    /// </summary>
    Max,

    /// <summary>
    /// Minimum value observed over the specified time period.
    /// </summary>
    Min,
}
