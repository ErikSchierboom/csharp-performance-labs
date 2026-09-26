# dotnet-gcdump

**Answers:** what is on the managed heap, by type, and what keeps it there? A gcdump holds the object *graph* (types, counts, sizes, references), not the object contents, so files stay small.

## Commands
```bash
dotnet-gcdump collect -n <id> -o a.gcdump              # take one snapshot
dotnet-gcdump report a.gcdump | head -30               # top types by size, in the terminal
```

To find a leak, take **two** snapshots some time apart, with the process doing the same work in between:
```bash
dotnet-gcdump collect -n <id> -o a.gcdump
# ...wait 20-30 seconds...
dotnet-gcdump collect -n <id> -o b.gcdump
```
Then open both in Visual Studio (**Compare to** another snapshot) or PerfView and look at what *grew*. Rider/dotMemory can import them too.

## Reading it
```
      1,397,379  GC Heap bytes
         15,849  GC Heap objects

   Object Bytes     Count  Type
        352,692         1  System.String (Bytes > 100K)
         40,024         1  InvoiceExport.Order[] (Bytes > 10K)
```
- **Size and count are both clues.** One huge object is a buffer or a big array. Millions of small ones is a collection that keeps growing.
- `System.String`, `Byte[]` and `Object[]` are nearly always at the top. Look for **your** types, and for the collections that hold them.
- The size column is each object's own size (shallow), not everything it references. A `Dictionary` that holds 80 MB can show up as a few KB. A viewer with retention paths (VS, PerfView, dotMemory) shows what holds what.

## Traps
- **Taking a gcdump runs a full, blocking GC.** That's fine in this lab. In production, it pauses the process for as long as the collection takes.
- A gcdump only shows **live** objects, because the GC has just run. If memory is high but the gcdump is small, the memory is garbage waiting to be collected, native memory, or free space inside the GC heap. That isn't a managed leak.
- It has no contents: you can see *that* there are 40,000 `Session` objects, but not their field values. For that, use [dotnet-dump](dotnet-dump.md).

## Docs
[dotnet-gcdump (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-gcdump)
