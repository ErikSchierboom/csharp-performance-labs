# L13-02 - Solution

## What the profile shows
- **Timeline:** request threads blocked on the logger lock; **strace:** thousands of open/write/close.

## Root cause
Synchronous, per-line file I/O under a global lock in the logging path: request latency includes disk latency and lock queueing.

## Fix
An asynchronous provider: `Log` enqueues to a bounded channel, one background thread writes to a single buffered `StreamWriter`. Flush on shutdown, drop and count when the queue is full. (In real apps use a proven async sink: Serilog's async wrapper, OpenTelemetry exporters, or the console/JSON logger to stdout, collected by the platform.)

## Take-aways
1. **Telemetry must not add latency or fail requests.** Logging, metrics and tracing are on the critical path unless you decouple them.
2. Bounded queue + drop-and-count beats blocking when the sink is slow.
3. Log less, and structure it: four lines per request is expensive at scale even when async.
4. Measure the observability tax (with and without) as part of your performance budget.

## Extra credit
What happens to log lines still in the channel when the process is killed? Design the trade-off.

## Go further
Sample the logging (1 in 10 requests) and compare p99. What information do you lose?
