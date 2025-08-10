using System.Reactive.Linq;
using Dawn;

namespace Reactive.Ext;

/// <summary>
/// Observable extensions.
/// </summary>
public static class ObservableExtensions
{
  /// <summary>
  /// Projects each element of the input observable into non overlapping windows based on the input timestamp and given window duration.
  /// </summary>
  /// <typeparam name="TSource">Type of input.</typeparam>
  /// <param name="source">Source observable.</param>
  /// <param name="windowDuration">Window duration.</param>
  /// <returns>Transformed observable.</returns>
  public static IObservable<IObservable<TSource>> WindowByTimestamp<TSource>(this IObservable<TSource> source,
    TimeSpan windowDuration)
    where TSource : ITimestamped
  {
    Guard.Argument(source, nameof(source)).NotNull();

    return source.WindowByTimestamp(src => src.Timestamp.Ticks, windowDuration);
  }

  /// <summary>
  /// Projects each element of the input observable into non overlapping windows based on the input timestamp and given window duration.
  /// </summary>
  /// <typeparam name="TSource">Type of input.</typeparam>
  /// <param name="source">Source observable.</param>
  /// <param name="timestampTicksSelector">Timestamp ticks selector.</param>
  /// <param name="windowDuration">Window duration.</param>
  /// <returns>Transformed observable.</returns>
  public static IObservable<IObservable<TSource>> WindowByTimestamp<TSource>(
    this IObservable<TSource> source,
    Func<TSource, long> timestampTicksSelector,
    TimeSpan windowDuration)
  {
    return new WindowByTimestampObservable<TSource>(source, timestampTicksSelector, windowDuration).Publish()
      .RefCount();
  }
}
