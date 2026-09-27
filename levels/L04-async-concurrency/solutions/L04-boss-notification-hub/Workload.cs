using System.Threading.Channels;
using PerfLab.Harness;

namespace NotificationHub;

public sealed class Bus { public event Action<int>? Message; public void Send(int m) => Message?.Invoke(m); }

public sealed class Inbox : IDisposable
{
    readonly Bus _bus;
    readonly Action<int> _handler;
    readonly byte[] _buffer = new byte[8_000];
    public int Seen;
    public Inbox(Bus bus) { _bus = bus; _handler = m => { Seen++; _buffer[m % _buffer.Length]++; }; bus.Message += _handler; }
    public void Dispose() => _bus.Message -= _handler; // unsubscribe: L03-01
}

public sealed class Stats
{
    readonly long[] _buckets = new long[64];
    public void Record(int v)
    {
        uint h = (uint)v * 2654435761u; // pure work outside any lock: L04-02
        for (int i = 0; i < 250; i++) h = (h ^ (h >> 13)) * 0x5bd1e995u + (uint)i;
        Interlocked.Add(ref _buckets[(int)(h >> 9) & 63], v);
    }
    public long Total() { long t = 0; for (int i = 0; i < 64; i++) t += Volatile.Read(ref _buckets[i]) * (i + 1); return t; }
}

public static class Workload
{
    static Bus _bus = new();
    public static void Reset() => _bus = new Bus();

    public static long Run()
    {
        long checksum = 0;

        for (int i = 0; i < 600; i++)
        {
            using var inbox = new Inbox(_bus);
            _bus.Send(i);
            checksum += inbox.Seen * 3 + i % 5;
        }

        var stats = new Stats();
        Parallel.For(0, 8, new ParallelOptions { MaxDegreeOfParallelism = 8 }, w => { for (int i = 0; i < 25_000; i++) stats.Record(w * 1_000 + i % 997); });
        checksum += stats.Total();

        // Bounded queue: the producer waits when the sender falls behind (L04-05).
        var queue = Channel.CreateBounded<byte[]>(new BoundedChannelOptions(50) { FullMode = BoundedChannelFullMode.Wait, SingleReader = true, SingleWriter = true });
        long sent = 0;
        var sender = Task.Run(async () =>
        {
            await foreach (var item in queue.Reader.ReadAllAsync())
            {
                uint h = 7; for (int k = 0; k < 8_000; k++) h = (h ^ (h >> 3)) * 2654435761u;
                sent += item[0] + (h & 1);
            }
        });
        Task.Run(async () =>
        {
            for (int i = 0; i < 2_000; i++)
            {
                var n = new byte[5_000]; n[0] = (byte)(i % 100);
                await queue.Writer.WriteAsync(n);
                Lab.Report(Metrics.MaxQueued, queue.Reader.Count);
            }
            queue.Writer.Complete();
        }).GetAwaiter().GetResult();
        sender.GetAwaiter().GetResult();
        return checksum + sent;
    }
}
