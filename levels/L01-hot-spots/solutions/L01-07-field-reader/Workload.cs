namespace FieldReader;

public class Item { public int Id { get; set; } public decimal Price { get; set; } public int Qty { get; set; } }

public static class Workload
{
    static readonly Item[] Items = Enumerable.Range(0, 2_000_000).Select(i => new Item { Id = i, Price = 1 + i % 50, Qty = i % 7 }).ToArray();

    // Look the column up ONCE, then read it through a fast typed getter (no reflection per item, no boxing).
    static Func<Item, decimal> Getter(string column) => column switch
    {
        "Price" => x => x.Price,
        "Qty" => x => x.Qty,
        "Id" => x => x.Id,
        _ => throw new ArgumentException(column),
    };

    static decimal Total(Item[] items, string column)
    {
        var get = Getter(column);
        decimal total = 0;
        foreach (var item in items) total += get(item);
        return total;
    }

    public static long Run() => (long)(Total(Items, "Price") + Total(Items, "Qty") + Total(Items, "Id"));
}
