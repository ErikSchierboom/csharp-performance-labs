# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Counters per layer: cache loads per key, third-party calls per request (`externalCalls`), failures (`giveUps`), pool waiters. Draw the request path and measure each hop.
</details>

<details><summary>Hint 2: where?</summary>

Fix the layer nearest the user first, then re-measure; the next problem will look different. Ask of each layer: is it bounded? is it shared? does it hold a scarce resource while waiting?
</details>

<details><summary>Hint 3: why?</summary>

This is L12-02 (stampede), L12-05 (retry storm) and L11-04 (connection held across a slow call) in one request path. Each amplifies the next: a cold cache creates concurrent loads → the third party overloads → immediate retries add load → connections are held longer → the pool starves.
</details>
