# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Compare two counters over time: `dotnet-counters` **`gc-heap-size`** (flat) vs **`working-set`** (climbing). When they disagree, the memory isn't in the managed heap. OS tools (`pmap -x <pid>`, `/proc/<pid>/smaps`) show where the growth lives.
</details>

<details><summary>Hint 2: where?</summary>

A managed leak shows up as reachable objects; this doesn't. What in the code allocates memory that the garbage collector *doesn't manage*, and what is responsible for giving it back?
</details>

<details><summary>Hint 3: why?</summary>

The GC reclaims the small managed wrapper object, but it knows nothing about the 1 MB block it pointed to. Unmanaged memory needs an explicit release: `IDisposable` (deterministic) plus a finalizer or `SafeHandle` as a safety net.
</details>
