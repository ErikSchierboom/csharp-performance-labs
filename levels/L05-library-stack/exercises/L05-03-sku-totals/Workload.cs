using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace SkuTotals;

public class Line { public int Id { get; set; } public string Sku { get; set; } = ""; public int Cents { get; set; } }

public sealed class SalesContext(DbContextOptions<SalesContext> options) : DbContext(options)
{
    public DbSet<Line> Lines => Set<Line>();
}

public static class Db
{
    static readonly SqliteConnection Conn = new("Data Source=:memory:");
    static readonly DbContextOptions<SalesContext> Options;

    static Db()
    {
        Conn.Open();
        Options = new DbContextOptionsBuilder<SalesContext>().UseSqlite(Conn).Options;
        using (var ctx = new SalesContext(Options)) ctx.Database.EnsureCreated();
        using var tx = Conn.BeginTransaction();
        using var cmd = Conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = "INSERT INTO Lines (Sku, Cents) VALUES ($s, $c)";
        var s = cmd.Parameters.Add("$s", SqliteType.Text); var c = cmd.Parameters.Add("$c", SqliteType.Integer);
        var rng = new Random(4);
        for (int i = 0; i < 200_000; i++) { s.Value = "SKU-" + rng.Next(500); c.Value = rng.Next(100, 10_000); cmd.ExecuteNonQuery(); }
        tx.Commit();
    }

    public static SalesContext Create() => new(Options);
}

public static class Workload
{
    public static long Run()
    {
        using var ctx = Db.Create();
        long checksum = 0;
        for (int i = 0; i < 100; i++)
        {
            string sku = "SKU-" + (i * 5);
            checksum += ctx.Lines.AsNoTracking().Where(l => l.Sku == sku).Sum(l => l.Cents); // "total sold for one SKU"
        }
        return checksum;
    }
}
