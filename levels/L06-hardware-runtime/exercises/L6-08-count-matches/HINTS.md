# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Sampling shows a delegate invocation (`<>c.<Run>b__…`) inside LINQ's `Count`. The disassembly of a simple counting loop over a `Span<int>` shows SIMD instructions (`vpcmpeqd`) instead of scalar compares.
</details>

<details><summary>Hint 2: where?</summary>

Per element the slow version does an *indirect call to the lambda*, then a compare and a conditional increment. What would let the CPU do 8 compares in one instruction?
</details>

<details><summary>Hint 3: why?</summary>

Modern CPUs have vector registers (AVX2: 8 ints). The BCL already vectorises common operations over spans (`Count`, `IndexOf`, `Contains`, `Sum`…). Search `MemoryExtensions` for the operation before writing SIMD by hand.
</details>
