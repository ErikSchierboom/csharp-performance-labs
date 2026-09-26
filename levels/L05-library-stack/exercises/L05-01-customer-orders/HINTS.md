# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

The SQL view (or EF's command logging) shows *count* and *total time* per statement. Sort by count. Sampling alone would show only time spread thinly across EF plumbing.
</details>

<details><summary>Hint 2: where?</summary>

The same statement, differing only in a parameter, is executed ~500 times. Find the loop in `Run` that causes it.
</details>

<details><summary>Hint 3: why?</summary>

Each iteration asks the database one small question, so you pay per-query overhead 500 times (parsing, network round trip on a real server). Can one query return the totals for *all* customers? (A projection with an aggregate, or `GroupBy`, or `Include`.)
</details>
