# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Server-side counters of *active requests* stay high after clients go away (the harness's `peakInFlight`), and the patient clients' latency is far above the 200 ms the work takes. Log or count work performed after `HttpContext.RequestAborted` is cancelled (`stepsAfterAbort`).
</details>

<details><summary>Hint 2: where?</summary>

Where does the handler learn that the client is gone? Does any of its awaits observe that, including the one that waits for a free slot?
</details>

<details><summary>Hint 3: why?</summary>

ASP.NET Core signals disconnects through `HttpContext.RequestAborted`, but only code that **passes the token** (or checks it) reacts. Awaiting without the token continues to completion. Thread a `CancellationToken` through every awaited call (waiting for a slot, database, HTTP, delay) so the whole chain unwinds and each slot goes back to someone who is still listening.
</details>
