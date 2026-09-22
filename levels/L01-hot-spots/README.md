# Level 1: obvious hot spots

The easy wins: code that's simply doing more work than it needs to, in ways a profiler makes obvious at a glance once you know where to look.

**Skills:** Read a call tree; self vs. total time; sampling vs. tracing; budgets as a regression gate.

**Mastery checkpoint:** Find the top self-time frame and its first "your code" caller in under 5 minutes, and say whether the problem is CPU, allocation or call count.

Run one: `dotnet run -c Release --project levels/L01-hot-spots/exercises/<id>` (expect `FAIL`), work it as described in the [top-level README](../../README.md), and only then open the solution.

**Further reading:** [Level 1 reading list](../../docs/READING-LIST.md#level-1-obvious-hot-spots)

## Exercises
| Exercise | Topic | Spoiler |
|---|---|---|
| [L1-01-invoice-export](exercises/L1-01-invoice-export/README.md) | Invoice export | [solution](solutions/L1-01-invoice-export/SOLUTION.md) |
| [L1-02-contact-import](exercises/L1-02-contact-import/README.md) | Contact import | [solution](solutions/L1-02-contact-import/SOLUTION.md) |
| [L1-03-log-classifier](exercises/L1-03-log-classifier/README.md) | Log classifier | [solution](solutions/L1-03-log-classifier/SOLUTION.md) |
| [L1-04-quantity-parsing](exercises/L1-04-quantity-parsing/README.md) | Quantity parsing | [solution](solutions/L1-04-quantity-parsing/SOLUTION.md) |
| [L1-05-customer-dashboard](exercises/L1-05-customer-dashboard/README.md) | Customer dashboard | [solution](solutions/L1-05-customer-dashboard/SOLUTION.md) |
| [L1-06-leaderboard](exercises/L1-06-leaderboard/README.md) | Leaderboard (sorting inside a loop) | [solution](solutions/L1-06-leaderboard/SOLUTION.md) |
| [L1-07-field-reader](exercises/L1-07-field-reader/README.md) | Field reader (reflection in a hot loop) | [solution](solutions/L1-07-field-reader/SOLUTION.md) |

## Final boss fight
A disguised combination of this level's defects: no per-defect hints. Do it last, then write down which exercise each defect came from.

| Exercise | Topic | Spoiler |
|---|---|---|
| [L1-boss-order-ledger](exercises/L1-boss-order-ledger/README.md) | Order ledger (final boss of Level 1) | [solution](solutions/L1-boss-order-ledger/SOLUTION.md) |
