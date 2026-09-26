# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotMemory **allocations**, grouped by *allocation site* (call stack), not by type. Then compare with the type view. Several types show up; that is a clue that it's not one bug.
</details>

<details><summary>Hint 2: where?</summary>

Everything happens inside `LineParser.Parse`. Count the allocating calls per line: `Split`, `Trim`, `ToUpperInvariant`, `ToLowerInvariant`, `Select`, `Where`, `Aggregate`. Which of these produce a new `string` or `string[]`? Which produce iterator or delegate objects?
</details>

<details><summary>Hint 3: why?</summary>

Each `Split` allocates an array *and* a string per piece. `Trim`/`ToLower` allocate again when they change the text. The lambdas capture `disabledFlags`, so the compiler allocates a closure object and delegates per call. Ask: can I *look at* part of the line without copying it? (`ReadOnlySpan<char>`). And does the `HashSet<string>` of disabled flags need to be a set of strings at all?
</details>
