using System.Diagnostics;
using System.Runtime.InteropServices;
using PerfLab.Harness;

namespace PhantomLeak;

/// <summary>A scratch buffer in unmanaged memory (think: a native image or crypto library handle).</summary>
public sealed class NativeBuffer
{
    private readonly IntPtr _ptr;
    private int Length { get; }

    public NativeBuffer(int length)
    {
        Length = length;
        _ptr = Marshal.AllocHGlobal(length);
        for (var i = 0; i < length; i += 4096) Marshal.WriteByte(_ptr, i, 0);
    }

    public void Fill(int seed) { for (int i = 0; i < Length; i += 4096) Marshal.WriteByte(_ptr, i, (byte)(seed + i)); }
    public long Sample() { long s = 0; for (int i = 0; i < Length; i += 4096) s += Marshal.ReadByte(_ptr, i); return s; }
}

public static class Workload
{
    private static long PrivateBytes() { 
        using var p = Process.GetCurrentProcess(); 
        return p.PrivateMemorySize64;
    }

    public static long Run()
    {
        var before = PrivateBytes();
        long checksum = 0;
        for (var i = 0; i < 150; i++)
        {
            var buffer = new NativeBuffer(1 << 20);
            buffer.Fill(i);
            checksum += buffer.Sample();
        }
        Lab.Report("privateMB", Math.Max(0, PrivateBytes() - before) / 1024f / 1024f);
        return checksum;
    }
}
