# L8-03 · Who holds the memory?

The service's memory grows steadily (about 8 MB per second). **Find which type is growing and what keeps it alive, without reading the source.**
(Stop it before it eats your RAM: it runs 120 s by default, about 1 GB.)

```powershell
./levels/L08-production/L8-03-who-holds-memory/run.ps1 120         # terminal 1; note the pid
dotnet-counters monitor -p <pid> System.Runtime  # is the heap growing? which generation?
dotnet-gcdump collect -p <pid> -o a.gcdump       # take one at ~10 s ...
dotnet-gcdump collect -p <pid> -o b.gcdump       # ... and one at ~40 s
dotnet-gcdump report a.gcdump | head -30         # top types by size / count
dotnet-dump collect -p <pid> -o core.dmp
dotnet-dump analyze core.dmp
> dumpheap -stat          # types by total size and count
> dumpheap -type Session -min 8000   # then take an address ...
> gcroot <address>        # ... and find what roots it
```
`dotnet-gcdump` is lighter (it triggers a GC first, so it shows only *live* objects); `dotnet-dump` is a full process dump (larger, includes garbage not yet collected).
Answer in [QUESTIONS.md](QUESTIONS.md), then compare with [ANSWERS.md](ANSWERS.md).
