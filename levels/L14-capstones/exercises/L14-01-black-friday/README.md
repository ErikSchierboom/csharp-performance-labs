# L14-01 - Black Friday

*Doorbusters*

## Symptom
On launch morning, product pages for 30 hot products get 60 concurrent shoppers. The third-party pricing service is hit about **7× more often than it should be** (207 calls for 30 products) and **fails for a quarter of them** (55 give-ups), and the database connection pool is held for the full length of every call. (In production, latency explodes as the retries pile up; at this scale the harness shows it in the counts.) Each component's dashboard blames another. The cache 'should' be absorbing this.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 245 ref-ms |
| Median allocated | 5 MB |
| Median p99 latency | 235 ref-ms |
| externalCalls | ≤ 46 |
| giveUps | ≤ 1 |

## Note (the ASP.NET Core levels (9–14) harness)
The exercise runs an ASP.NET Core server on loopback **inside the harness process** (`WebRig`) and drives it with virtual users. Allocation and CPU include the small constant client cost.

## Capstone rules
Symptom only; several defects, each hiding the next. Write the post-mortem (`templates/POSTMORTEM.md`) **before** reading the solution. Hints are generic on purpose.
