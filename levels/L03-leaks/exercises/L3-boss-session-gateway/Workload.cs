namespace SessionGateway;

public sealed class Presence { public event Action<int>? Changed; public void Publish(int v) => Changed?.Invoke(v); }
public sealed class Profile { public readonly byte[] Data = new byte[2_000]; public int Id; }
public sealed class SessionView
{
    readonly byte[] _scratch = new byte[8_000];
    public int Seen;
    public SessionView(Presence p) => p.Changed += v => { Seen++; _scratch[v % _scratch.Length] = 1; };
}
public static class Workload
{
    static Presence _presence = new();
    static Dictionary<string, Profile> _profiles = new();
    static readonly List<Func<int>> Callbacks = new();

    public static void Reset() { _presence = new Presence(); _profiles = new(); Callbacks.Clear(); }   // test scaffolding only

    public static long Run()
    {
        long checksum = 0;
        for (int i = 0; i < 1_500; i++)
        {
            var view = new SessionView(_presence);                              // subscribes to the shared presence feed
            _presence.Publish(i);
            checksum += view.Seen * 3 + i % 5;
            _profiles["u" + i] = new Profile { Id = i };                         // "cache every profile we have loaded"
            if (i >= 100 && _profiles.TryGetValue("u" + (i - 50), out var back)) checksum += back.Id % 7;
            var report = new byte[6_000];                                        // a scratch report...
            int idx = i % report.Length;
            report[idx] = (byte)i;
            Callbacks.Add(() => report[idx]);                                    // ...kept alive by the callback
        }
        foreach (var cb in Callbacks) checksum += cb();
        return checksum;
    }
}
