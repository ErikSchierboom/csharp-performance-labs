# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Timeline: threads *blocked on a monitor*; contention counter very high. A single lookup is fast, but eight threads doing it non-stop turn one lock into the whole program.
</details>

<details><summary>Hint 2: where?</summary>

`Get` takes the lock for a read. What does a reader need protecting *from*?
</details>

<details><summary>Hint 3: why?</summary>

Readers only need protection from writers. If a writer never changes a published object (it builds a new one and swaps a reference), readers need no lock at all. That's copy-on-write. When is it a bad idea? (Think: how big is the object, and how often are writes?)
</details>
