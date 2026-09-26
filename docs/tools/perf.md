# perf
> Linux only

**Answers:** what is the *CPU* doing? Instructions per cycle, cache misses and branch misses: things no .NET profiler shows. Use it when the profiler says "all the time is on this one line" and the line looks innocent.

## Install
```bash
sudo dnf install perf                                         # Fedora
sudo apt install linux-tools-common linux-tools-$(uname -r)   # Ubuntu/Debian
```
If `perf` says it has no permission, check `cat /proc/sys/kernel/perf_event_paranoid`. At `2` (a common default) you can measure your own processes in user mode, which covers everything here. Lower it with `sudo sysctl kernel.perf_event_paranoid=1` only if you need more.

## Commands
Build once, then run the `.dll` directly, so you measure the exercise and not the `dotnet run` build step:
```bash
dotnet build -c Release levels/<level>/exercises/<id>
perf stat -B dotnet levels/<level>/exercises/<id>/bin/Release/net10.0/<id>.dll --profile --seconds 5
```
Run the same command for `solutions/<id>` and compare the two.

## Reading it
- **IPC** = instructions ÷ cycles. Around 3 or more is a CPU working flat out. Below 1 means it's mostly **waiting**, usually for memory.
- **cache-misses:** compare the exercise with the solution rather than reading the absolute number. A fix that cuts the misses tenfold explains a big speed-up.
- **branch-misses:** the same idea. Many more misses for the same work means the CPU keeps guessing wrong.
- The whole run is counted, including startup and the harness. Longer `--seconds` makes the workload dominate.

## Traps
- **Hybrid Intel CPUs** (P-cores and E-cores) report each event twice, as `cpu_core/...` and `cpu_atom/...`. The percentage on the right is how much of the run each type was counted for. Read the row with the high percentage, or pin the run to the P-cores with [taskset](taskset.md).
- In VMs and containers, some events show `<not supported>`: the hardware counter isn't exposed there.
- `perf record` also works on .NET (set `DOTNET_PerfMapEnabled=1` so it can name JIT-compiled methods), but most frames are runtime internals. For "where does the time go", [dotnet-trace](dotnet-trace.md) is easier to read.

## Docs
[perf wiki: tutorial](https://perfwiki.github.io/main/tutorial/) · [perf-stat man page](https://man7.org/linux/man-pages/man1/perf-stat.1.html)
