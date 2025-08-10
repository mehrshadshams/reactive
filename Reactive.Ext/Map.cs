using System.Collections.Concurrent;

namespace Reactive.Ext;

/// <summary>
/// Wraps a <see cref="ConcurrentDictionary{TKey,TValue}"/> to add more functionality.
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
