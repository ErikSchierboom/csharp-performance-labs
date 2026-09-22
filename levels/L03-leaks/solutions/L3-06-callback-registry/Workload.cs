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
            var report = new byte[20_000];
            int idx = id % report.Length;
            report[idx] = (byte)id;
            int value = report[idx];                                   // capture only the small value the callback needs
            Registry.Register(() => value);                            // the 20 KB array is now garbage after this iteration
        }
        return Registry.Fire();
    }
}
