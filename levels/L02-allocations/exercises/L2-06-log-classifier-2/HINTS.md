# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotMemory allocations by type. Count how many object types are created per *line*. The regex engine is not the problem any more; what is it handing back?
</details>

<details><summary>Hint 2: where?</summary>

`Regex.Match` allocates a `Match`, a `GroupCollection`, `Group` objects and capture arrays; `Groups["name"]` looks up by name; `.Value` allocates a string per group you read. You read only three groups, but the engine records all of them.
</details>

<details><summary>Hint 3: why?</summary>

The regex API is built around objects, so it can't be zero-allocation when you need to read groups. The line format is simple and fixed. Would a hand-written parser over `ReadOnlySpan<char>` (`IndexOf`, slicing, `SequenceEqual`) be able to find the same fields without creating any of those objects? Be careful to keep the *same* grammar: the checksum will tell you if you don't.
</details>
