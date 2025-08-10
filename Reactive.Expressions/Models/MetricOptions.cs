namespace Reactive.Expressions.Models;

using System;

/// <summary>
/// Configuration options for metric processing and evaluation.
/// Controls timing, windows, and other operational parameters.
/// </summary>
public class MetricOptions
{
    /// <summary>
    /// Gets or sets default time window for metric processing when not specified in expressions.
    /// This affects how metric streams are buffered and processed.
    /// </summary>
    public TimeSpan TimeWindow { get; set; } = TimeSpan.FromSeconds(5);
}
