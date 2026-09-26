# Answers (measured)

> From a real `dotnet-gcdump` and `dotnet-dump analyze` of an earlier, unthrottled run of this service (about 10 s at ~24k requests/s); the shape is the same at the shipped rate of ~1,000 requests/s.

1. **Bytes:** `System.Byte[]` dominates (in my capture 1.9 GB across 237,101 arrays), but that is just the payloads. The *growing types* are **`Session`** (237,000 instances), its `List<string>` and `string[]`/`string` (`Trail`) and the `Dictionary<Guid, Session>` **entries array** (10 MB and growing). Counts of `Session`, `Byte[]` (8,000-byte ones) and the dictionary entries match: one leaked `Session` per request.
2. **Constant:** ~40 `Byte[]` of 500,000 bytes (a 20 MB catalogue, loaded once), and a queue of ~1,000 small `Audit` objects. Neither changes between the two dumps.
3. **Root:** static field **`Requests.ActiveSessions`**, a `Dictionary<Guid, Session>`: `gcroot` shows *static variable → Dictionary → Entry[] → Session → …*.
4. The catalogue is loaded once and never appended to; the audit queue is *bounded* (old items dequeued at 1,000). `ActiveSessions` only ever adds.
5. "Remove sessions from `ActiveSessions` when they end (expiry or logout), or bound it (LRU/TTL)." (This is L03-02's fix.)
6. A test or alert on **growth of live bytes after a full GC** across a steady workload (this harness's kept-after-GC gate; `dotMemory Unit` in a unit test; a production alert on `gc-heap-size` after gen2 trending up).

## Note on `dumpheap -stat` vs `dotnet-gcdump`
`dumpheap -stat` in the raw dump also counted 12,681 `Audit` objects (not the 1,000 live ones): the dump contains garbage not yet collected. `dotnet-gcdump` collects a GC first and shows only live objects. Know which you're reading.

## Reveal
Each request creates a `Session` (8 KB payload + a trail list) and stores it in a static dictionary forever.
