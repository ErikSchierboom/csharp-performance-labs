using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace OrderReport;

public class Customer { public int Id { get; set; } public string Name { get; set; } = ""; public List<Order> Orders { get; set; } = new(); }
public class Order { public int Id { get; set; } public int CustomerId { get; set; } public int Cents { get; set; } public string Notes { get; set; } = ""; }
public sealed class ShopContext(DbContextOptions<ShopContext> o) : DbContext(o)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
}

public static class Workload
{
    private static readonly SqliteConnection Conn = new("Data Source=:memory:");
    private static readonly DbContextOptions<ShopContext> Options;
    private static readonly ILogger Log = LoggerFactory.Create(b => b.SetMinimumLevel(LogLevel.Warning)).CreateLogger("report");   // production: warnings and above

    static Workload()
    {
        Conn.Open();
        Options = new DbContextOptionsBuilder<ShopContext>().UseSqlite(Conn).Options;
        using var ctx = new ShopContext(Options);
        ctx.Database.EnsureCreated();
        var notes = new string('n', 800);
        for (var c = 1; c <= 300; c++)
        {
            var cust = new Customer { Id = c, Name = "Customer " + c };
            for (var o = 0; o < 6; o++) cust.Orders.Add(new Order { CustomerId = c, Cents = 100 + (c * 7 + o * 13) % 900, Notes = notes });
            ctx.Customers.Add(cust);
        }
        ctx.SaveChanges();
    }

    public static long Run()
    {
        var audit = Path.Combine(Path.GetTempPath(), "perflab-audit-" + Environment.ProcessId + ".log");
        if (File.Exists(audit)) File.Delete(audit);
        long checksum = 0;
        using var ctx = new ShopContext(Options);
        var customers = ctx.Customers.ToList();
        foreach (var c in customers)
        {
            var orders = ctx.Orders.Where(o => o.CustomerId == c.Id).ToList();
            long total = 0;
            foreach (var o in orders)
            {
                Log.LogDebug($"Order {o.Id} for {c.Name}: {o.Cents} cents");
                total += o.Cents;
            }
            checksum += c.Id * 7L + total;
            File.AppendAllText(audit, $"{c.Id}|{total}\n");
        }
        checksum += new FileInfo(audit).Length;
        File.Delete(audit);
        return checksum;
    }
}
