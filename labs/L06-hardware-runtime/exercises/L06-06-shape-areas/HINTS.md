# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Sampling shows the loop and tiny `Area` methods. `perf stat -e branch-misses` (mispredicted *indirect* branches), and JIT disassembly showing an indirect call per element.
</details>

<details><summary>Hint 2: where?</summary>

The loop calls `s.Area()` through an interface on elements of *three different types in random order*. Can the JIT know, at that call, which method it will be?
</details>

<details><summary>Hint 3: why?</summary>

With one dominant type, dynamic PGO devirtualises and inlines the call. With three types in random order, the indirect branch target is unpredictable, so the CPU mispredicts it often, and nothing can be inlined. If you process the shapes **grouped by concrete type**, every loop is monomorphic and the bodies inline.
</details>
