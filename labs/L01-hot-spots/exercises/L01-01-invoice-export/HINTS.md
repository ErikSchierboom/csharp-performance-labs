# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Start with a **sampling** profile in Release. Open the hot spots / call tree and sort by **own (self) time**,
not total time. Also glance at the harness output: allocated MB and gen0/1/2 counts are clues too.
</details>

<details><summary>Hint 2: where?</summary>

`FormatRow` looks like the suspicious, "expensive" method (`string.Format`, several args). Check what share of
time it actually has. Then look at what sits at the top of self time and who calls it.
</details>

<details><summary>Hint 3: why?</summary>

Strings are immutable. What happens to the existing contents every time you write `report += ...`?
How does the total amount of copying grow as the report grows? And what is special about objects larger than
~85,000 bytes on the .NET heap?
</details>
