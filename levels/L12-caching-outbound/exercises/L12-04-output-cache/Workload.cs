using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;

namespace OutputCaching;

public static class Workload
{
    static readonly WebRig Rig = WebRig.Start(app =>
    {
        app.MapGet("/report/{n:int}", async (int n) =>
        {
            await Task.Delay(8);                                    // an expensive report: 8 ms
            return "report-" + n;
        });
    });

    public static void Reset() {  }   // scaffolding

    public static long Run() { return Rig.Drive(users: 32, total: 1200, i => "/report/" + i % 20); }
}
