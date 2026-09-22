using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace OrdersService;

public class Customer { public int Id { get; set; } public string Name { get; set; } = ""; public List<Order> Orders { get; set; } = new(); public List<Address> Addresses { get; set; } = new(); }
public class Order { public int Id { get; set; } public int CustomerId { get; set; } public int Cents { get; set; } }
public class Address { public int Id { get; set; } public int CustomerId { get; set; } public string Line { get; set; } = ""; }
public class ShopContext : DbContext
{
    public ShopContext(DbContextOptions<ShopContext> o) : base(o) { }
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Address> Addresses => Set<Address>();
}
public sealed class CommandCounter : DbCommandInterceptor
{
    public static int Count;
    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand c, CommandEventData e, InterceptionResult<DbDataReader> r) { Interlocked.Increment(ref Count); return r; }
    public override InterceptionResult<int> NonQueryExecuting(DbCommand c, CommandEventData e, InterceptionResult<int> r) { Interlocked.Increment(ref Count); return r; }
    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand c, CommandEventData e, InterceptionResult<DbDataReader> r, CancellationToken ct = default) { Interlocked.Increment(ref Count); return new(r); }
    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand c, CommandEventData e, InterceptionResult<int> r, CancellationToken ct = default) { Interlocked.Increment(ref Count); return new(r); }
}
public static class Db
{
    static readonly string DbFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "perflab-" + Environment.ProcessId + "-" + Guid.NewGuid().ToString("N") + ".db");
    static readonly string ConnStr = "Data Source=" + DbFile;
    static readonly DbContextOptions<ShopContext> Options;
    static Db()
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) => { SqliteConnection.ClearAllPools(); foreach (var f in Directory.GetFiles(System.IO.Path.GetDirectoryName(DbFile)!, System.IO.Path.GetFileName(DbFile) + "*")) File.Delete(f); };
        Options = new DbContextOptionsBuilder<ShopContext>().UseSqlite(ConnStr).AddInterceptors(new CommandCounter()).Options;
        using var ctx = new ShopContext(Options);
        ctx.Database.EnsureCreated(); ctx.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
        for (int c = 1; c <= 400; c++)
        {
            var cust = new Customer { Id = c, Name = "Customer " + c };
            for (int o = 0; o < 20; o++) cust.Orders.Add(new Order { CustomerId = c, Cents = 100 + (c * 7 + o * 13) % 900 });
            for (int a = 0; a < 20; a++) cust.Addresses.Add(new Address { CustomerId = c, Line = "Street " + a });
            ctx.Customers.Add(cust);
        }
        ctx.SaveChanges();
    }
    public static ShopContext Create() => new(Options);
}

public sealed class ConnectionPool
{
    readonly SemaphoreSlim _slots = new(8);
    public async Task<Lease> RentAsync() { await _slots.WaitAsync(); return new Lease(_slots); }
    public sealed class Lease : IDisposable { readonly SemaphoreSlim _s; public Lease(SemaphoreSlim s) => _s = s; public void Dispose() => _s.Release(); }
}
public static class External { public static Task CallAsync() => Task.Delay(20); }        // a slow third-party (tax) service

public static class Workload
{
    static readonly WebRig Rig = WebRig.Start(app =>
    {
        var pool = new ConnectionPool();
        app.MapGet("/customer/{id:int}", async (int id) =>
        {
            using var ctx = Db.Create();
            await External.CallAsync();                                                         // slow non-database work first
            using var conn = await pool.RentAsync();                                            // hold a connection only for the queries
            var c = await ctx.Customers.AsNoTracking().Include(x => x.Orders).Include(x => x.Addresses).AsSplitQuery().SingleAsync(x => x.Id == id);
            long total = c.Orders.Sum(o => (long)o.Cents);                                      // already loaded: no extra queries
            return (total + c.Addresses.Count).ToString();
        });
    });

    public static void Reset() {  }   // scaffolding

    public static long Run() { CommandCounter.Count = 0; var r = Rig.Drive(users: 32, total: 400, i => "/customer/" + (1 + i % 400)); Lab.Report("sqlCommands", CommandCounter.Count); return r; }
}
