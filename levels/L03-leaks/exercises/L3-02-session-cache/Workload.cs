namespace SessionCache;

public sealed class Session
{
    public readonly int Id;
    public readonly byte[] State = new byte[2_000];
    public int Hits;
    public Session(int id) { Id = id; State[0] = (byte)id; }
}

public static class Sessions
{
    static readonly Dictionary<long, Session> Cache = new();
    static long _next;                       // ids are unique across runs

    public static void Reset() { Cache.Clear(); }   // test scaffolding only

    public static (long id, Session session) Create()
    {
        long id = _next++;
        var s = new Session((int)id);
        Cache[id] = s;
        return (id, s);
    }

    public static Session? TryGet(long id) => Cache.TryGetValue(id, out var s) ? s : null;
}

public static class Workload
{
    public static void Reset() => Sessions.Reset();

    public static long Run()
    {
        long checksum = 0;
        var recent = new long[200];          // ids of the most recent sessions
        for (int i = 0; i < 40_000; i++)
        {
            var (id, s) = Sessions.Create();
            s.State[0] = (byte)i;                // run-relative, so the checksum doesn't depend on earlier runs
            recent[i % recent.Length] = id;
            if (i % 5 == 0 && i >= 200)      // a returning user: one of the last 200 sessions
            {
                var back = Sessions.TryGet(recent[(i * 7) % recent.Length]);
                if (back is not null) { back.Hits++; checksum += back.Hits * 11 + back.State[0]; }
            }
            checksum += s.State.Length;
        }
        return checksum;
    }
}
