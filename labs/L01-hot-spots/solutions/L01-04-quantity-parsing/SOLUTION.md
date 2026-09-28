# L01-04 - Solution

## What the profile shows
Under `decimal.Parse`, the exception machinery: throwing, capturing the stack trace, unwinding to the catch,
plus allocation of the exception objects (27 MB allocated before the fix vs. 4.6 MB after).
Exact frame names differ between .NET versions.

## Root cause
About half the inputs are not numbers, so about half the iterations **throw and catch a `FormatException`**.
Exceptions are for the exceptional; a routine data condition shouldn't use them for control flow. The cost of
a throw grows with stack depth, so in a real (deeper) call stack it would be worse than here.

## Fix
`decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)`. Same acceptance rules
as `decimal.Parse` with those arguments, so the checksum matches.

## Take-aways
1. The BCL has a `Try...` variant for a reason. Use it when failure is an expected outcome.
2. Newer runtimes made exception handling substantially cheaper (.NET 9 rewrote the unwinder in managed code),
   which is exactly why this workload is large: the exercise should fail on *any* runtime, not just old ones.
   Faster exceptions are still much slower than a returned `false`.
3. **A debugger makes this dramatically worse** (first-chance exception notifications). Try the slow version
   under Debug vs Profile to feel it. Never take timings with a debugger attached.
4. Cheap production check: `dotnet-counters monitor System.Runtime` shows an `exception-count` rate. A steady
   non-zero rate on a healthy service is a smell worth investigating.

## Go further
Add a third input class ("1,234.50" with thousands separators, or `null`) and make sure your fix handles
`null` without throwing (`TryParse` accepts null and returns false).
