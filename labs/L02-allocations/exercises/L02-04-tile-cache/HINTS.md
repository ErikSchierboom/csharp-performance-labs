# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Use dotTrace's **timeline** or a GC view, and compare gen1 counts. Also look at *which threads* are running. Is there a thread you didn't create doing work?
</details>

<details><summary>Hint 2: where?</summary>

Time is not in `Paint` or `Fingerprint`. It is in allocation and in the GC itself. Look at the members of `Tile` that are *not* fields or methods you call yourself.
</details>

<details><summary>Hint 3: why?</summary>

A class with a finalizer is registered with the runtime when it is allocated (a slower allocation path), and when it dies the GC can't just free it: it is put on the finalization queue and **survives** at least one more collection, so it gets promoted. Does `Tile` own anything that needs a finalizer?
</details>
