# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotTrace sampling, looking at *system* time: frames in `File.OpenHandle`, `SafeFileHandle`, `RandomAccess.WriteAtOffset`, `close`. On Linux, `strace -c -f` counts syscalls.
</details>

<details><summary>Hint 2: where?</summary>

Which .NET call is invoked 20,000 times, and what does *each* call do to the file?
</details>

<details><summary>Hint 3: why?</summary>

`File.AppendAllText` opens the file, writes, and closes it, every time: several system calls (open/write/close, and metadata) per 20 bytes. A `StreamWriter` with a buffer batches many small writes into few large ones. What is the trade-off if the process crashes before the buffer is flushed?
</details>
