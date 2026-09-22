# L7-01 · Checkout service

## Symptom
At launch, 60 checkout requests arrive together. Each has 20 line items priced from a cache backed by a 3 ms database. The **slowest checkout takes about 0.4 seconds**, the CPU is mostly idle, and the database itself is nowhere near busy. Traffic in production will be much higher.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 15 ref-ms |
| Median allocated | 2 MB |
| Median p99 latency | 15 ref-ms |

## Boss fight rules
- **Symptom only.** There are several defects and fixing one usually exposes the next; re-measure after every change.
- Hints are deliberately generic. Use `templates/POSTMORTEM.md` and write the post-mortem *before* you read the solution.
- Budgets are on several metrics at once; hitting one is not enough.

## Extra credit
What happens when the database call *fails* for one SKU? Is a faulted `Lazy<Task>` cached forever in your fix? Design the behaviour you want.
