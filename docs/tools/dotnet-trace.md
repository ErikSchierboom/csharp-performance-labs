# dotnet-trace

**Answers:** where does the time go? It records stack samples (and runtime events) from a running process into a file, which you then open in a viewer. It's the CLI equivalent of a sampling profiler.

## Commands
```bash
# record 10 seconds, and also write a speedscope file next to the .nettrace
dotnet-trace collect -n <id> --duration 00:00:00:10 --format Speedscope -o run.nettrace

# quick text summary of the hottest methods, no viewer needed
dotnet-trace report run.nettrace topN -n 10

# start the program under the tracer instead of attaching (catches startup too)
dotnet-trace collect --format Speedscope -o run.nettrace -- dotnet path/to/app.dll --profile --seconds 10

# allocation and GC events instead of CPU samples
dotnet-trace collect -n <id> --profile gc-verbose --duration 00:00:00:10

dotnet-trace list-profiles                             # what the other --profile values record
```
Press Enter or `Ctrl+C` to stop early. `--duration` is `dd:hh:mm:ss`.

## Viewing the result
- **speedscope:** open [speedscope.app](https://www.speedscope.app) and drop the `.speedscope.json` file on it. It runs in your browser; the file isn't uploaded. Use **Left Heavy** to see where time went (widest bar = most time), and **Sandwich** to see one method's callers and callees.
- **Rider / dotTrace, Visual Studio, PerfView:** open the `.nettrace` directly.
- **No viewer:** `dotnet-trace report ... topN` prints the top methods by exclusive time.

## Reading it
- Find **your** code first (the exercise's namespace), then look at what it calls. Framework methods at the top of the list (`Buffer.Memmove`, `String.Concat`, `Monitor.Enter`) are usually the *cost*; the frame in your code just below them is the *cause*.
- **The samples cover every thread**, and waiting threads are sampled too. In a quick test on an exercise that does all its work on one thread, the top entries were `Memmove` (the real work) *and* `WaitHandle.WaitOne` (an idle helper thread), at the same percentage. Filter to the thread that runs `Workload.Run`, or ignore frames that are plainly waiting.

## Traps
- **Attach after warm-up.** The first seconds are JIT compilation and tiering, which the harness never measures. Profile mode already loops, so wait a couple of seconds before collecting.
- **Traces grow fast:** a 4-second trace of a busy exercise was about 11 MB. Keep `--duration` short.
- On Linux, `collect-linux` records kernel events too, but needs root. Plain `collect` covers everything in this lab.

## Docs
[dotnet-trace (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-trace) · [speedscope](https://github.com/jlfwong/speedscope#usage)
