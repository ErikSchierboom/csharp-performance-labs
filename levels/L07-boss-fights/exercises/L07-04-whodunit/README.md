# L07-04 - Whodunit

*It Wasn't the Butler*

## Symptom
A lookup loop checks 2 million keys against an index of 50,000 entries and takes about **half a second**: over 200 ns per lookup, for a dictionary...

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 15 ref-ms |
| Median allocated | 0 MB |

## Boss fight rules
- **Symptom only.** There are several defects and fixing one usually exposes the next; re-measure after every change.
- Hints are deliberately generic. Use `templates/POSTMORTEM.md` and write the post-mortem *before* you read the solution.
