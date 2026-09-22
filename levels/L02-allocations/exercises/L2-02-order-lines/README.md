# L2-02 · Order lines

## Symptom
Parsing 200,000 order lines of the form `10423; eu ;17;19.99;Gift|FRAGILE` takes about **50 ms** and allocates about **106 MB**: roughly
530 bytes of garbage per line, for a result that is one small struct. The GC is busy (gen0 collections in every run).

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 65 ref-ms |
| Median allocated | 4 MB |

## Note on the input data
The workload's *input* is generated once, on first use. The harness warms up before it measures, so the input is not
counted as allocation. Only the algorithm you are fixing is. (Time is scaled to your machine; allocation is not.)

## Extra credit
Your parser must still treat `" eu "`, `"APAC"`, `"Gift"`, `" fragile"` and unknown flags the same way as before, and reject lines
with a wrong field count. Add three edge-case lines to the input in a scratch copy and check your version and the original agree. What about an empty flags field?
