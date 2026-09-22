using System.Net;
using System.Net.Sockets;
using PerfLab.Harness;

namespace HttpClientPerCall;

/// <summary>A tiny local HTTP server that also counts how many distinct TCP connections it saw.</summary>
public static class LocalServer
{
    static readonly HashSet<int> Ports = new();
    static readonly HttpListener Listener = new();
    public static string Url { get; }

    static LocalServer()
    {
        var probe = new TcpListener(IPAddress.Loopback, 0); probe.Start();
        int port = ((IPEndPoint)probe.LocalEndpoint).Port; probe.Stop();
        Url = $"http://127.0.0.1:{port}/";
        Listener.Prefixes.Add(Url);
        Listener.Start();
        new Thread(Loop) { IsBackground = true }.Start();
    }

    static async void Loop()
    {
        while (true)
        {
            var ctx = await Listener.GetContextAsync();
            lock (Ports) Ports.Add(ctx.Request.RemoteEndPoint.Port);
            var bytes = System.Text.Encoding.UTF8.GetBytes("pong");
            ctx.Response.ContentLength64 = bytes.Length;
            await ctx.Response.OutputStream.WriteAsync(bytes);
            ctx.Response.Close();
        }
    }

    public static int Connections { get { lock (Ports) return Ports.Count; } }
    public static void ResetCount() { lock (Ports) Ports.Clear(); }
}

public static class Workload
{
    // One long-lived client: its handler keeps connections alive and reuses them.
    static readonly HttpClient Shared = new();

    public static void Reset() => LocalServer.ResetCount();

    public static long Run()
    {
        long total = 0;
        for (int i = 0; i < 300; i++)
            total += Shared.GetStringAsync(LocalServer.Url).GetAwaiter().GetResult().Length;
        Lab.Report("connections", LocalServer.Connections);
        return total;
    }
}
