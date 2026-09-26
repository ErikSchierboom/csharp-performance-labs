# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

EF command logging (`LogTo`), the SQL view, or OpenTelemetry traces: count statements *per request*. The harness prints the run total.
</details>

<details><summary>Hint 2: where?</summary>

Divide `sqlCommands` by the number of requests. What number do you get, and what in the handler produces it?
</details>

<details><summary>Hint 3: why?</summary>

N+1 again (L05-01), but now every request pays it and concurrency multiplies the database load. One aggregate query returns the same answer.
</details>
