# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotTrace/timeline: threads waiting on the logger's lock; `strace -c`: open/write/close per log line.
</details>

<details><summary>Hint 2: where?</summary>

What does one `Log` call do, and what does it hold while doing it?
</details>

<details><summary>Hint 3: why?</summary>

The provider opens, appends and closes the file under a global lock on the request thread: every request serialises on the disk (L5-07 + L4-02). A logging provider should **enqueue and return**, with a background writer batching to a buffered stream, and a bounded queue that drops (and counts) on overload rather than blocking requests.
</details>
