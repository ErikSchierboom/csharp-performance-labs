using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace PagedCatalogue;

public class Product { public int Id { get; set; } public string Name { get; set; } = ""; public string Description { get; set; } = ""; }
public class CatalogContext : DbContext { public CatalogContext(DbContextOptions<CatalogContext> o) : base(o) { } public DbSet<Product> Products => Set<Product>(); }
public static class Db
{
    static readonly string DbFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "perflab-" + Environment.ProcessId + "-" + Guid.NewGuid().ToString("N") + ".db");
    static readonly string ConnStr = "Data Source=" + DbFile;
    static readonly DbContextOptions<CatalogContext> Options;
    static Db()
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) => { SqliteConnection.ClearAllPools(); foreach (var f in Directory.GetFiles(System.IO.Path.GetDirectoryName(DbFile)!, System.IO.Path.GetFileName(DbFile) + "*")) File.Delete(f); }; Options = new DbContextOptionsBuilder<CatalogContext>().UseSqlite(ConnStr).Options;
        using var ctx = new CatalogContext(Options); ctx.Database.EnsureCreated(); ctx.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
        var text = new string('x', 1_000);
        for (int i = 1; i <= 20_000; i++) ctx.Products.Add(new Product { Id = i, Name = "P" + i, Description = text });
        ctx.SaveChanges();
    }
    public static CatalogContext Create() => new(Options);
}

public static class Workload
{
    static readonly WebRig Rig = WebRig.Start(app =>
    {
        app.MapGet("/products/{n:int}", async (int n) =>
        {
            using var ctx = Db.Create();                                     // a short-lived context per request (pooled by EF if you use AddDbContextPool)
            var page = await ctx.Products.AsNoTracking().Where(p => p.Id > n * 50 && p.Id <= n * 50 + 50).ToListAsync();
            return page.Count.ToString();
        });
    });

    public static void Reset() {  }   // scaffolding

    public static long Run() { return Rig.Drive(users: 8, total: 400, i => "/products/" + i); }
}
