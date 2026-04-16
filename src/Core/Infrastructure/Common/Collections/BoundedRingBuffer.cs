namespace CalloraVoipSdk.Core.Infrastructure.Common.Collections;

/// <summary>
/// Thread-safe bounded ring buffer that keeps the most recent items.
/// When capacity is reached, the oldest item is overwritten.
/// </summary>
internal sealed class BoundedRingBuffer<T> where T : class
{
    private readonly T?[] _entries;
    private readonly object _sync = new();
    private int _writeIndex;
    private int _count;

    /// <summary>
    /// Creates a new ring buffer with fixed <paramref name="capacity"/>.
    /// </summary>
    /// <param name="capacity">Maximum number of items retained.</param>
    public BoundedRingBuffer(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "Capacity must be greater than zero.");

        _entries = new T[capacity];
    }

    /// <summary>
    /// Maximum number of retained items.
    /// </summary>
    public int Capacity => _entries.Length;

    /// <summary>
    /// Adds one item and overwrites the oldest entry when full.
    /// </summary>
    /// <param name="item">Item to append.</param>
    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);

        lock (_sync)
        {
            _entries[_writeIndex] = item;
            _writeIndex = (_writeIndex + 1) % _entries.Length;
            if (_count < _entries.Length)
                _count++;
        }
    }

    /// <summary>
    /// Returns a stable oldest-to-newest snapshot.
    /// </summary>
    public T[] Snapshot()
    {
        lock (_sync)
        {
            if (_count == 0)
                return [];

            var snapshot = new T[_count];
            var start = (_writeIndex - _count + _entries.Length) % _entries.Length;
            for (var i = 0; i < _count; i++)
            {
                var index = (start + i) % _entries.Length;
                snapshot[i] = _entries[index]!;
            }

            return snapshot;
        }
    }
}
