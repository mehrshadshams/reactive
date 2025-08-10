namespace Reactive.Expressions.Models;

/// <summary>
/// Represents the result of an aggregation operation over a time period.
/// Contains the aggregation type, time period, and computed value.
/// </summary>
/// <param name="NodeName">Identifier for the aggregation (usually the metric name).</param>
/// <param name="AggregationType">Type of aggregation performed (avg, sum, max, min, count).</param>
/// <param name="Period">Time period over which the aggregation was computed.</param>
/// <param name="Value">The computed aggregation result.</param>
public record AggregationResult(string NodeName, AggregationType AggregationType, Period Period, double Value);
