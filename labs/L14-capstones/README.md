# Lab 14: production capstones

Everything, stacked, under load: as close to a real incident as this repo gets. Write the post-mortem like it actually happened to you.

**Skills:** Stacked failures, each hiding the next; write the post-mortem.

**Mastery checkpoint:** Run a load test you can defend.

Run one: `dotnet run -c Release --project labs/L14-capstones/exercises/<id>` (expect `FAIL`), work it as described in the [top-level README](../../README.md), and only then open the solution.

**Further reading:** [Lab 14 reading list](../../docs/READING-LIST.md#lab-14-production-diagnosis-capstones)

## Exercises
| Exercise | Scenario | Spoiler |
|---|---|---|
| [L14-01-black-friday](exercises/L14-01-black-friday/README.md) | Black Friday (capstone) | [solution](solutions/L14-01-black-friday/SOLUTION.md) |
| [L14-02-oom-killed](exercises/L14-02-oom-killed/README.md) | OOM-killed at 3 a.m. (capstone) | [solution](solutions/L14-02-oom-killed/SOLUTION.md) |

