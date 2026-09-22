namespace SessionGateway;

public sealed class Presence { public event Action<int>? Changed; public void Publish(int v) => Changed?.Invoke(v); }
public sealed class Profile { public readonly byte[] Data = new byte[2_000]; public int Id; }
public sealed class SessionView : IDisposable
{
    readonly Presence _p; readonly Action<int> _handler;
    readonly byte[] _scratch = new byte[8_000];
    public int Seen;
    public SessionView(Presence p) { _p = p; _handler = v => { Seen++; _scratch[v % _scratch.Length] = 1; }; p.Changed += _handler; }
    public void Dispose() => _p.Changed -= _handler;                            // L3-01
}
public static class Workload
{
    static Presence _presence = new();
    static Dictionary<string, Profile> _profiles = new();
    static readonly Queue<string> _order = new();
    static readonly List<Func<int>> Callbacks = new();

    public static void Reset() { _presence = new Presence(); _profiles = new(); _order.Clear(); Callbacks.Clear(); }   // test scaffolding only

    public static long Run()
    {
        long checksum = 0;
        for (int i = 0; i < 1_500; i++)
        {
            using var view = new SessionView(_presence);
            _presence.Publish(i);
            checksum += view.Seen * 3 + i % 5;
            _profiles["u" + i] = new Profile { Id = i };
            _order.Enqueue("u" + i);
            while (_order.Count > 300) _profiles.Remove(_order.Dequeue());       // bounded, oldest first (L3-02); lookups only reach back 50
            if (i >= 100 && _profiles.TryGetValue("u" + (i - 50), out var back)) checksum += back.Id % 7;
            var report = new byte[6_000];
            int idx = i % report.Length;
            report[idx] = (byte)i;
            int value = report[idx];                                             // capture only the small value (L3-06)
            Callbacks.Add(() => value);
        }
        foreach (var cb in Callbacks) checksum += cb();
        return checksum;
    }
}
