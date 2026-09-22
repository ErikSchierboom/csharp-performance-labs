# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Sampling profile, Release. Expand the call tree under `LineParser.Parse` and look at *which parts* of the
regex machinery show up. There are two very different phases: preparing a pattern, and running it.
</details>

<details><summary>Hint 2: where?</summary>

Frames with names like parser / writer / char-class / constructor belong to *preparing* the pattern.
Frames with names like scan / interpreter / match belong to *running* it. Which group dominates?
</details>

<details><summary>Hint 3: why?</summary>

`new Regex(pattern)` parses the pattern text and builds the matching program every time it is called.
How often does the pattern change between calls? Then look at the allocation profile: those 380 MB are
mostly the by-products of that work.
</details>
