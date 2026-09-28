# L04-07 - Solution

## What the profile shows
> Illustrative: profiler views are what the code implies (no profiler capture).

- **Wall time ≈ items × latency** (serial), CPU and thread count flat.

## Root cause
A concurrency limit of 1 turns a parallelisable I/O workload into a serial one.

## Fix
Raise `MaxDegreeOfParallelism` to what the downstream can serve (32 here) after measuring where its latency starts to climb.

## Take-aways
1. **Bounds have two failure modes:** too loose overloads the dependency (L04-03), too tight wastes it.
2. Little's law: throughput = concurrency ÷ latency. If you know two, you know the third.
3. Choose limits from measurement and revisit them when the dependency changes.
4. Make the limit configurable and observable (in-flight gauge).

## Extra credit
What's the *smallest* degree that finishes in under 200 ms?

## Go further
Sweep the degree from 1 to 256 and plot total time. Where does it stop improving?
