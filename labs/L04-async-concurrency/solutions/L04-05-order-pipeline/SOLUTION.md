# L04-05 - Solution

## What the profile shows
- **Metric:** `maxQueued` ≈ 25844
- **Metric:** `peakHeapMb` ≈ 254 Mb
- **allocations:** live `byte[]` climbs then falls as the consumer drains.

## Root cause
An unbounded queue between mismatched stages. Nothing slows the producer, so the queue length is limited only by memory.

## Fix
`Channel.CreateBounded(capacity)` with `FullMode = Wait`, and `await WriteAsync` in the producer. The producer now runs at the consumer's pace, and memory use is capped at `capacity x item size`.

## Take-aways
1. **Every queue needs a bound.** An unbounded queue hides overload until it becomes an outage.
2. Back-pressure propagates: a slow consumer slows the producer, which can in turn slow *its* caller: that's the system telling you about capacity.
3. Pick the bound from memory budget and acceptable latency according to Little's law (queue length x service time = wait). Add load shedding when waiting isn't acceptable.
4. Same idea at every scale: thread-pool queues, message-broker prefetch, HTTP accept queues.

## Extra credit
Make the consumer 4× faster. Where does `maxQueued` settle for the *unbounded* version, and why?

## Go further
Switch `FullMode` to `DropOldest` and explain what you gave up. When is dropping the right call?
