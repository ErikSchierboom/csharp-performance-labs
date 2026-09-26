# L01-05 - Customer dashboard

*The Numbers Add Up (Slowly)*

## Symptom
Building a summary for 40,000 customers takes about **150 ms**. Reading the code, every step happens once:
filter the active customers, score them, count them, sum the scores, find the top one. Nothing allocates much
and there are no obvious hot loops, but it's roughly 3x slower than the amount of "real work" suggests.

## Goal
Same summary (checksum), and:

| Budget | Value     |
|---|-----------|
| Median time | 50 ref-ms |
| Median allocated | 5 MB      |

## Note
This one is deliberately hard to see in a *sampling* profile: nothing is "wrong" with any single frame. It's a
good exercise in choosing the right profiler mode. Try both and compare what each tells you.
