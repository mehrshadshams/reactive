using Dawn;

namespace Reactive.Ext;

/// <summary>
/// Safe observers that can dispose of a resource on a terminal notification
/// or when disposed itself.
/// Based on this file:
/// https://github.com/dotnet/reactive/blob/main/Rx.NET/Source/src/System.Reactive/Internal/SafeObserver.cs.
/// </summary>
/// <typeparam name="TSource">Type parameter.</typeparam>
internal abstract class SafeObserver<TSource> : ISafeObserver<TSource>, IObserver<TSource>, IDisposable
{
  private IDisposable _disposable;
  private bool _disposedValue;

  /// <summary>
  /// Wrap an observer in a safe observer.
  /// </summary>
  /// <param name="observer">Observer.</param>
  /// <returns>Safe observer.</returns>
  public static ISafeObserver<TSource> Wrap(IObserver<TSource> observer)
  {
    return new WrappingSafeObserver(Guard.Argument(observer, nameof(observer)).NotNull().Value);
  }

  /// <inheritdoc/>
  public void Dispose()
  {
    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }

  /// <inheritdoc/>
  public abstract void OnCompleted();

  /// <inheritdoc/>
  public abstract void OnError(Exception error);

  /// <inheritdoc/>
  public abstract void OnNext(TSource value);

  /// <inheritdoc/>
  public void SetResource(IDisposable resource)
  {
    _disposable = resource;
  }

  /// <summary>
  /// Dispose observer.
  /// </summary>
  /// <param name="disposing">disposing.</param>
  protected virtual void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (disposing)
      {
        _disposable?.Dispose();
      }

      _disposedValue = true;
    }
  }

  private sealed class WrappingSafeObserver : SafeObserver<TSource>
  {
    private readonly IObserver<TSource> _observer;

    public WrappingSafeObserver(IObserver<TSource> observer)
    {
      _observer = observer;
    }

    public override void OnCompleted()
    {
      using (this)
      {
        _observer.OnCompleted();
      }
    }

    public override void OnError(Exception error)
    {
      using (this)
      {
        _observer.OnError(error);
      }
    }

    public override void OnNext(TSource value)
    {
      bool noError = false;
      try
      {
        _observer.OnNext(value);
        noError = true;
      }
      finally
      {
        if (!noError)
        {
          Dispose();
        }
      }
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
    }
  }
}
