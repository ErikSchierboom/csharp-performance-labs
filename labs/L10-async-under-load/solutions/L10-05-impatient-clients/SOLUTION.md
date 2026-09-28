# L10-05 - Solution

## What the profile shows


## Root cause
The handler never observes `RequestAborted`: every abandoned request runs to completion. Here the shared resource is a 4-slot `SemaphoreSlim`. Abandoned requests wait for a slot (and stay in the queue after the client is gone), then hold it for the full 200 ms. In real code the slot is a database connection, a thread or a downstream call's capacity.

Note what *doesn't* change: the client's own timing (each impatient client gives up after 30 ms either way). The gain shows up in the service, and in the clients that stayed.

## Fix
Pass `ctx.RequestAborted` to every awaited operation (`Capacity.WaitAsync(ct)` and `Task.Delay(10, ct)` here; `SaveChangesAsync(ct)`, `SendAsync(req, ct)`, `ReadAsync(ct)` in real code). An `OperationCanceledException` from a client disconnect is normal and is not logged as an error. The `finally` blocks release the slot and decrement the in-flight count when the exception unwinds.

## Take-aways
1. **Cancellation is cooperative: you must plumb the token through.** A missing token on one call breaks the chain.
2. Wasted work under retries is a positive feedback loop; cancellation is part of overload protection.
3. Add timeouts *inside* the server too (linked `CancellationTokenSource` with a deadline) so slow downstreams don't hold requests forever.
4. Accept `CancellationToken ct` in your own async methods and pass it on; analyzers (CA2016) flag missing forwarding.

## Extra credit
Make the slow work CPU-bound (a loop) instead of awaits. How do you make *that* cancellable?

## Go further
Add a server-side deadline of 100 ms by linking `RequestAborted` with a `CancellationTokenSource(100)`. What status should the client see?
