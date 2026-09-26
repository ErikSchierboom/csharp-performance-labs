# L06-06 - Solution

## What the profile shows
- **Disassembly:** `call [rax+…]` (indirect call) per element in the slow loop; inlined multiplies in the fix.
- **`perf stat`:** many `branch-misses` on the indirect branch (slow).

## Root cause
Interface dispatch over a heterogeneous sequence in random order: an unpredictable indirect call per element that also blocks inlining.

## Fix
Store the data grouped by concrete type (three arrays) and loop over each. Every call disappears (inlined field arithmetic). Other fixes: sort/group the interface array by type; use a `switch` on a tag field; generics with struct constraints (`where T : struct, IShape`) so the JIT specialises per type.

## Take-aways
1. **Virtual/interface calls are cheap when predictable and costly when not**; the cost is branch misprediction plus lost inlining.
2. `sealed` helps only when the JIT can see the exact type; here it can't (the static type is the interface).
3. Grouping by type (or a struct-generic approach) is a standard data-oriented fix for polymorphic hot loops.
4. Dynamic PGO (on by default since .NET 8) devirtualises the *dominant* type per call site; it can't rescue an even mix.

## Extra credit
Change the mix to 95% `Rect`. What does the slow version do, and why? (Think dynamic PGO.)

## Go further
Keep the interface array but sort it by type once; is that enough? Then try the generic struct-constraint approach.
