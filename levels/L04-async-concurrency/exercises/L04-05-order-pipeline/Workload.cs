using System.Threading.Channels;
using PerfLab.Harness;

namespace OrderPipeline;

public static class Workload
{
    public static long Run()
    {
        // A fast producer (reads a burst of orders) and a slower consumer (processing each takes real work).
        var channel = Channel.CreateUnbounded<byte[]>();
        long checksum = 0;

        var consumer = Task.Run(async () =>
        {
            await foreach (var item in channel.Reader.ReadAllAsync())
            {
                Process(item);
                checksum += item[0];
            }
        });

        for (int i = 0; i < 30_000; i++)
        {
            var order = new byte[10_000];
            order[0] = (byte)(i % 251);
            channel.Writer.TryWrite(order);
            Lab.Report("maxQueued", channel.Reader.Count);
            Lab.Report("peakHeapMb", GC.GetTotalMemory(false) / 1_048_576.0);
        }
        channel.Writer.Complete();
        consumer.GetAwaiter().GetResult();
        return checksum;
    }

    static void Process(byte[] order)
    {
        uint h = 17;
        for (int i = 0; i < 40; i++) h = h * 31 + order[i];        // ~20-30 microseconds of "work"
        for (int k = 0; k < 8_000; k++) h = (h ^ (h >> 3)) * 2654435761u;
        order[1] = (byte)h;
    }
}
