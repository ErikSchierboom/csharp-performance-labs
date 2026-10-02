namespace FieldReader;

public class Item { public int Id { get; set; } public decimal Price { get; set; } public int Qty { get; set; } }

public static class Workload
{
    static readonly Item[] Items = Enumerable.Range(0, 2_000_000).Select(i => new Item { Id = i, Price = 1 + i % 50, Qty = i % 7 }).ToArray();

    static long Total(Item[] items)
    {
        decimal total = 0;
        foreach (var item in items)
        {
            total += item.Id + item.Price + item.Qty;
        }
        return (long)total;
    }

    public static long Run() => Total(Items);
}
