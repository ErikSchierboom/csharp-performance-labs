using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonResponse;

public enum OrderStatus { Pending, Paid, Shipped, Cancelled }
public record OrderLine(string Sku, int Quantity, decimal UnitPrice);
public record Order(int Id, string Customer, OrderStatus Status, DateTime PlacedAt, string? Note, List<OrderLine> Lines);
public record OrdersPage(int Page, int PageSize, int Total, List<Order> Items);

/// <summary>The <c>GET /orders?page=N</c> handler: writes one page of orders to the response body as JSON.</summary>
public static class OrdersEndpoint
{
    public const int PageSize = 50;

    public static void Handle(int page, IReadOnlyList<Order> orders, Stream responseBody)
    {
        var items = orders.Skip((page - 1) * PageSize).Take(PageSize).ToList();
        var dto = new OrdersPage(page, PageSize, orders.Count, items);

        // Straight to UTF-8 in the response body, with metadata built once (at compile time).
        JsonSerializer.Serialize(responseBody, dto, ApiJson.Default.OrdersPage);
    }
}

// The API's JSON conventions, declared once: camelCase, enums as strings, no nulls.
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    UseStringEnumConverter = true)]
[JsonSerializable(typeof(OrdersPage))]
public partial class ApiJson : JsonSerializerContext { }

public static class Workload
{
    private static readonly List<Order> Orders = Create(5_000); // input: built once, not measured

    // Every page of the catalogue, one request each (100 requests).
    public static long Run()
    {
        var body = new ChecksumStream();
        for (var page = 1; (page - 1) * OrdersEndpoint.PageSize < Orders.Count; page++)
            OrdersEndpoint.Handle(page, Orders, body);
        return body.Checksum;
    }

    private static List<Order> Create(int n)
    {
        var rng = new Random(8);
        var list = new List<Order>(n);
        for (var i = 0; i < n; i++)
        {
            var lines = new List<OrderLine>();
            for (int l = 0, count = rng.Next(1, 4); l < count; l++)
                lines.Add(new OrderLine("SKU-" + rng.Next(300), rng.Next(1, 40), rng.Next(100, 20_000) / 100m));
            list.Add(new Order(i, "Customer " + rng.Next(900), (OrderStatus)rng.Next(4),
                new DateTime(2026, 1, 1).AddMinutes(rng.Next(500_000)), rng.Next(10) == 0 ? "Leave at door" : null, lines));
        }
        return list;
    }
}

/// <summary>Stands in for the network: a write-only response body that fingerprints the bytes it receives.</summary>
public sealed class ChecksumStream : Stream
{
    private long _hash, _position;

    public long Checksum => _hash * 31 + _position;

    public override void Write(byte[] buffer, int offset, int count) => Write(buffer.AsSpan(offset, count));

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        foreach (var b in buffer)
            if (_position++ % 61 == 0) _hash = _hash * 31 + b;
    }

    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => _position;
    public override long Position { get => _position; set => throw new NotSupportedException(); }
    public override void Flush() { }
    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
}
