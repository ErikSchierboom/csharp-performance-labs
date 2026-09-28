# taskset
> Linux only

**Answers:** nothing by itself. It controls **which CPU cores** a program may run on. Use it to make measurements steadier, or to see how a program behaves on a smaller machine than yours.

`taskset` ships with Linux (package `util-linux`). No install needed.

## Commands
```bash
taskset -c 0-7 dotnet run -c Release --project labs/<lab>/exercises/<id>       # cores 0 to 7 only
taskset -c 0,2 dotnet <...>.dll                                                    # two specific cores
taskset -cp <pid>                                                                  # show a running process's cores
nproc --all; lscpu --extended                                                      # how many cores, and which are which
```

## When it helps
- **Noisy numbers.** The web exercises (Lab 9 and up) run the server *and* the load generator in one process. Keeping it to a fixed set of cores stops the OS from moving it around, so repeated runs agree better.
- **"Works on my machine."** Your laptop may have 20 cores and production 2. Running on `-c 0,1` shows roughly what a 2-core container sees. The runtime sizes the thread pool and the GC from the cores it's allowed to use.
- **Hybrid CPUs.** On Intel P-core/E-core chips, `lscpu --extended` shows which core numbers are which (different `MAXMHZ`). Pinning to one type gives much more consistent timings.

## Traps
- **Don't pin to a single core** for timing runs. The JIT's background compiler then shares the core with your code, and methods take far longer to reach their optimised version (see [RUNTIME.md](../RUNTIME.md)).
- The harness calibrates against this machine's speed. Pinning doesn't change the budgets, only how steady the numbers are.
- **Windows:** `start /affinity 3 dotnet ...` in `cmd` (the value is a hex mask: `3` = cores 0 and 1), or set **Affinity** in Task Manager.

## Docs
[taskset man page](https://man7.org/linux/man-pages/man1/taskset.1.html)
