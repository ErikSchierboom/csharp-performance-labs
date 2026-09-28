using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PerfLab.Harness;
using PerfLab.Harness.Web;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace PagedCatalogue;

public class Product { public int Id { get; set; } public string Name { get; set; } = ""; public string Description { get; set; } = ""; }
public class CatalogContext(DbContextOptions<CatalogContext> o) : DbContext(o)
{
    public DbSet<Product> Products => Set<Product>(); 
}

public static class Db
{
    private static readonly string DbFile = Path.Combine(Path.GetTempPath(), "perflab-" + Environment.ProcessId + "-" + Guid.NewGuid().ToString("N") + ".db");
    private static readonly string ConnStr = "Data Source=" + DbFile;
    static readonly DbContextOptions<CatalogContext> Options;
    static Db()
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) => { SqliteConnection.ClearAllPools(); foreach (var f in Directory.GetFiles(System.IO.Path.GetDirectoryName(DbFile)!, System.IO.Path.GetFileName(DbFile) + "*")) File.Delete(f); }; Options = new DbContextOptionsBuilder<CatalogContext>().UseSqlite(ConnStr).Options;
        using var ctx = new CatalogContext(Options);
        ctx.Database.EnsureCreated();
        ctx.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
        
        var text = new string('x', 1_000);
        for (var i = 1; i <= 20_000; i++) ctx.Products.Add(new Product { Id = i, Name = "P" + i, Description = text });
        ctx.SaveChanges();
    }
    public static CatalogContext Create() => new(Options);
}

public static class Workload
{
    private static CatalogContext Shared = Db.Create();
    static readonly WebRig Rig = WebRig.Start(app =>
    {
        app.MapGet("/products/{n:int}", (int n) =>
        {
            lock (Shared)
            {
                var page = Shared.Products.Where(p => p.Id > n * 50 && p.Id <= n * 50 + 50).ToList();     // tracked, never released
                return page.Count.ToString();
            }
        });
    });

    public static void Reset() { Shared = Db.Create(); } // scaffolding

    public static long Run() { return Rig.Drive(users: 8, total: 400, i => "/products/" + i); }
}
