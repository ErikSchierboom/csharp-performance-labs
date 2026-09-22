# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Look at the **disassembly**: `DOTNET_JitDisasm="Run"` (or BenchmarkDotNet's `[DisassemblyDiagnoser]`). Look for a large block copy (`vmovdqu` runs / `rep movs`) *before each call* to `Score`.
</details>

<details><summary>Hint 2: where?</summary>

The struct sits in a `readonly` field. The compiler must guarantee the field isn't changed by the call. What must it assume about `Score()`?
</details>

<details><summary>Hint 3: why?</summary>

If a method is called on a `readonly` field and the compiler can't prove the method leaves the struct alone, it calls the method on a **defensive copy**: a 512-byte copy per call. Declaring the struct `readonly` (or just the method `readonly`) tells the compiler it's safe, so no copy is needed.
</details>
