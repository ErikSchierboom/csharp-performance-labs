# L00-01 - Activity feed

*Training Wheels*

> **This is Lab 0: a complete, already-worked exercise.** Do it once *with* [`docs/worked-example/`](../../../../docs/worked-example/README.md)
> open beside you, to see what a finished attempt looks like: every template filled in. Then start Lab 1 and fill in your own.

## Symptom
Building an activity feed of 60,000 items takes about **190 ms**. The feed shows newest first, and the code
does exactly one simple operation per item. Allocation is small (about 2 MB) and the GC never runs. So it is not memory; something is burning CPU.

## Goal
Same feed (checksum), and:

| Budget | Value     |
|---|-----------|
| Median time | 29 ref-ms |
| Median allocated | 8 MB      |
