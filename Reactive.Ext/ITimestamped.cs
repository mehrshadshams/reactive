namespace Reactive.Ext;

/// <summary>
/// Defines the interface for a record that has timestamp.
/// </summary>
public interface ITimestamped
{
  /// <summary>
  /// Gets the record timestamp.
  /// </summary>
  DateTimeOffset Timestamp { get; }
}
