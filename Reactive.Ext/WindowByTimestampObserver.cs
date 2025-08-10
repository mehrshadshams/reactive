using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace Reactive.Ext;

/// <summary>
/// Window by timestamp observer. This is loosely based on
/// https://github.com/dotnet/reactive/blob/main/Rx.NET/Source/src/System.Reactive/Linq/Observable/GroupByUntil.cs.
/// </summary>
/// <typeparam name="T">Type parameter.</typeparam>
public class WindowByTimestampObserver<T> : IObserver<T>, IDisposable
{
  private readonly CompositeDisposable _disposables = new();
  private readonly TimeSpan _duration;
  private readonly object _gate = new object();
  private readonly Map<long, ISubject<T>> _map;
  private readonly IObserver<IObservable<T>> _observer;
  private readonly Func<T, long> _keySelector;
  private readonly Func<long, TimeSpan, long> _windowIdSelector = (x, y) => x / y.Ticks;

  private ISubject<T> _last;

  private bool _disposedValue;

  /// <summary>
  /// Initializes a new instance of the <see cref="WindowByTimestampObserver{T}"/> class.
  /// </summary>
  /// <param name="observer">Source observer.</param>
  /// <param name="lag">Lag.</param>
  /// <param name="threshold">Threshold.</param>
  /// <param name="influence">Influence.</param>
  public WindowByTimestampObserver(IObserver<IObservable<T>> observer, Func<T, long> keySelector, TimeSpan duration)
  {
    _observer = observer;
    _keySelector = keySelector;
    _duration = duration;
    _map = new Map<long, ISubject<T>>(4, EqualityComparer<long>.Default);
  }

  /// <inheritdoc/>
  public void Dispose()
  {
    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }

  /// <inheritdoc/>
  public void OnCompleted()
  {
    using (this)
    {
      foreach (ISubject<T> value in _map.Values)
      {
        value.OnCompleted();
      }

      ForwardOnCompleted();
    }
  }

  /// <inheritdoc/>
  public void OnError(Exception error)
  {
    using (this)
    {
      foreach (ISubject<T> value in _map.Values)
      {
        value.OnError(error);
      }

      lock (_gate)
      {
        ForwardOnError(error);
      }
    }
  }

  /// <inheritdoc/>
  public void OnNext(T item)
  {
    long windowId;
    bool fireNewMapEntry;
    ISubject<T> writer;

#pragma warning disable CA1031 // Do not catch general exception types
    try
    {
      long key = _keySelector(item);
      windowId = _windowIdSelector(key, _duration);
      writer = _map.GetOrAdd(windowId, () => new Subject<T>(), out fireNewMapEntry);
    }
    catch (Exception ex)
    {
      OnError(ex);
      return;
    }
#pragma warning restore CA1031 // Do not catch general exception types

    if (fireNewMapEntry)
    {
      WriterObserver writerObserver = new WriterObserver(this, windowId, writer);
      _disposables.Add(writerObserver);
      writerObserver.SetResource(writer.SubscribeSafe(writerObserver));
      _observer.OnNext(writer);
      _last?.OnCompleted();
      _last = writer;
    }

    writer.OnNext(item);
  }

  /// <summary>
  /// Dispose.
  /// </summary>
  /// <param name="disposing">disposing flag.</param>
  protected virtual void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (disposing)
      {
        _disposables?.Dispose();
      }

      _disposedValue = true;
    }
  }

  private void ForwardOnError(Exception error)
  {
    _observer.OnError(error);
  }

  private void ForwardOnCompleted()
  {
    _observer.OnCompleted();
  }

  /// <summary>
  /// Observer that ensures we perform clean up and forward errors in the parent object when a window is finished or error occurs.
  /// </summary>
  private class WriterObserver : SafeObserver<T>
  {
    private readonly long _currentWindowId;
    private readonly WindowByTimestampObserver<T> _parent;
    private readonly ISubject<T> _writer;

    public WriterObserver(WindowByTimestampObserver<T> parent, long windowId, ISubject<T> writer)
    {
      _parent = parent;
      _currentWindowId = windowId;
      _writer = writer;
    }

    public override void OnCompleted()
    {
      if (_parent._map.Remove(_currentWindowId))
      {
        _writer.OnCompleted();
      }

      _parent._disposables?.Remove(this);
    }

    public override void OnError(Exception error)
    {
      _parent.ForwardOnError(error);
      Dispose();
    }

    public override void OnNext(T value)
    {
      long key = _parent._keySelector(value);
      long windowId = _parent._windowIdSelector(key, _parent._duration);
      if (windowId != _currentWindowId)
      {
        OnCompleted();
      }
    }
  }
}
