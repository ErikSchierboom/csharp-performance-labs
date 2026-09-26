# L05-06 - Debug logging

*Talking to Nobody*

## Symptom
The app logs at Debug level in a hot loop, but production only records Warning and above, so those log lines are **never written**. Yet 300,000 of them cost **~17 ms and ~41 MB of allocation**. The logger is off; the cost is not.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 18 ref-ms |
| Median allocated | 1 MB |
