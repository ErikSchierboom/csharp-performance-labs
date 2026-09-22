namespace SessionCache;

public sealed class Session
{
    public readonly int Id;
    public readonly byte[] State = new byte[2_000];
    public int Hits;
    public Session(int id) { Id = id; State[0] = (byte)id; }
}

/// <summary>A small least-recently-used cache: bounded, so it cannot grow without limit.</summary>
public sealed class LruCache<TKey, TValue> where TKey : notnull
{
    readonly int _capacity;
    readonly Dictionary<TKey, LinkedListNode<(TKey Key, TValue Value)>> _map = new();
    readonly LinkedList<(TKey Key, TValue Value)> _order = new();   // most recent at the front

    public LruCache(int capacity) => _capacity = capacity;

    public void Set(TKey key, TValue value)
    {
        if (_map.TryGetValue(key, out var existing)) _order.Remove(existing);
        _map[key] = _order.AddFirst((key, value));
        if (_map.Count > _capacity)
        {
            var last = _order.Last!;
            _order.RemoveLast();
            _map.Remove(last.Value.Key);
        }
    }

    public bool TryGet(TKey key, out TValue value)
    {
        if (_map.TryGetValue(key, out var node))
        {
            _order.Remove(node); _order.AddFirst(node);     // touch
            value = node.Value.Value; return true;
        }
        value = default!; return false;
    }

    public void Clear() { _map.Clear(); _order.Clear(); }
}

public static class Sessions
{
    static readonly LruCache<long, Session> Cache = new(capacity: 1_000);   // bounded: this is the fix
    static long _next;

    public static void Reset() { Cache.Clear(); }

    public static (long id, Session session) Create()
    {
        long id = _next++;
        var s = new Session((int)id);
        Cache.Set(id, s);
        return (id, s);
    }

    public static Session? TryGet(long id) => Cache.TryGet(id, out var s) ? s : null;
}

public static class Workload
{
    public static void Reset() => Sessions.Reset();

    public static long Run()
    {
        long checksum = 0;
        var recent = new long[200];
        for (int i = 0; i < 40_000; i++)
        {
            var (id, s) = Sessions.Create();
            s.State[0] = (byte)i;                // run-relative, so the checksum doesn't depend on earlier runs
            recent[i % recent.Length] = id;
            if (i % 5 == 0 && i >= 200)
            {
                var back = Sessions.TryGet(recent[(i * 7) % recent.Length]);
                if (back is not null) { back.Hits++; checksum += back.Hits * 11 + back.State[0]; }
            }
            checksum += s.State.Length;
        }
        return checksum;
    }
}
