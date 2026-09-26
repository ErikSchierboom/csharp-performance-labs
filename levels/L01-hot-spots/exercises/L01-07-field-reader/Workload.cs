using System.Reflection;

namespace FieldReader;

public class Item { public int Id { get; set; } public decimal Price { get; set; } public int Qty { get; set; } }

public static class Workload
{
    static readonly Item[] Items = Enumerable.Range(0, 2_000_000).Select(i => new Item { Id = i, Price = 1 + i % 50, Qty = i % 7 }).ToArray();

    // A "configurable report": the column to total is chosen by name at run time.
    static decimal Total(Item[] items, string column)
    {
        decimal total = 0;
        foreach (var item in items)
        {
            var prop = typeof(Item).GetProperty(column)!; // look up the property
            total += Convert.ToDecimal(prop.GetValue(item)); // ...and read it through reflection
        }
        return total;
    }

    public static long Run() => (long)(Total(Items, "Price") + Total(Items, "Qty") + Total(Items, "Id"));
}
