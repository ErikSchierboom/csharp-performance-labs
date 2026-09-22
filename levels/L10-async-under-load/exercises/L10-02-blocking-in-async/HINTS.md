# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Counters + timeline as in L10-01. Then read the handler *line by line* and mark every line that doesn't `await`.
</details>

<details><summary>Hint 2: where?</summary>

`async` describes the method, not what's inside it. Which statement holds a thread for 15 ms?
</details>

<details><summary>Hint 3: why?</summary>

A synchronous wait (`Thread.Sleep`, `File.ReadAllText`, a sync database driver, `HttpClient.Send`) inside an async handler holds a pool thread just like `.Result` does. `await Task.Yield()` afterwards doesn't help. Use the async API for the same operation.
</details>
