# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotTrace with system frames / `strace -c`: how many `read` system calls does the run make?
</details>

<details><summary>Hint 2: where?</summary>

What is `bufferSize: 0`, and what does it mean for each `ReadByte()`?
</details>

<details><summary>Hint 3: why?</summary>

A `FileStream` normally reads a block (4 KB by default) and serves bytes from its buffer. With buffering disabled, each `ReadByte` is an OS call. Enable buffering (or read the whole file / blocks into an array).
</details>
