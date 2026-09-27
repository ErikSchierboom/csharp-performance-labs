# L14-01 - Black Friday

*Doorbusters*

## Symptom
On launch morning, a wave of concurrent shoppers hits pages for a handful of hot products. The third-party pricing service's own dashboard shows far more traffic than the storefront should be sending it, and a real share of those calls fail outright. Latency during the surge is much worse than on a quiet morning. Every team's dashboard points at someone else's service.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 150 ref-ms |
| Median allocated | 2.3 MB |
| Median p99 latency | 100 ref-ms |
| externalCalls | ≤ 46 |
| giveUps | ≤ 1 |

## Capstone rules
Symptom only; several defects, each hiding the next. Write the post-mortem (`templates/POSTMORTEM.md`) **before** reading the solution. Hints are generic on purpose.
