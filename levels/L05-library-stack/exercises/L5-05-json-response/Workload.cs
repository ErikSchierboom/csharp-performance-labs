using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonResponse;

public record Order(int Id, string Customer, string Sku, int Quantity, double UnitPrice, string[] Tags);

public static class Workload
{
    static readonly List<Order> Orders = Create(5_000);       // input: built once, not measured

    public static long Run()
    {
        byte[] payload;
        // Serialise each order to a string, join them into an array, then encode the whole thing to bytes.
        var parts = new List<string>();
        foreach (var o in Orders) parts.Add(JsonSerializer.Serialize(o));
        string json = "[" + string.Join(",", parts) + "]";
        payload = Encoding.UTF8.GetBytes(json);
        long h = payload.Length;
        for (int i = 0; i < payload.Length; i += 97) h = h * 31 + payload[i];
        return h;
    }

    static List<Order> Create(int n)
    {
        var rng = new Random(8);
        var list = new List<Order>(n);
        for (int i = 0; i < n; i++)
            list.Add(new Order(i, "Customer " + rng.Next(900), "SKU-" + rng.Next(300), rng.Next(1, 40), rng.Next(100, 20_000) / 100.0, new[] { "a", "b" + i % 7 }));
        return list;
    }
}
