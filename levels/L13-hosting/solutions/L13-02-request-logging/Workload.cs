using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PerfLab.Harness;
using PerfLab.Harness.Web;

namespace RequestLogging;

public sealed class BufferedFileLoggerProvider : ILoggerProvider
{
    readonly System.Threading.Channels.Channel<string> _q = System.Threading.Channels.Channel.CreateBounded<string>(new System.Threading.Channels.BoundedChannelOptions(10_000) { FullMode = System.Threading.Channels.BoundedChannelFullMode.DropWrite });
    readonly Task _writer;
    public BufferedFileLoggerProvider(string path)
    {
        _writer = Task.Factory.StartNew(async () =>
        {
            await using var w = new StreamWriter(path, false, new System.Text.UTF8Encoding(false), 64 * 1024);           // one open file, big buffer
            await foreach (var line in _q.Reader.ReadAllAsync()) w.WriteLine(line);
        }, TaskCreationOptions.LongRunning).Unwrap();
    }
    public ILogger CreateLogger(string category) => new L(this, category);
    public void Dispose() { _q.Writer.TryComplete(); _writer.Wait(TimeSpan.FromSeconds(5)); }
    sealed class L : ILogger
    {
        readonly BufferedFileLoggerProvider _p; readonly string _c;
        public L(BufferedFileLoggerProvider p, string c) { _p = p; _c = c; }
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel l) => l >= LogLevel.Information;
        public void Log<TState>(LogLevel l, EventId id, TState s, Exception? e, Func<TState, Exception?, string> f)
            => _p._q.Writer.TryWrite($"{DateTime.UtcNow:O} {l} {_c} {f(s, e)}");                                    // enqueue and return: the request never waits on the disk
    }
}

public static class Workload
{
    internal static readonly string LogPath = Path.Combine(Path.GetTempPath(), "perflab-log-" + Environment.ProcessId + ".log");
    static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/work/{id:int}", (int id, ILoggerFactory lf) =>
    {
        var log = lf.CreateLogger("work");
        log.LogInformation("start {Id}", id);
        log.LogInformation("validated {Id}", id);
        log.LogInformation("computed {Id}", id);
        log.LogInformation("done {Id}", id);
        return (id * 2).ToString();
    }),
    b => b.Logging.AddProvider(new BufferedFileLoggerProvider(Workload.LogPath)));

    public static void Reset() {  }   // scaffolding

    public static long Run() { return Rig.Drive(users: 32, total: 1500, i => "/work/" + i); }
}
