using System.Runtime.CompilerServices;

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

/// <summary>Attaches extra data to objects we don't own, without keeping them alive.</summary>
public static class Tags
{
    // Entries live exactly as long as their key object: no strong reference from the table to the key.
    static readonly ConditionalWeakTable<object, Metadata> Meta = new();

    public static void Reset() => Meta.Clear();          // test scaffolding only

    public static Metadata For(object owner) => Meta.GetOrCreateValue(owner);
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
