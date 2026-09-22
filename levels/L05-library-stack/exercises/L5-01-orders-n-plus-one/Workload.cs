using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PerfLab.Harness;

namespace OrdersNPlusOne;

public class Customer { public int Id { get; set; } public string Name { get; set; } = ""; public List<Order> Orders { get; set; } = new(); }
public class Order { public int Id { get; set; } public int CustomerId { get; set; } public int Cents { get; set; } }

public sealed class ShopContext : DbContext
{
    public ShopContext(DbContextOptions<ShopContext> options) : base(options) { }
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
}

public sealed class CommandCounter : DbCommandInterceptor
{
    public static int Count;
    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    { Interlocked.Increment(ref Count); return result; }
}

public static class Db
{
    static readonly SqliteConnection Conn = new("Data Source=:memory:");     // one shared in-memory database
    static readonly DbContextOptions<ShopContext> Options;

    static Db()
    {
        Conn.Open();
        Options = new DbContextOptionsBuilder<ShopContext>().UseSqlite(Conn).AddInterceptors(new CommandCounter()).Options;
        using var ctx = new ShopContext(Options);
        ctx.Database.EnsureCreated();
        var rng = new Random(3);
        for (int c = 1; c <= 500; c++)
        {
            var cust = new Customer { Id = c, Name = "Customer " + c };
            for (int o = 0; o < 4; o++) cust.Orders.Add(new Order { CustomerId = c, Cents = rng.Next(100, 20_000) });
            ctx.Customers.Add(cust);
        }
        ctx.SaveChanges();
    }

    public static ShopContext Create() => new(Options);
}

public static class Workload
{
    public static long Run()
    {
        CommandCounter.Count = 0;
        using var ctx = Db.Create();
        long checksum = 0;
        var customers = ctx.Customers.ToList();
        foreach (var c in customers)
        {
            var orders = ctx.Orders.Where(o => o.CustomerId == c.Id).ToList();     // one query per customer
            checksum += c.Id * 7L + orders.Sum(o => (long)o.Cents);
        }
        Lab.Report("sqlCommands", CommandCounter.Count);
        return checksum;
    }
}
