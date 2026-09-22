using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace OrderReport;

public class Customer { public int Id { get; set; } public string Name { get; set; } = ""; public List<Order> Orders { get; set; } = new(); }
public class Order { public int Id { get; set; } public int CustomerId { get; set; } public int Cents { get; set; } public string Notes { get; set; } = ""; }
public sealed class ShopContext : DbContext
{
    public ShopContext(DbContextOptions<ShopContext> o) : base(o) { }
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
}

public static class Workload
{
    static readonly SqliteConnection Conn = new("Data Source=:memory:");
    static readonly DbContextOptions<ShopContext> Options;
    static readonly ILogger Log = LoggerFactory.Create(b => b.SetMinimumLevel(LogLevel.Warning)).CreateLogger("report");   // production: warnings and above

    static Workload()
    {
        Conn.Open();
        Options = new DbContextOptionsBuilder<ShopContext>().UseSqlite(Conn).Options;
        using var ctx = new ShopContext(Options);
        ctx.Database.EnsureCreated();
        var notes = new string('n', 800);
        for (int c = 1; c <= 300; c++)
        {
            var cust = new Customer { Id = c, Name = "Customer " + c };
            for (int o = 0; o < 6; o++) cust.Orders.Add(new Order { CustomerId = c, Cents = 100 + (c * 7 + o * 13) % 900, Notes = notes });
            ctx.Customers.Add(cust);
        }
        ctx.SaveChanges();
    }

    public static long Run()
    {
        string audit = Path.Combine(Path.GetTempPath(), "perflab-audit-" + Environment.ProcessId + ".log");
        if (File.Exists(audit)) File.Delete(audit);
        long checksum = 0;
        using var ctx = new ShopContext(Options);
        var rows = ctx.Customers.AsNoTracking()
            .Select(c => new { c.Id, Total = c.Orders.Sum(o => o.Cents) })              // one query, only what we need
            .ToList();
        using (var audit_ = new StreamWriter(audit, false, new System.Text.UTF8Encoding(false), 64 * 1024))      // one open, buffered file
        {
            foreach (var r in rows)
            {
                checksum += r.Id * 7L + r.Total;
                audit_.Write($"{r.Id}|{r.Total}\n");
            }
        }
        checksum += new FileInfo(audit).Length;
        File.Delete(audit);
        return checksum;
    }
}
