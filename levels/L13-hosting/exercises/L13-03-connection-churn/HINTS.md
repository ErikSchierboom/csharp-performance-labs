# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Count distinct connections (`connections`), `ss -tan | grep -c TIME-WAIT`, and response headers (`curl -v`).
</details>

<details><summary>Hint 2: where?</summary>

What header decides whether the client may reuse the connection, and who sets it?
</details>

<details><summary>Hint 3: why?</summary>

HTTP/1.1 connections are persistent by default. A `Connection: close` header (added by middleware, a proxy or a load balancer setting) forces one request per connection. Handshakes and slow-start dominate small responses. Remove it (and check what sits in front of you: LB idle timeouts must be *longer* than Kestrel's keep-alive).
</details>
