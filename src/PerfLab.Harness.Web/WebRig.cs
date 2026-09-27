using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PerfLab.Harness.Web;

/// <summary>
/// An in-process ASP.NET Core server on loopback plus a load driver, for the ASP.NET Core levels (Levels 9–14).
/// Each exercise's workload starts a rig once (like other exercises' input data), and each <c>Run()</c> drives a
/// burst of requests at it. Latencies are fed to <see cref="Lab.RecordLatency"/>, so p99 budgets work.
///
/// Honest limits: server and client share one process and one machine. Allocation and CPU figures therefore include
/// the (small, constant) client cost, and the load generator competes with the server for cores. Budgets are derived
/// on that basis. Pin the process (<c>taskset -c 0-7 dotnet ...</c>) if you want less noise.
/// </summary>
public sealed class WebRig : IDisposable
{
    readonly WebApplication _app;

    /// <summary>The client used to drive requests at the rig. Reuses pooled connections across a whole run.</summary>
    public HttpClient Client { get; }

    /// <summary>The loopback base URL the server is actually listening on (a randomly assigned free port).</summary>
    public string BaseUrl { get; }

    WebRig(WebApplication app, HttpClient client, string url) { _app = app; Client = client; BaseUrl = url; }

    /// <summary>
    /// Starts an in-process Kestrel server on a random loopback port and returns a rig ready to drive requests at
    /// it. Call once per exercise (like other exercises' input data); each <c>Run()</c> should reuse the same rig.
    /// </summary>
    /// <param name="map">Configures the app's endpoints/middleware, e.g. <c>app.MapGet(...)</c>.</param>
    /// <param name="configure">Optional hook to configure the <see cref="WebApplicationBuilder"/> before it's built (services, DI lifetimes, etc.).</param>
    /// <returns>A started <see cref="WebRig"/>; dispose it to stop the server.</returns>
    public static WebRig Start(Action<WebApplication> map, Action<WebApplicationBuilder>? configure = null)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Logging.ClearProviders();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.WebHost.ConfigureKestrel(o => o.AddServerHeader = false);
        configure?.Invoke(builder);
        var app = builder.Build();
        map(app);
        app.StartAsync().GetAwaiter().GetResult();
        string url = app.Urls.First();
        var handler = new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(10) };
        var client = new HttpClient(handler) { BaseAddress = new Uri(url), Timeout = TimeSpan.FromSeconds(120) };
        return new WebRig(app, client, url);
    }

    /// <summary>Closed loop: <paramref name="users"/> virtual users, each sending its next request when the previous one finishes.</summary>
    /// <param name="users">Number of concurrent virtual users.</param>
    /// <param name="total">Total number of requests to send across all users.</param>
    /// <param name="path">Builds the request path from the request index.</param>
    /// <param name="method">HTTP method to use; defaults to GET.</param>
    /// <param name="content">Optional per-request body content, built from the request index.</param>
    /// <returns>A checksum over every response's status code and body length, so a wrong or short response fails the exercise.</returns>
    public long Drive(int users, int total, Func<int, string> path, HttpMethod? method = null, Func<int, HttpContent>? content = null)
    {
        long checksum = 0; int errors = 0, next = 0;
        var tasks = Enumerable.Range(0, users).Select(_ => Task.Run(async () =>
        {
            int i;
            while ((i = Interlocked.Increment(ref next) - 1) < total)
            {
                long t0 = Stopwatch.GetTimestamp();
                try
                {
                    using var req = new HttpRequestMessage(method ?? HttpMethod.Get, path(i));
                    if (content != null) req.Content = content(i);
                    using var res = await Client.SendAsync(req);
                    var body = await res.Content.ReadAsStringAsync();
                    Lab.RecordLatency(Stopwatch.GetElapsedTime(t0).TotalMilliseconds);
                    if ((int)res.StatusCode >= 500) Interlocked.Increment(ref errors);
                    Interlocked.Add(ref checksum, (int)res.StatusCode * 1_000_003L + body.Length);
                }
                catch { Interlocked.Increment(ref errors); Lab.RecordLatency(Stopwatch.GetElapsedTime(t0).TotalMilliseconds); }
            }
        })).ToArray();
        Task.WaitAll(tasks);
        Lab.Report("errors", errors);
        return checksum;
    }

    /// <summary>
    /// Like <see cref="Drive"/>, but every request is <b>abandoned</b> by the client after <paramref name="abandonAfterMs"/> ms
    /// (as a browser tab closing or a client timeout would). Abandoned requests are not errors; the checksum counts them.
    /// </summary>
    /// <param name="users">Number of concurrent virtual users.</param>
    /// <param name="total">Total number of requests to send across all users.</param>
    /// <param name="path">Builds the request path from the request index.</param>
    /// <param name="abandonAfterMs">How long to wait before abandoning (cancelling) each request, in milliseconds.</param>
    /// <returns>A checksum over every request's outcome (status code, or a sentinel for a cancelled/failed request).</returns>
    public long DriveAbandon(int users, int total, Func<int, string> path, int abandonAfterMs)
        => DriveAbandon(users, total, path, _ => abandonAfterMs);

    /// <summary>
    /// A mix of impatient and patient clients. Request <c>i</c> is abandoned after <c>abandonAfterMs(i)</c> ms, or waits
    /// for the response when that is 0 or less. The latency of every request that <i>completes</i> is fed to
    /// <see cref="Lab.RecordLatency"/>, so a p99 budget measures what the patient clients experienced; abandoned
    /// requests are not recorded, since the client stopped waiting.
    /// </summary>
    /// <param name="users">Number of concurrent virtual users.</param>
    /// <param name="total">Total number of requests to send across all users.</param>
    /// <param name="path">Builds the request path from the request index.</param>
    /// <param name="abandonAfterMs">Builds each request's patience in milliseconds from its index; 0 or less means wait for the response.</param>
    /// <returns>A checksum over every request's outcome (status code, or a sentinel for a cancelled/failed request).</returns>
    public long DriveAbandon(int users, int total, Func<int, string> path, Func<int, int> abandonAfterMs)
    {
        long checksum = 0; int next = 0;
        var tasks = Enumerable.Range(0, users).Select(_ => Task.Run(async () =>
        {
            int i;
            while ((i = Interlocked.Increment(ref next) - 1) < total)
            {
                int patience = abandonAfterMs(i);
                using var cts = patience > 0 ? new CancellationTokenSource(patience) : new CancellationTokenSource();
                long t0 = Stopwatch.GetTimestamp();
                long code;
                try
                {
                    using var res = await Client.GetAsync(path(i), cts.Token);
                    await res.Content.ReadAsStringAsync(cts.Token);
                    Lab.RecordLatency(Stopwatch.GetElapsedTime(t0).TotalMilliseconds);
                    code = (int)res.StatusCode;
                }
                catch (OperationCanceledException) { code = -1; }
                catch { code = -2; }
                Interlocked.Add(ref checksum, code * 1_000_003L + 7);
            }
        })).ToArray();
        Task.WaitAll(tasks);
        return checksum;
    }

    /// <summary>
    /// Open loop: requests are <i>scheduled</i> at <paramref name="rps"/> whether or not earlier ones have finished, and
    /// latency is measured from the scheduled time, so a slow server can't hide queueing by slowing the client down
    /// (coordinated omission).
    /// </summary>
    /// <param name="rps">Target requests per second.</param>
    /// <param name="total">Total number of requests to schedule.</param>
    /// <param name="path">Builds the request path from the request index.</param>
    /// <returns>A checksum over every response's status code and body length.</returns>
    public long DriveOpen(double rps, int total, Func<int, string> path)
    {
        long checksum = 0; int errors = 0;
        long start = Stopwatch.GetTimestamp();
        var inflight = new List<Task>(total);
        for (int i = 0; i < total; i++)
        {
            long due = start + (long)(i / rps * Stopwatch.Frequency);
            while (Stopwatch.GetTimestamp() < due) Thread.Sleep(0);
            int id = i;
            inflight.Add(Task.Run(async () =>
            {
                try
                {
                    using var res = await Client.GetAsync(path(id));
                    var body = await res.Content.ReadAsStringAsync();
                    Lab.RecordLatency((Stopwatch.GetTimestamp() - due) * 1000.0 / Stopwatch.Frequency);
                    if ((int)res.StatusCode >= 500) Interlocked.Increment(ref errors);
                    Interlocked.Add(ref checksum, (int)res.StatusCode * 1_000_003L + body.Length);
                }
                catch { Interlocked.Increment(ref errors); }
            }));
        }
        Task.WaitAll(inflight.ToArray());
        Lab.Report("errors", errors);
        return checksum;
    }

    /// <summary>Disposes <see cref="Client"/> and stops the server.</summary>
    public void Dispose() { Client.Dispose(); _app.StopAsync().GetAwaiter().GetResult(); }
}

