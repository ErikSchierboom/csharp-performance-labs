# Contributing

Everything else in this repo is written for someone *solving* an exercise. This file is for someone *adding* one, fixing a bug in an existing one, or otherwise changing the lab itself.

## Adding a new exercise

### Naming
`L<level>-<NN>-<slug>` (e.g. `L2-07-route-stats`). A level's final boss is `L<level>-boss-<slug>` instead of a number. Level 8 is the exception: its labs are compiled "mystery services" (`L8-01-triage`, …), not exercises; see [levels/L08-production/README.md](levels/L08-production/README.md) if you're adding one of those instead.

### Required files
Every exercise is a matched pair of projects:

```
levels/<level>/exercises/<id>/
    README.md        symptom + budgets table, NOT the diagnosis. See any existing exercise for the shape.
    HINTS.md          progressive hints, one <details> block per hint, cheapest tool first.
    Workload.cs       the deliberately slow/broken implementation.
    Program.cs        calls Lab.Run(new LabSpec(...), args): the single source of truth for budgets and checksum.
    <id>.csproj       OutputType Exe, ProjectReference to src/PerfLab.Harness/PerfLab.Harness.csproj.
    Properties/launchSettings.json   'Measure' (no args, first so plain `dotnet run` measures) and 'Profile' (`--profile --seconds 15`) profiles; copy from any existing exercise. A launch profile cannot select a build configuration, so don't try to encode Release in it.

levels/<level>/solutions/<id>/
    SOLUTION.md       what the profile shows, root cause, the fix, "go further" question, further reading.
    Workload.cs       the fixed implementation, same public surface, same checksum as the exercise.
    Properties/launchSettings.json   same as the exercise's.
    <id>.csproj       same shape, but links the exercise's Program.cs instead of duplicating it:
                      <Compile Include="..\..\exercises\<id>\Program.cs" Link="Program.cs" />
```

The `<Compile Include>` link is what guarantees the exercise and its solution are graded by the exact same budgets and checksum; never copy `Program.cs` into the solution folder instead of linking it.

### Setting budgets and the checksum
`Program.cs` constructs a `LabSpec`, see [`src/PerfLab.Harness/Lab.cs`](src/PerfLab.Harness/Lab.cs) for what every field means (it's fully XML-documented). In short:
- **`ExpectedChecksum`**: run the *fixed* `Workload.cs`, read the value it returns, paste it in. Both the exercise and the solution must produce the same checksum: that's what "you kept the output correct" means.
- **`MaxMedianMs`**: don't just paste in the number you measured, your machine almost certainly isn't the reference machine, so a raw measurement bakes in your machine's skew for everyone else who runs the gate (see README.md's "The harness" section for what the scaling actually does):
  1. Run any already-built exercise once, e.g. `dotnet run -c Release --project levels/L01-hot-spots/exercises/L1-01-invoice-export`, and read the printed `Machine factor X.XXx vs. reference` line. The spin loop it's timing is workload-independent, so any exercise gives the same factor.
  2. Run your new solution and read its raw **median time** from the run table, not the "budget" line, that one's already scaled.
  3. Divide: `reference_ms = raw_median_ms / factor`. That's the number to write into `MaxMedianMs`.
  4. Add headroom on top.

  If the workload's time is dominated by a fixed wall-clock wait instead of CPU work (`Task.Delay`, `Thread.Sleep`, a real request round trip via `WebRig.Drive`), set `ScaleTime: false` instead of scaling it: that wait doesn't get faster on a quicker CPU, so no factor computed this way would be correct for it. Write the raw measured number directly in that case, with headroom.

  The same `ScaleTime: false` applies if the workload is dominated by the OS's native heap allocator (`Marshal.AllocHGlobal`/`FreeHGlobal`, or other unmanaged allocation), but for a different reason: allocator cost isn't a CPU-speed question at all, it depends on which code path a given block size and pattern hits (Windows' heap manager/`VirtualAlloc`, glibc `malloc`/`mmap` on Linux, macOS's zone allocator), and those differ *qualitatively* between platforms, not by a single speed ratio any factor could capture. Known gap: repeated 1 MB `AllocHGlobal`/`Free` calls (`L7-02-phantom-leak`) measured a stable ~35 ms on Windows against a ~6 ms budget calibrated elsewhere, roughly a 6x platform gap for that exact pattern. There's no portable way to calibrate this, so pad the raw number generously (2-3x what you measure) rather than trying to tighten it, and where the exercise has a real correctness gate available (retained memory, a reported metric like `privateMB`), prefer that over the time budget for the actual discrimination, since it's usually what the exercise is really testing anyway.
- **`MaxAllocatedMB`**: same idea, but **not** scaled: allocation budgets are absolute.
- Only set `MaxGen2Collections`, `MaxRetainedMB`, `Reset`, `MaxP99Ms`, `MaxCpuMs`, `MaxMetrics`, or `ScaleTime` if the exercise's specific lesson needs that gate (LOH/leak/concurrency levels, or a fixed-delay workload). Leaving them at their defaults means "not gated" (or, for `ScaleTime`, "scaled normally").

The slow (exercise) version must **fail** at least one budget; the fixed (solution) version must **pass all of them**, with an identical checksum. That pairing is what `scripts/perf-gate.ps1` checks automatically (see Validating, below). It's also the whole point of the exercise, so don't skip actually verifying it by hand first.

### Registering the project
Add a `<Project Path="...">` line to the right `<Folder>` in **both** solution files:
- `PerfLab.slnx` → path under `levels/<level>/exercises/<id>/<id>.csproj`
- `PerfLab.Solutions.slnx` → path under `levels/<level>/solutions/<id>/<id>.csproj`

Then add a row for it to that level's `levels/<level>/README.md` exercise table.

### Style
The existing exercises share a voice: the symptom is described in terms of what you'd *observe*, never what's wrong with the code; `README.md` never uses the word "bug"; hints escalate from "which tool" to "which line" without ever just stating the fix. Read two or three existing exercises in the target level before writing a new one; matching that voice matters more than matching any template mechanically.

## Validating your change
```powershell
./scripts/perf-gate.ps1 <prefix>   # e.g. ./scripts/perf-gate.ps1 L2, or a single exercise's L-number
```
Requires [PowerShell 7+](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell) (`pwsh`), which runs the same on Windows, Linux and macOS; no bash/WSL needed.
Builds both `.slnx` solutions in Release, then for every exercise/solution pair runs it and checks: every solution exits `0` (pass), every exercise exits `1` or `2` (fails its own budgets: a slow version that accidentally passes is the one thing this script exists to catch). A mismatch prints `SURPRISE` and the script exits non-zero.

This is also what [.github/workflows/perf-gate.yml](.github/workflows/perf-gate.yml) runs in CI on PRs touching `src/**`, `levels/**`, `solutions/**`, or `Directory.Build.props`.

## Other changes
- **Docs** (`README.md`, `ROADMAP.md`, `docs/`, `templates/`): built with MkDocs. `./scripts/build-docs.ps1` (or `-Serve` for local preview) builds under `--strict`, which fails on broken internal links; run it before opening a PR that touches markdown.
- **The harness** (`src/PerfLab.Harness`, `src/PerfLab.Harness.Web`): keep XML doc comments current on the public API (`LabSpec`, `Lab.Run`, `Lab.RecordLatency`, `Lab.Report`, `WebRig`'s public members); they're the first thing IntelliSense shows an exercise author.
