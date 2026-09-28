# Post-mortem

You won't write one of these until Lab 7, but here is the template completed for a small, real-shaped incident, so the format is familiar.
(Fictional service; the defect is the one from L00-01.)

## Feed page slow for power users, 2026-09-20

#### Summary
The notifications page for accounts with large histories took seconds to load. Median users were unaffected. About 1% of accounts, all of them the heaviest users.

#### Impact
p50 40 ms (unchanged); p99 2.4 s (was 90 ms last month); no errors. CPU one core pegged per slow request.

#### Timeline
1. Alert on p99 only, so it looked like "a few slow requests", not a regression.
2. Checked the database first (wrong turn: the SQL view showed 3 ms). Wasted ~1 h.
3. Sampling profile of a slow request: 95% self time in `Buffer.BulkMoveWithWriteBarrierBatch` under `List<T>.Insert`. Found `Insert(0, …)` in the feed builder.

#### Root causes
Only one: front insertion into an array-backed list, quadratic in history length. It "worked" because test accounts had < 200 items.

##### Detection
A load test with a *realistic long-tail* of history sizes, or a budget on the p99 by account-size bucket. The p50 alert could never fire.

##### Fix
Append + one `Reverse()`. Confirmed: p99 back to 95 ms; a 600k-item synthetic account now builds in ~10 ms.

#### What I got wrong
"The database is usually the slow part." The SQL view had the answer in minutes (3 ms); I should have believed it.

#### Prevention
1. Budget in CI on feed-build time at n = 100k.
2. Alert on p99 latency per account-size bucket.
3. A code-review note: no `Insert(0, …)` on lists that can grow.

#### Earlier exercise it resembles
L01-02 (linear work inside a loop): the same "count x cost" shape.
