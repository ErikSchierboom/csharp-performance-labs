# L01-01 - Invoice export

*A Gigabyte for a Spreadsheet*

## Symptom
The nightly job exports 5,000 orders to a CSV report. The file is only a few hundred KB, yet the run takes
a quarter of a second or more, and the harness reports **over a gigabyte allocated** and **hundreds of GCs**.
Something is doing far more work than the size of the output justifies.

## Goal
Pass both budgets **without changing the output** (the checksum must still match):

| Budget | Value     |
|---|-----------|
| Median time | 24 ref-ms |
| Median allocated | 8 MB      |

## Rules
- Edit `Workload.cs`. Don't edit `Program.cs` (budgets/checksum live there).
- Profile **before** you read the code for the answer. Write your hypothesis in `templates/LAB-LOG.md` first.
- Stuck? `HINTS.md`, one hint at a time. Solution: `levels/L01-hot-spots/solutions/L01-01-invoice-export/` (only after you pass).
