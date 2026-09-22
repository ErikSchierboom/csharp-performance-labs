# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Connections seen by the callee (`connections`), or `ss -tan | grep -c TIME-WAIT`; dotTrace: handler and socket construction per request.
</details>

<details><summary>Hint 2: where?</summary>

What object does each `new HttpClient()` create underneath, and what does it own?
</details>

<details><summary>Hint 3: why?</summary>

A client owns a handler with a **connection pool**; a new client is a new pool and a new connection. Use `IHttpClientFactory` (typed/named clients) or a long-lived client with `PooledConnectionLifetime` so connections are reused and DNS changes are still honoured.
</details>
