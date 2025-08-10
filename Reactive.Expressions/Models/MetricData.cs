using Reactive.Ext;

namespace Reactive.Expressions.Models;

using System;

/// <summary>
/// Represents a single metric data point with name, value, and timestamp.
/// This is the fundamental data unit that flows through the metric processing pipeline.
/// </summary>
/// <remarks>
/// MetricData objects are used to:
/// - Transport metric values from data sources
/// - Trigger expression evaluations based on new data
/// - Maintain temporal ordering for time-based aggregations
/// - Provide context for metric-based conditions.
/// </remarks>
public class MetricData : ITimestamped
{
    /// <summary>
    /// Gets or sets the name/identifier of the metric (e.g., "cpu", "memory", "disk_usage").
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the numeric value of the metric at the given timestamp.
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this metric value was recorded.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Returns a string representation of the metric data for debugging and logging.
    /// </summary>
    public override string ToString()
    {
        return $"{Name}: {Value} at {Timestamp}";
    }
}
