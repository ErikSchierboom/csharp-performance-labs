# CLI tools

Short guides to the command-line tools this lab uses: what each one answers, the commands you'll actually type, and how to read what comes back. They're free, they work without an IDE, and most of them work on any OS. You'll need them in Lab 8, and they're a good second opinion in every other lab.

For *which kind* of measurement a symptom needs (sampling, tracing, snapshots, ...), start with the [profiling guide](../PROFILING-GUIDE.md). These pages are the "how do I type it" companion.

## Which tool for which question
| Question | Tool | OS |
|---|---|---|
| Is the process busy, allocating, collecting, queueing? (the first look) | [dotnet-counters](dotnet-counters.md) | any |
| Where does the time go? | [dotnet-trace](dotnet-trace.md) | any |
| What is on the heap, and what holds it? | [dotnet-gcdump](dotnet-gcdump.md) | any |
| Everything in memory, inspected offline (threads, locks, roots) | [dotnet-dump](dotnet-dump.md) | any |
| What is every thread doing *right now*? | [dotnet-stack](dotnet-stack.md) | any |
| Cache misses, branch misses, instructions per cycle | [perf](perf.md) | Linux |
| How many system calls, and which ones? | [strace](strace.md) | Linux |
| How many TCP connections, in which state? | [ss](ss.md) | Linux |
| Run on fewer cores, or on the same cores every time | [taskset](taskset.md) | Linux |

## Install the .NET tools once
```bash
dotnet tool install --global dotnet-counters
dotnet tool install --global dotnet-trace
dotnet tool install --global dotnet-gcdump
dotnet tool install --global dotnet-dump
dotnet tool install --global dotnet-stack
```
Later, `dotnet tool update --global <name>` updates one. If the shell can't find them after installing, add `~/.dotnet/tools` (Linux/macOS) or `%USERPROFILE%\.dotnet\tools` (Windows) to your `PATH`.

## Point a tool at an exercise
All of these tools attach to a **running process**, and a normal exercise run is over in a second. Use the harness's profile mode, which keeps the workload looping:

```bash
# terminal 1: keep the exercise busy for a minute
dotnet run -c Release --project labs/<lab>/exercises/<id> -- --profile --seconds 60

# terminal 2: attach by name (the exercise id)...
dotnet-counters monitor -n <id>
# ...or find the pid first
dotnet-counters ps
dotnet-counters monitor -p <pid>
```

- **Every .NET tool here takes `-n <name>` or `-p <pid>`.** The name is the exercise id (`L1-01-invoice-export`) when you start it with `dotnet run`. If you start the `.dll` with `dotnet <file>.dll`, the process is called `dotnet`, so use the pid.
- `dotnet-counters ps` lists **every** .NET process, IDEs and build servers included. Look for the exercise id in the command-line column.
- Lab 8 exercises print their own pid when they start, so you can skip `ps` there.
- The tools only see processes running as the same user. Don't start the exercise with `sudo`.
