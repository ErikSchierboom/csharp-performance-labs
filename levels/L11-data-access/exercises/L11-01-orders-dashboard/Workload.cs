using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PerfLab.Harness.Data;

namespace OrdersDashboard;

public class Customer { public int Id { get; set; } public string Name { get; set; } = ""; public List<Order> Orders { get; set; } = new(); public List<Address> Addresses { get; set; } = new(); }
public class Order { public int Id { get; set; } public int CustomerId { get; set; } public int Cents { get; set; } }
public class Address { public int Id { get; set; } public int CustomerId { get; set; } public string Line { get; set; } = ""; }
public class ShopContext(DbContextOptions<ShopContext> o) : DbContext(o)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Address> Addresses => Set<Address>();
}
public static class Db
{
    public static readonly string DbFile = Path.Combine(Path.GetTempPath(), "perflab-" + Environment.ProcessId + "-" + Guid.NewGuid().ToString("N") + ".db");
    public static readonly string ConnStr = "Data Source=" + DbFile;
    private static readonly DbContextOptions<ShopContext> Options;
    static Db()
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) => { SqliteConnection.ClearAllPools(); foreach (var f in Directory.GetFiles(System.IO.Path.GetDirectoryName(DbFile)!, System.IO.Path.GetFileName(DbFile) + "*")) File.Delete(f); };
        Options = new DbContextOptionsBuilder<ShopContext>().UseSqlite(ConnStr).AddInterceptors(new CommandCounter()).Options;
        using var ctx = new ShopContext(Options);
        ctx.Database.EnsureCreated(); ctx.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
        for (var c = 1; c <= 400; c++)
        {
            var cust = new Customer { Id = c, Name = "Customer " + c };
            for (var o = 0; o < 20; o++) cust.Orders.Add(new Order { CustomerId = c, Cents = 100 + (c * 7 + o * 13) % 900 });
            for (var a = 0; a < 20; a++) cust.Addresses.Add(new Address { CustomerId = c, Line = "Street " + a });
            ctx.Customers.Add(cust);
        }
        ctx.SaveChanges();
    }
    public static ShopContext Create() => new(Options);
}

public static class Workload
{
    static readonly WebRig Rig = WebRig.Start(app => app.MapGet("/totals/{start:int}", async (int start) =>
    {
        await using var ctx = Db.Create();
        long total = 0;
        var customers = await ctx.Customers.Where(c => c.Id >= start && c.Id < start + 20).ToListAsync();
        foreach (var c in customers)
            total += await ctx.Orders.Where(o => o.CustomerId == c.Id).SumAsync(o => o.Cents);
        return total.ToString();
    }));

    public static long Run() { CommandCounter.Reset(); var r = Rig.Drive(users: 16, total: 400, i => "/totals/" + (1 + i % 380)); Lab.Report(DbMetrics.CommandsPerRequest, CommandCounter.Total); return r; }
}
