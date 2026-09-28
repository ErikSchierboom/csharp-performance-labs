using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PerfLab.Harness;
using PerfLab.Harness.Web;

namespace RequestLogging;

public sealed class SyncFileLoggerProvider(string path) : ILoggerProvider
{
    readonly string _path = path; readonly object _gate = new();
    public ILogger CreateLogger(string category) => new L(this, category);
    public void Dispose() { }

    private sealed class L(SyncFileLoggerProvider p, string c) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel l) => l >= LogLevel.Information;
        public void Log<TState>(LogLevel l, EventId id, TState s, Exception? e, Func<TState, Exception?, string> f)
        {
            lock (p._gate) File.AppendAllText(p._path, $"{DateTime.UtcNow:O} {l} {c} {f(s, e)}\n");         // open, write, close, under a global lock
        }
    }
}

public static class Workload
{
    private static readonly string LogPath = Path.Combine(Path.GetTempPath(), "perflab-log-" + Environment.ProcessId + ".log");
    static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/work/{id:int}", (int id, ILoggerFactory lf) =>
    {
        var log = lf.CreateLogger("work");
        log.LogInformation("start {Id}", id);
        log.LogInformation("validated {Id}", id);
        log.LogInformation("computed {Id}", id);
        log.LogInformation("done {Id}", id);
        return (id * 2).ToString();
    }),
    b => b.Logging.AddProvider(new SyncFileLoggerProvider(LogPath)));

    public static long Run() { return Rig.Drive(users: 32, total: 1500, i => "/work/" + i); }
}
