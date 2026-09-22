# L8-01 · Triage

Five copies of one service, each in a different state (scenarios `a` to `e`). Your job: for each one, say **which resource is the problem**: CPU, garbage collection, the thread pool, a lock, or nothing (healthy). You may only use `dotnet-counters`.

```powershell
./levels/L08-production/L8-01-triage/run.ps1 a 120            # terminal 1: prints its pid
dotnet-counters ps                          # terminal 2 (find the pid)
dotnet-counters monitor -p <pid> System.Runtime
# or, to keep numbers you can sum:
dotnet-counters collect -p <pid> --counters System.Runtime --format csv -o a.csv --refresh-interval 2 --duration 00:00:10
```
Stop each with Ctrl-C before starting the next.

Fill in [QUESTIONS.md](QUESTIONS.md). Then compare with [ANSWERS.md](ANSWERS.md).
