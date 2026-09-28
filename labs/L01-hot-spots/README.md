# Lab 1: obvious hot spots

The easy wins: code that's simply doing more work than it needs to, in ways a profiler makes obvious at a glance once you know where to look.

**Skills:** Read a call tree; self vs. total time; sampling vs. tracing; budgets as a regression gate.

**Mastery checkpoint:** Find the top self-time frame and its first "your code" caller in under 5 minutes, and say whether the problem is CPU, allocation or call count.

Run one: `dotnet run -c Release --project labs/L01-hot-spots/exercises/<id>` (expect `FAIL`), work it as described in the [top-level README](../../README.md), and only then open the solution.

**Further reading:** [Lab 1 reading list](../../docs/READING-LIST.md#lab-1-obvious-hot-spots)

## Exercises
| Exercise | Scenario | Spoiler |
|---|---|---|
| [L01-01-invoice-export](exercises/L01-01-invoice-export/README.md) | Invoice export | [solution](solutions/L01-01-invoice-export/SOLUTION.md) |
| [L01-02-contact-import](exercises/L01-02-contact-import/README.md) | Contact import | [solution](solutions/L01-02-contact-import/SOLUTION.md) |
| [L01-03-log-classifier](exercises/L01-03-log-classifier/README.md) | Log classifier | [solution](solutions/L01-03-log-classifier/SOLUTION.md) |
| [L01-04-quantity-parsing](exercises/L01-04-quantity-parsing/README.md) | Quantity parsing | [solution](solutions/L01-04-quantity-parsing/SOLUTION.md) |
| [L01-05-customer-dashboard](exercises/L01-05-customer-dashboard/README.md) | Customer dashboard | [solution](solutions/L01-05-customer-dashboard/SOLUTION.md) |
| [L01-06-leaderboard](exercises/L01-06-leaderboard/README.md) | Leaderboard | [solution](solutions/L01-06-leaderboard/SOLUTION.md) |
| [L01-07-field-reader](exercises/L01-07-field-reader/README.md) | Field reader | [solution](solutions/L01-07-field-reader/SOLUTION.md) |

## Final boss fight
A disguised combination of this lab's defects: no per-defect hints. Do it last, then write down which exercise each defect came from.

| Exercise | Scenario | Spoiler |
|---|---|---|
| [L01-boss-order-ledger](exercises/L01-boss-order-ledger/README.md) | Order ledger | [solution](solutions/L01-boss-order-ledger/SOLUTION.md) |
