using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PerfLab.Harness.Web;

using Microsoft.AspNetCore.Mvc;

namespace HelloTax;
[ApiController]
public sealed class HelloController : ControllerBase
{
    [HttpGet("/hello")]
    public IActionResult Get() => Ok(new { message = "hello" });
}

public static class Workload
{
    static readonly WebRig Rig = WebRig.Start(app => app.MapControllers(), b => b.Services.AddControllers());

    public static void Reset() { }

    public static long Run() => Rig.Drive(users: 32, total: 4000, i => "/hello");
}
