# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Count how many *distinct* responses there are vs how many requests: 20 vs 1,200. That ratio is the cache's potential.
</details>

<details><summary>Hint 2: where?</summary>

Which layer could answer identical requests without running the handler?
</details>

<details><summary>Hint 3: why?</summary>

ASP.NET Core's **output caching** middleware stores whole responses keyed by the request and serves repeats without invoking the endpoint. It also collapses concurrent identical requests (resource locking) so the handler runs once per key per expiry.
</details>
