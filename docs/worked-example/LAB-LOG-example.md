# Lab-log entry

Copy of the template from `templates/LAB-LOG.md`, completed for L00-01. Compare with your own after you've tried the exercise.

## L00-01 activity-feed, 2026-09-20

#### Prediction
Something quadratic, given "one simple operation per item". If I go from 60k to 600k items I expect roughly 100× the time, not 10×.

#### Baseline
187 ms / 2.4 MB (budget 50 ref-ms ≈ 30 ms on this machine = FAIL)

#### Profiler + mode
dotTrace, sampling, Release, `--profile --seconds 15`

#### What I saw
Almost all self time in `System.Buffer.BulkMoveWithWriteBarrierBatch`. Nothing in my own code has meaningful self time. Allocation is small and the GC never ran: not a memory problem.

#### Hypothesis
`Insert(0, x)` shifts all existing elements every call, so total work grows as n². The fix is to stop inserting at the front.

#### Change made
`Add` in arrival order and a single `Reverse()` at the end. Also pre-sized the list.

#### Result
0.6 ms / 1.8 MB → PASS

#### Suprise
I expected memory to matter (a "feed" sounds like a memory thing). Also: 187 ms -> 0.6 ms is ~300x, far more than my "n²" intuition alone gave me at 60k items. Moving 240 KB blocks of references 60,000 times isn't free.

#### Hints used
0

#### Could I explain it to someone else in 2 minutes?
Yes, a list is an array; inserting at the front moves everything; 60,000 times moves 1.8 billion slots.

#### Where would this bite in a real system?
Any "newest first" structure that grows without a cap: notification lists, chat history, undo stacks, log tails. It is invisible at 100 items and a wall at 100,000.

#### Break my own fix
Wrong when readers need the feed between inserts. Then a different data structure is needed.

#### Extra-credit check
600k items on the slow version took 43,210 ms vs 187 ms at 60k: 231x, not the ~100x a pure n² guess predicts.
Why the extra 2x: at 600k the array is ~4.8 MB, so each block move no longer fits in the CPU cache and runs at memory-bandwidth speed. Prediction was right in kind (quadratic), wrong in size: log that.