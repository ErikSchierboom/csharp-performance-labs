# L08-04 - Container limits

The same service (150 MB of live data plus a stream of short-lived garbage) is run under different **memory limits**. Predict, then observe:

```powershell
./levels/L08-production/L08-04-container-limits/run.ps1 none
./levels/L08-production/L08-04-container-limits/run.ps1 260M
./levels/L08-production/L08-04-container-limits/run.ps1 220M
./levels/L08-production/L08-04-container-limits/run.ps1 200M
./levels/L08-production/L08-04-container-limits/run.ps1 220M DOTNET_GCHeapHardLimit=0xC800000      # 200 MB explicit heap cap
./levels/L08-production/L08-04-container-limits/run.ps1 220M DOTNET_GCConserveMemory=9
```
The service prints its GC counts, heap size and **the memory limit the GC believes it has** every 2 s, and total work done ("allocations") at the end (a throughput proxy).
Fill in [QUESTIONS.md](QUESTIONS.md), then compare with [ANSWERS.md](ANSWERS.md).

**Platform note:** the memory limit itself is a real Linux cgroups feature (via `systemd-run`), not something any script can emulate; only `run.ps1 none` (unlimited) runs natively on Windows/macOS. Do the limited runs under WSL2, a Linux VM, or a Linux CI runner.

Podman/Docker work too: `podman run --memory=220m ...` with a container image of the published output (not provided here).
