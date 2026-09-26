# L06-05 - Solution

## What the profile shows
- **Sampling:** time in `Run` and the copy prologue; **disassembly:** a 512-byte block copy before each `Score()` call in the slow version, none in the fix.

## Root cause
An instance method on a *mutable* struct called through a `readonly` field forces the compiler to make a defensive copy of the whole 512-byte struct on every call.

## Fix
Make the struct `readonly` (the compiler then verifies immutability and elides the copies). Or mark just the method `readonly`. Or pass the struct by `in`/`ref readonly` to methods that take it. (The constructor here uses `Unsafe.AsRef` only to fill an `InlineArray` inside a readonly struct; a plain struct with fields wouldn't need it.)

## Take-aways
1. **Big structs make hidden copies**: passing by value, `foreach` over struct collections, and calling methods through `readonly` fields all copy.
2. `readonly struct` is nearly free correctness *and* speed: default to it for value types.
3. Analyzers/IDE hints flag defensive copies (IDE0250, 'struct can be made readonly'); the disassembly proves them.
4. Rule of thumb: structs up to ~16 bytes copy cheaply; beyond that, think about `in`/`ref` or make it a class. Below ~128 bytes the effect is small (I measured 1.2× at 128 B and 3.5× at 512 B).

## Extra credit
Remove `[MethodImpl(NoInlining)]` from `Score`. Does the difference between the two versions shrink? What does that say about inlining and copy elision?

## Go further
Keep the mutable struct but mark only `Score()` as `readonly`. Same speed-up? Then pass the struct by `in` to a static method and compare.
