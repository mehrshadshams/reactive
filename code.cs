namespace Reactive;
    
    using System;

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

    /// <summary>
    /// Base interface for observers that can dispose of a resource on a terminal notification
    /// or when disposed itself.
    /// </summary>
    /// <typeparam name="T">Type parameter.</typeparam>
    internal interface ISafeObserver<in T> : IObserver<T>, IDisposable
    {
        void SetResource(IDisposable resource);
    }

    /// <summary>
    /// Wraps a <see cref="ConcurrentDictionary{TKey, TValue}"/> to add more functionality.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    internal sealed class Map<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, TValue> _map;

        /// <summary>
        /// Initializes a new instance of the <see cref="Map{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="capacity">Initial capacity.</param>
        /// <param name="comparer">Comparer.</param>
        public Map(int? capacity, IEqualityComparer<TKey> comparer)
        {
            if (capacity.HasValue)
            {
                _map = new ConcurrentDictionary<TKey, TValue>(DefaultConcurrencyLevel, capacity.Value, comparer);
            }
            else
            {
                _map = new ConcurrentDictionary<TKey, TValue>(comparer);
            }
        }

        /// <summary>
        /// Gets map values.
        /// </summary>
        public IEnumerable<TValue> Values => _map.Values.ToArray();

        private static int DefaultConcurrencyLevel => 4 * Environment.ProcessorCount;

        /// <summary>
        /// Get or add value.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="valueFactory">Value factory.</param>
        /// <param name="added">Flag indicating if an item was added.</param>
        /// <returns>Value.</returns>
        public TValue GetOrAdd(TKey key, Func<TValue> valueFactory, out bool added)
        {
            added = false;
            TValue newValue = default(TValue);
            bool hasNewValue = false;
            TValue value;
            while (!_map.TryGetValue(key, out value))
            {
                if (!hasNewValue)
                {
                    newValue = valueFactory();
                    hasNewValue = true;
                }

                if (_map.TryAdd(key, newValue))
                {
                    added = true;
                    return newValue;
                }
            }

            return value;
        }

        /// <summary>
        /// Remove key.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Flag whether the key was removed.</returns>
        public bool Remove(TKey key)
        {
            TValue value;
            return _map.TryRemove(key, out value);
        }
    }

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


/// <summary>
    /// Observable extensions.
    /// </summary>
    public static class ObservableExtensions
    {
        /// <summary>
        /// Computes the moving average on the input observable given the window size.
        /// </summary>
        /// <param name="source">Source observable.</param>
        /// <param name="windowSize">Window size. (positive).</param>
        /// <returns>Transformed observable.</returns>
        public static IObservable<MovingStatModel> MovingAverage(this IObservable<double> source, int windowSize)
        {
            Guard.Argument(source, nameof(source)).NotNull();
            Guard.Argument(windowSize, nameof(windowSize)).GreaterThan(0);

            return new MovingAverageObservable(source, windowSize).Publish().RefCount();
        }

        /// <summary>
        /// Projects each element of the input observable into non overlapping windows based on the input timestamp and given window duration.
        /// </summary>
        /// <typeparam name="TSource">Type of input.</typeparam>
        /// <param name="source">Source observable.</param>
        /// <param name="windowDuration">Window duration.</param>
        /// <returns>Transformed observable.</returns>
        public static IObservable<IObservable<TSource>> WindowByTimestamp<TSource>(this IObservable<TSource> source, TimeSpan windowDuration)
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
            return new WindowByTimestampObservable<TSource>(source, timestampTicksSelector, windowDuration).Publish().RefCount();
        }

        /// <summary>
        /// Returns an observable sequence that contains only distinct contiguous elements with their appearance count.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">An observable sequence to retain distinct contiguous elements for.</param>
        /// <returns>An observable sequence only containing the distinct contiguous elements from the source sequence.</returns>
        public static IObservable<DistinctUntilChangedModel<TSource>> DistinctUntilChangedWithCount<TSource>(this IObservable<TSource> source)
            where TSource : IEquatable<TSource>
        {
            Guard.Argument(source, nameof(source)).NotNull();
            return new DistinctUntilChangedWithCountObservable<TSource>(source);
        }

        /// <summary>
        /// Returns an observable sequence that contains only distinct contiguous elements with their appearance count.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">An observable sequence to retain distinct contiguous elements for.</param>
        /// <returns>An observable sequence only containing the distinct contiguous elements from the source sequence.</returns>
        public static IObservable<DistinctUntilChangedModel<TSource>> DistinctUntilChangedWithCount<TSource>(this IObservable<TSource> source, IEqualityComparer<TSource> equalityComparer)
            where TSource : IEquatable<TSource>
        {
            Guard.Argument(source, nameof(source)).NotNull();
            Guard.Argument(equalityComparer, nameof(equalityComparer)).NotNull();

            return new DistinctUntilChangedWithCountObservable<TSource>(source, equalityComparer);
        }

        /// <summary>
        /// Returns an observable sequence that contains only distinct contiguous elements with their appearance count.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">An observable sequence to retain distinct contiguous elements for.</param>
        /// <param name="timeout">timeout.</param>
        /// <returns>An observable sequence only containing the distinct contiguous elements from the source sequence.</returns>
        public static IObservable<TSource> DistinctUntilChangedWithTimeout<TSource>(this IObservable<TSource> source, TimeSpan timeout)
            where TSource : IEquatable<TSource>
        {
            Guard.Argument(source, nameof(source)).NotNull();
            return new DistinctUntilChangedWithTimeoutObservable<TSource>(source, timeout);
        }

        /// <summary>
        /// Returns an observable sequence that contains only distinct contiguous elements with their appearance count.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">An observable sequence to retain distinct contiguous elements for.</param>
        /// <param name="timeout">Timeout.</param>
        /// <returns>An observable sequence only containing the distinct contiguous elements from the source sequence.</returns>
        public static IObservable<TSource> DistinctUntilChangedWithTimeout<TSource>(this IObservable<TSource> source, TimeSpan timeout, IEqualityComparer<TSource> equalityComparer)
        {
            Guard.Argument(source, nameof(source)).NotNull();
            return new DistinctUntilChangedWithTimeoutObservable<TSource>(source, timeout, equalityComparer);
        }
    }
