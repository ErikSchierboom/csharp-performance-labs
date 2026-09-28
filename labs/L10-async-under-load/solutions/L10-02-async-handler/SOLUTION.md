# L10-02 - Solution

## What the profile shows
- **Timeline:** pool threads in `Thread.Sleep` (or the sync API's wait).
- **Counters:** queue length rising with idle CPU.

## Root cause
A synchronous blocking call inside an `async` method. The method is async in name only; the thread is held for the whole 15 ms.

## Fix
Use the asynchronous API (`await Task.Delay` here; `ReadAllTextAsync`, `ExecuteReaderAsync`, `SendAsync` in real code).

## Take-aways
1. **`async` on the signature guarantees nothing.** Look for synchronous I/O (files, DB drivers, `Send`, `Stream.Read`), `Thread.Sleep`, `.Wait()`, `.Result`, and `lock` around slow work.
2. Analyzers help (e.g. `CA1849` call async methods in async methods; banned-API analyzers for `Thread.Sleep`).
3. Blocking on the thread pool degrades the whole process, not just one endpoint.
4. Prefer libraries with true async I/O; wrapping sync calls in `Task.Run` only moves the blocking to another pool thread.

## Extra credit
Add `Kestrel` synchronous IO usage (`AllowSynchronousIO`) to a scratch copy and see what the server tells you about it.

## Go further
Wrap the sync call in `Task.Run` in a scratch copy. Does it help? What does it cost?
