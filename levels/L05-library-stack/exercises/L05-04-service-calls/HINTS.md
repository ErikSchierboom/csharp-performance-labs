# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Look at the harness's `connections` metric, then at the *sockets*: `ss -tan` (Linux) shows a pile of `TIME-WAIT` entries. In dotTrace, look for connection setup frames (`ConnectAsync`, handler construction).
</details>

<details><summary>Hint 2: where?</summary>

Inside the loop, what is created every iteration, and what does that object own that is expensive?
</details>

<details><summary>Hint 3: why?</summary>

`HttpClient` wraps a handler that owns the connection pool. A new client means a new pool means a new connection (and a full handshake) per request; when many are disposed, sockets linger in `TIME_WAIT`, and at scale you run out of ports. Reuse one client (or `IHttpClientFactory`, which manages handler lifetime for you).
</details>
