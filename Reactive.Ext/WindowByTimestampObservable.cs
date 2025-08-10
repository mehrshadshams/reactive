using System.Reactive.Linq;
using Dawn;

namespace Reactive.Ext;

/// <summary>
/// Window by timestamped observable.
/// </summary>
/// <typeparam name="T">Type of input.</typeparam>
internal class WindowByTimestampObservable<T> : IObservable<IObservable<T>>
{
  private readonly IObservable<T> _source;
  private readonly Func<T, long> _keySelector;
  private readonly TimeSpan _duration;
  private readonly int _bufferMilliseconds;

  /// <summary>
  /// Initializes a new instance of the <see cref="WindowByTimestampObservable{T}"/> class.
  /// </summary>
  /// <param name="source">source observable.</param>
  /// <param name="keySelector">key selector.</param>
  /// <param name="duration">window duration.</param>
  /// <param name="bufferMilliseconds">Determines how many data points we keep in memory before we sort and group by timestamp.</param>
  public WindowByTimestampObservable(IObservable<T> source, Func<T, long> keySelector, TimeSpan duration, int bufferMilliseconds = 1000)
  {
    _source = Guard.Argument(source, nameof(source)).Value;
    _keySelector = Guard.Argument(keySelector, nameof(keySelector)).NotNull().Value;
    _duration = duration;
    _bufferMilliseconds = Guard.Argument(bufferMilliseconds, nameof(bufferMilliseconds)).GreaterThan(0);
  }

  /// <inheritdoc/>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Observer disposes itself.")]
  public IDisposable Subscribe(IObserver<IObservable<T>> observer)
  {
    // buffer and sort. deal with async data points arriving at the same time.
    return _source
      .Buffer(TimeSpan.FromMilliseconds(_bufferMilliseconds))
      .SelectMany(list => list.OrderBy(o => _keySelector(o)))
      .Subscribe(new WindowByTimestampObserver<T>(observer, _keySelector, _duration));
  }
}
