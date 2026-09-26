using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PerfLab.Harness.Web;

namespace HelloEndpoint;

public static class Workload
{
    static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/hello", () => Results.Text("{\"message\":\"hello\"}", "application/json")));

    public static long Run() => Rig.Drive(users: 32, total: 4000, i => "/hello");
}
