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

**Requirements:** Docker or Podman (Docker Desktop on Windows/macOS). The script builds a small Linux image from [Dockerfile](Dockerfile) on first run (the .NET SDK/runtime images are pulled once) and applies the limit with `--memory` (plus `--memory-swap` equal to it, so no swap), which is a cgroup limit exactly like in Kubernetes. It works identically on Windows, macOS and Linux; `none` runs the same container without a limit. No prior `build-all.ps1` is needed for this lab.

Manual equivalent: `docker build -t perflab-l08-04 .` then `docker run --rm --memory=220m --memory-swap=220m -e DOTNET_GCConserveMemory=9 perflab-l08-04`.
