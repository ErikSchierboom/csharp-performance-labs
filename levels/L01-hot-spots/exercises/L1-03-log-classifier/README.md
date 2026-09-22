# L1-03 · Log classifier

## Symptom
Parsing 40,000 log lines takes about **0.7 seconds** and allocates roughly **380 MB**. The parsing logic is
one regular-expression match per line. That shouldn't cost this much. The GC is busy (gen0 and gen1 counts
are non-zero in the harness output).

## Goal
Same parsed results (checksum), and:

| Budget | Value |
|---|---|
| Median time | 250 ref-ms |
| Median allocated | 100 MB |

## Extra credit
After you pass, try three different fixes and compare them (see the solution write-up for what to expect):
instance cached in a static field, `RegexOptions.Compiled`, and `[GeneratedRegex]`.
Also: what's still allocating after your fix, and why?
