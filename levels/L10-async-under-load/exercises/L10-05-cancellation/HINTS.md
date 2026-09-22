# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Server-side counters of *active requests* stay high after clients go away. Log or count work performed after `HttpContext.RequestAborted` is cancelled (the harness's `stepsAfterAbort`).
</details>

<details><summary>Hint 2: where?</summary>

Where does the handler learn that the client is gone? Does any of its awaits observe that?
</details>

<details><summary>Hint 3: why?</summary>

ASP.NET Core signals disconnects through `HttpContext.RequestAborted`, but only code that **passes the token** (or checks it) reacts. Awaiting without the token continues to completion. Thread a `CancellationToken` through every awaited call (database, HTTP, delay) so the whole chain unwinds.
</details>
