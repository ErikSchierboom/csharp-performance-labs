# L04-boss - Notification hub

*You've Got Mail (Eventually)*

## Symptom
A notification service has three stages per batch: sessions register inboxes on a shared bus, workers record delivery statistics, and outgoing messages are queued for a slower sender. The batch is slow, **memory that should have been freed is still reachable after a full GC**, and **the queue grows to thousands of items**. Nothing in the three stages obviously interacts.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 113 ref-ms |
| Median allocated | 37 MB |
| maxQueued | ≤ 76 |
| Kept after a full GC | ≤ 1 MB |

## Final boss fight
This is the **final boss** of its lab: a **disguised combination** of defects from this lab, in a different domain. There are no per-defect hints. Profile it, list what you find, fix one thing at a time, and afterwards write down **which earlier exercise each defect came from** (the solution lists them). Passing means hitting *all* the budgets.

The harness gates time, allocation, **memory still reachable after a full GC**, and **queue depth**. Needs at least 4 cores.
