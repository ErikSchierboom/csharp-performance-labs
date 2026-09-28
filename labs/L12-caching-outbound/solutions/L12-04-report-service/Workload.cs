using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;

namespace ReportService;

public static class Workload
{
    private static readonly WebRig Rig = WebRig.Start(app =>
    {
        app.UseOutputCache();
        app.MapGet("/report/{n:int}", async (int n) =>
        {
            await Task.Delay(10); // an expensive report
            return "report-" + n;
        }).CacheOutput();
    },
    b => b.Services.AddOutputCache());

    public static long Run() { return Rig.Drive(users: 32, total: 1200, i => "/report/" + i % 20); }
}
