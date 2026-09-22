using System.Threading.Channels;
using PerfLab.Harness;

namespace OrderPipeline;

public static class Workload
{
    public static long Run()
    {
        // Bounded channel: when 50 items are waiting, the producer *waits* (back-pressure) instead of piling up memory.
        var channel = Channel.CreateBounded<byte[]>(new BoundedChannelOptions(50) { FullMode = BoundedChannelFullMode.Wait, SingleReader = true, SingleWriter = true });
        long checksum = 0;

        var consumer = Task.Run(async () =>
        {
            await foreach (var item in channel.Reader.ReadAllAsync())
            {
                Process(item);
                checksum += item[0];
            }
        });

        Task.Run(async () =>
        {
            for (int i = 0; i < 3_000; i++)
            {
                var order = new byte[10_000];
                order[0] = (byte)(i % 251);
                await channel.Writer.WriteAsync(order);
                Lab.Report("maxQueued", channel.Reader.Count);
            }
            channel.Writer.Complete();
        }).GetAwaiter().GetResult();

        consumer.GetAwaiter().GetResult();
        return checksum;
    }

    static void Process(byte[] order)
    {
        uint h = 17;
        for (int i = 0; i < 40; i++) h = h * 31 + order[i];
        for (int k = 0; k < 8_000; k++) h = (h ^ (h >> 3)) * 2654435761u;
        order[1] = (byte)h;
    }
}
