using System.Threading.Channels;
using PerfLab.Harness;

namespace NotificationHub;

public sealed class Bus { public event Action<int>? Message; public void Send(int m) => Message?.Invoke(m); }

public sealed class Inbox
{
    readonly byte[] _buffer = new byte[8_000];
    public int Seen;
    public Inbox(Bus bus) => bus.Message += m => { Seen++; _buffer[m % _buffer.Length]++; };
}

public sealed class Stats
{
    readonly object _gate = new();
    readonly long[] _buckets = new long[64];
    public void Record(int v)
    {
        lock (_gate)
        {
            uint h = (uint)v * 2654435761u;
            for (int i = 0; i < 250; i++) h = (h ^ (h >> 13)) * 0x5bd1e995u + (uint)i;
            _buckets[(int)(h >> 9) & 63] += v;
        }
    }
    public long Total() { long t = 0; for (int i = 0; i < 64; i++) t += _buckets[i] * (i + 1); return t; }
}

public static class Workload
{
    static Bus _bus = new();
    public static void Reset() => _bus = new Bus();

    public static long Run()
    {
        long checksum = 0;

        // 1. Sessions come and go, each with an inbox on the shared bus.
        for (int i = 0; i < 600; i++)
        {
            var inbox = new Inbox(_bus);
            _bus.Send(i);
            checksum += inbox.Seen * 3 + i % 5;
        }

        // 2. Eight workers record delivery statistics.
        var stats = new Stats();
        Parallel.For(0, 8, new ParallelOptions { MaxDegreeOfParallelism = 8 }, w => { for (int i = 0; i < 25_000; i++) stats.Record(w * 1_000 + i % 997); });
        checksum += stats.Total();

        // 3. A burst of outgoing notifications is queued for a slower sender.
        var queue = Channel.CreateUnbounded<byte[]>();
        long sent = 0;
        var sender = Task.Run(async () =>
        {
            await foreach (var item in queue.Reader.ReadAllAsync())
            {
                uint h = 7; for (int k = 0; k < 8_000; k++) h = (h ^ (h >> 3)) * 2654435761u;
                sent += item[0] + (h & 1);
            }
        });
        for (int i = 0; i < 2_000; i++)
        {
            var n = new byte[5_000]; n[0] = (byte)(i % 100);
            queue.Writer.TryWrite(n);
            Lab.Report("maxQueued", queue.Reader.Count);
        }
        queue.Writer.Complete();
        sender.GetAwaiter().GetResult();
        return checksum + sent;
    }
}
