# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotTrace on the server side: time in `Response.Body.FlushAsync` and per-call write overhead. On the wire, `ss`/`tcpdump` show many small segments; the harness shows the latency.
</details>

<details><summary>Hint 2: where?</summary>

Which two calls happen 300 times per request, and does the *client* benefit from the first byte arriving early here?
</details>

<details><summary>Hint 3: why?</summary>

Each `FlushAsync` forces the bytes onto the socket (a system call and a network segment) and each `WriteAsync` has fixed overhead. Streaming is right for long-running or huge responses; for a 9 KB body, build it and write it once. (Even better: write straight to `Response.BodyWriter` without building a string.)
</details>
