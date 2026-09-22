# L7-04 · Solution

## What the profile shows
> Illustrative: harness figures below were measured; profiler views are what the code implies (no profiler capture).

- **Tracing:** `IsValid` looks like the top self-time frame with millions of calls: an artifact of instrumentation (no inlining, per-call overhead).
- **Sampling:** `Dictionary.FindValue` → `Key.Equals` dominate; the JIT has inlined `IsValid` away, so it doesn't appear.

## Root cause
A poor `GetHashCode` (`A + B`) makes many distinct keys collide in the same bucket, so each dictionary lookup walks a long chain calling `Equals`. The instrumentation profile misled: it inflated the cost of a tiny method that costs ~nothing in real optimised code.

## Fix
Use a hash that mixes both fields: `HashCode.Combine(A, B)`. Leave `IsValid` alone.

## Measured (20-core Linux box, .NET 10, Release)
| | time | allocated |
|---|---|---|
| before | ≈ 410 ms | 0.00 MB |
| after | ≈ 13 ms | 0.00 MB |

Times depend on your machine; ratios and the allocation/GC numbers should look similar.

## Take-aways
1. **Every profiling mode distorts something.** Tracing changes inlining and adds per-call cost; sampling can't count calls. Use one to locate, the other to confirm, and a stopwatch to settle it.
2. When a profile says 'this tiny method is hot', ask whether that is real or the tool's overhead.
3. Bad hash codes are a silent quadratic: check distribution for any type used as a key (count collisions).
4. The fix is one line; finding *which* line took the right tool interpretation.

## Go further
Instrument `Equals` with a counter (scratch copy) and print calls per lookup for both hash functions. Then try `(A * 397) ^ B` and see how good it is.

## Further reading
- [Kokosa et al., Pro .NET Memory Management](https://prodotnetmemory.com/)
- [Gregg, Systems Performance (the USE method)](https://www.brendangregg.com/systems-performance-2nd-edition-book.html)
- templates/POSTMORTEM.md (this repo): write yours first
- JetBrains dotTrace docs: profiling modes and overhead *(title only)*
