using Dawn;

namespace Reactive.Expressions;

using System;

/// <summary>
/// Represents a time period with start and end timestamps.
/// Used for time-windowed aggregations and result tracking.
/// </summary>
/// <param name="Start">The start timestamp of the period.</param>
/// <param name="End">The end timestamp of the period.</param>
public record Period
{
    public static readonly Period Empty = new(DateTime.MinValue, DateTime.MaxValue);

    /// <summary>
    /// Initializes a new instance of the <see cref="Period"/> class.
    /// </summary>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    public Period(DateTime start, DateTime end)
    {
        /*Guard.Argument(start, nameof(start)).NotDefault();
        Guard.Argument(end, nameof(end)).NotDefault();
        if (end < start)
        {
            throw new ArgumentException("End time must be greater than or equal to start time.");
        }*/

        Start = start;
        End = end;
    }

    /// <summary>
    /// Gets the start time of this period in ISO 8601 format.
    /// </summary>
    public DateTime Start { get; init; }

    /// <summary>
    /// Gets the end time of this period in ISO 8601 format.
    /// </summary>
    public DateTime End { get; init; }

    /// <summary>
    /// Gets the duration of this time period.
    /// </summary>
    public TimeSpan Duration => End - Start;

    /// <summary>
    /// Gets a value indicating whether gets whether this period represents an empty/zero duration.
    /// </summary>
    public bool IsEmpty => Duration == TimeSpan.Zero;

    /// <summary>
    /// Joins two periods to create a period that encompasses both.
    /// Returns the earliest start time and latest end time.
    /// </summary>
    /// <param name="left">First period to join.</param>
    /// <param name="right">Second period to join.</param>
    /// <returns>Combined period spanning both input periods.</returns>
    public static Period Join(Period left, Period right)
    {
        Guard.Argument(left, nameof(left)).NotNull();
        Guard.Argument(right, nameof(right)).NotNull();

        if (left.IsEmpty)
        {
            return right;
        }

        if (right.IsEmpty)
        {
            return left;
        }

        return new Period(
            left.Start < right.Start ? left.Start : right.Start,
            left.End > right.End ? left.End : right.End);
    }

    /// <summary>
    /// Checks if a specific timestamp falls within this period (inclusive).
    /// </summary>
    /// <param name="time">Timestamp to check.</param>
    /// <returns>True if the time is within the period bounds.</returns>
    public bool Contains(DateTime time)
    {
        return time >= Start && time <= End;
    }

    /// <summary>
    /// Returns a string representation of the period in ISO 8601 format.
    /// </summary>
    public override string ToString()
    {
        return $"{Start:O}..{End:O}";
    }

    /// <summary>
    /// Creates a period representing a single point in time (start == end).
    /// </summary>
    /// <param name="timestamp">The timestamp for the single-point period.</param>
    /// <returns>Period with identical start and end times.</returns>
    public static Period SinglePoint(DateTime timestamp) => new Period(timestamp, timestamp);
}
