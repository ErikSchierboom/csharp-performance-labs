# dotnet-stack

**Answers:** what is every managed thread doing *right now*? It prints one snapshot of all stacks and exits. It's the fastest way to see *what* a stuck or slow process is waiting on.

## Commands
```bash
dotnet-stack report -n <id>
dotnet-stack report -n <id> > stacks-1.txt             # save it to compare with a second one
```

## Reading it
```
Thread (0x620E9):
  System.Private.CoreLib!System.Buffer.MemmoveInternal(...)
  L1-01-invoice-export!InvoiceExport.InvoiceExporter.Export(...)
  L1-01-invoice-export!InvoiceExport.Workload.Run()
  PerfLab.Harness!PerfLab.Harness.Lab.RunProfile(...)
```
- The **top** line is where the thread is now. Read downwards for how it got there.
- **Look for repetition.** If most threads end in the same frame (`Monitor.Enter`, `Task.Wait`, `TaskAwaiter.GetResult`, `Thread.Sleep`, a socket read), that's the bottleneck.
- A thread whose top frame is a wait (`WaitHandle.WaitOne`, `Monitor.Wait`) is **idle**, not busy. The runtime always has a few of these.

## Traps
- **It's one moment.** Take two or three reports a few seconds apart. A frame that keeps showing up is a pattern; a frame you saw once might be luck.
- It only shows managed frames (native code shows up as `[Native Frames]`). For a timeline of what threads did over time, use [dotnet-trace](dotnet-trace.md). For stacks merged by call path, take a [dump](dotnet-dump.md) and run `pstacks`.

## Docs
[dotnet-stack (Microsoft Learn)](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-stack)
