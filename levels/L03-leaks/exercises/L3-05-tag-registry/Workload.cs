namespace TagRegistry;

public sealed class Document
{
    public readonly int Id;
    public readonly byte[] Body = new byte[4_096];
    public Document(int id) => Id = id;
}

public sealed class Metadata
{
    public int Views;
    public string Label = "";
}

/// <summary>Attaches extra data to objects we don't own (so we can't add a field to them).</summary>
public static class Tags
{
    static readonly Dictionary<object, Metadata> Meta = new();

    public static void Reset() => Meta.Clear();          // test scaffolding only

    public static Metadata For(object owner)
    {
        if (!Meta.TryGetValue(owner, out var m)) Meta[owner] = m = new Metadata();
        return m;
    }
}

public static class Workload
{
    public static void Reset() => Tags.Reset();

    public static long Run()
    {
        long checksum = 0;
        for (int i = 0; i < 20_000; i++)
        {
            var doc = new Document(i);
            var m = Tags.For(doc);
            m.Views += i % 5;
            m.Label = "doc-" + i;
            checksum += m.Views * 31 + m.Label.Length;
        }
        return checksum;
    }
}
