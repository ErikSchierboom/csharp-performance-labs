namespace CallbackRegistry;

public static class Registry
{
    static readonly List<Func<int>> Callbacks = new();
    public static void Register(Func<int> cb) => Callbacks.Add(cb);
    public static void Reset() => Callbacks.Clear();                  // test scaffolding only
    public static int Fire() { int s = 0; foreach (var cb in Callbacks) s += cb(); return s; }
}

public static class Workload
{
    public static void Reset() => Registry.Reset();

    public static long Run()
    {
        for (int id = 0; id < 3_000; id++)
        {
            var report = new byte[20_000];                             // a big local used while building the callback...
            int idx = id % report.Length;
            report[idx] = (byte)id;
            Registry.Register(() => report[idx]);                       // ...and the callback keeps `report` alive
        }
        return Registry.Fire();
    }
}
