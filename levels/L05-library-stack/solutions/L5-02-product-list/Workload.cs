using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace WideProducts;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public int Cents { get; set; }
    public bool Active { get; set; }
    public string Description { get; set; } = "";       // long text: ~1 KB per row
}

public sealed class CatalogContext : DbContext
{
    public CatalogContext(DbContextOptions<CatalogContext> options) : base(options) { }
    public DbSet<Product> Products => Set<Product>();
}

public static class Db
{
    static readonly SqliteConnection Conn = new("Data Source=:memory:");
    static readonly DbContextOptions<CatalogContext> Options;

    static Db()
    {
        Conn.Open();
        Options = new DbContextOptionsBuilder<CatalogContext>().UseSqlite(Conn).Options;
        using var ctx = new CatalogContext(Options);
        ctx.Database.EnsureCreated();
        var rng = new Random(9);
        string[] cats = { "books", "toys", "tools", "games", "garden" };
        var text = new string('x', 1_000);
        for (int i = 1; i <= 10_000; i++)
            ctx.Products.Add(new Product { Id = i, Name = "Product " + i, Category = cats[i % cats.Length], Cents = rng.Next(100, 50_000), Active = rng.Next(4) != 0, Description = text });
        ctx.SaveChanges();
    }

    public static CatalogContext Create() => new(Options);
}

public static class Workload
{
    public static long Run()
    {
        long checksum = 0;
        foreach (var category in new[] { "books", "toys", "tools" })
        {
            using var ctx = Db.Create();
            var rows = ctx.Products.AsNoTracking()
                .Where(p => p.Category == category && p.Active)                  // filter in the database
                .Select(p => new { p.Name, p.Cents })                            // only the columns we use
                .ToList();
            foreach (var p in rows) checksum += p.Name.Length + p.Cents;
        }
        return checksum;
    }
}
