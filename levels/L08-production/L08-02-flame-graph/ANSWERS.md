# Answers (measured)

> From a real `dotnet-trace --format Speedscope` capture (8 s) of this service on a 20-core Linux box, .NET 10 runtime, methods marked non-inlinable so they appear as distinct frames. I summed the trace's event durations by stack.

1. **Self time:** almost entirely in the hashing loop (`Hash`, called from many places: the leaf). **Inclusive:** `Compress` dominates the `service!` frames.
2. **Path:** `Main → HandleRequest → Render → Template → Layout → Widgets → Serialize → Compress → Hash`.
3. `Compress` accounts for about **99% of the time spent inside `HandleRequest`** in my capture (≈ 47.5% of all trace time vs `HandleRequest` ≈ 47.9%; the other half of the trace time was runtime/GC-poll frames, which you can ignore).
4. It is reached only on every 5th request (`Render` takes the `Template` branch when `i % 5 == 0`) and it is called four times per such request. The *expensive minority* dominates: that's why an average per-request view (or reading the code top-down) hides it. **Counts** (tracing mode) would show `Compress` is called rarely; **time** (sampling) shows it's where the CPU is: the combination is the diagnosis.
5. `Authenticate`, `Route`, `Lookup`, `Normalize` are called on every request but are tiny (hundreds of loop rounds versus 12,000 in `Compress`). They are wide in *call count* but thin in *time*. In a flame graph, **width = time**, so they're slivers.

## What you'd do next
Optimise or cache `Compress` (it's the only code worth touching), or avoid calling it (compress once, not four times per request). Then re-profile: the graph will change shape.

## Reveal
`Compress` = `Hash(i, 12_000)`; everything else is 100–200 rounds.
