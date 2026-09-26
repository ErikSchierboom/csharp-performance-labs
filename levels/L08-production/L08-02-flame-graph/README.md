# L08-02 - Flame graph

A service is burning a full core. **Find the hot code path without reading the source.**

```powershell
./levels/L08-production/L08-02-flame-graph/run.ps1 120                      # terminal 1; note the pid
dotnet-trace collect -p <pid> --format Speedscope -o flame --duration 00:00:10    # terminal 2
# open flame.speedscope.json in https://www.speedscope.app  (drag & drop; nothing is uploaded)
```
In speedscope: **Left Heavy** view shows where time is; **Sandwich** shows callers/callees of one function.
(In Rider you can also profile with dotTrace sampling and read the call tree; same answer.)

Answer in [QUESTIONS.md](QUESTIONS.md), then compare with [ANSWERS.md](ANSWERS.md).

A note: the trace contains runtime frames too (GC-poll and thread frames); look for the `service!` frames and ignore the rest.
