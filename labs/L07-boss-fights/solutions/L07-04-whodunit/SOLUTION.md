# L07-04 - Solution

## What the profile shows

- **Sampling:** nearly all of `Workload.Run` is `Dictionary.FindValue`'s own time. Nothing of yours is visible.
- **Tracing:** the same picture. `FindValue` has 92% of the time as *own* time; `IsValid` and `Key.GetHashCode` add up to under a millisecond for the whole session. `Key.Equals` doesn't appear under `FindValue` at all.

## Root cause
A poor `GetHashCode` (`A + B`) gives many distinct keys the same hash, so each dictionary lookup walks a long chain calling `Equals`. The profiles didn't mislead, but they stopped one level short: the hot frame is the framework's `FindValue`, and the cause is the hash code your type gives it. `GetHashCode` itself is cheap and looks innocent in every profile.

## Fix
Use a hash that mixes both fields: `HashCode.Combine(A, B)`. Leave `IsValid` alone.

## Take-aways
1. **A profile shows where the time is, not whose fault it is.** When the hot frame is framework code (`FindValue`, `Sort`, `Concat`), ask what *your* code hands it: keys, comparers, sizes.
2. **Inlined methods are invisible** in sampling and tracing alike: their cost is booked to the caller. A tiny method that "costs nothing" in the profile can still be the cause.
3. Bad hash codes are a silent quadratic: check distribution for any type used as a key (count collisions).
4. The fix is one line; finding *which* line took the right tool interpretation.

## Extra credit
Put `[MethodImpl(MethodImplOptions.NoInlining)]` on `Key.Equals` in a scratch copy and profile again, in both modes. Where does the time move, and how does the run time change? What does that tell you about trusting a profile of code the JIT can inline?

## Go further
Count the distinct hash codes your keys produce (a `HashSet<int>` of `GetHashCode()` results) for `A + B`, `HashCode.Combine(A, B)` and `(A * 397) ^ B`. Which is good enough for these keys, and why is `HashCode.Combine` the safe default?
