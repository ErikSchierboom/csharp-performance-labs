# Level 13: hosting & deployment

The stuff that doesn't show up until you actually deploy the thing: connection handling, logging on the request path, GC configuration.

**Skills:** Connection behaviour and telemetry cost.

**Mastery checkpoint:** Justify each hosting setting with a measurement.

Run one: `dotnet run -c Release --project levels/L13-hosting/exercises/<id>` (expect `FAIL`), work it as described in the [top-level README](../../README.md), and only then open the solution.

**Further reading:** [Level 13 reading list](../../docs/READING-LIST.md#level-13-hosting-runtime-config-deployment)

## Exercises
| Exercise | Topic | Spoiler |
|---|---|---|
| [L13-03-connection-churn](exercises/L13-03-connection-churn/README.md) | Connection churn (`Connection: close`) | [solution](solutions/L13-03-connection-churn/SOLUTION.md) |
| [L13-04-log-sink](exercises/L13-04-log-sink/README.md) | Log sink (synchronous logging on the request path) | [solution](solutions/L13-04-log-sink/SOLUTION.md) |

