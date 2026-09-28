# L06-02 - Particle sweep

*Particle Physics*

## Symptom
Visiting 2 million particles and summing five fields takes **~20 ms**: about 10 ns per particle, which is far more than five loads and a few multiplies. There is no allocation in the loop.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 5 ref-ms |
| Median allocated | 0 MB |
