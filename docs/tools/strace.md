# strace
> Linux only

**Answers:** how often does the program call into the operating system, and for what? Opening files, reading, writing, sockets, sleeping. Use it when a profiler shows time "in the kernel" or "outside your code" and you want to know what that time was.

## Install
```bash
sudo dnf install strace                                # Fedora
sudo apt install strace                                # Ubuntu/Debian
```

## Commands
```bash
# summary table: count and time per system call (-f follows every thread)
strace -f -c dotnet levels/<level>/exercises/<id>/bin/Release/net10.0/<id>.dll

# only file-related calls
strace -f -c -e trace=%file,read,write,close dotnet <...>.dll

# attach to a running process for a while, Ctrl+C to stop and print the summary
strace -f -c -p <pid>
```
`-f` matters: .NET is multithreaded, and without it you only see the main thread.

## Reading it
```
% time     seconds  usecs/call     calls    errors syscall
------ ----------- ----------- --------- --------- ----------------
 61.20    0.412345           1    400123           read
```
- Look at **calls** first. Compare the number with how much work the program does. 400,000 `read` calls to read a 400 KB file is one per byte.
- `openat`/`close` counts that match the number of items processed mean a file is being opened per item.
- `futex` is threads waiting on each other. It's always there in .NET. It only matters if it dominates.

## Traps
- **strace slows every system call down a lot.** Trust the *counts*, not the timings.
- It counts the whole run, startup included. Run it on a normal (non-profile) run, and compare the exercise with the solution.
- Attaching to a process you didn't start may need `sudo`, depending on `kernel.yama.ptrace_scope`.

## Docs
[strace man page](https://man7.org/linux/man-pages/man1/strace.1.html)
