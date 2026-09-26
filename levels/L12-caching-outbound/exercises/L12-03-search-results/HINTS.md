# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Kept-after-GC in the harness; dotMemory: one large `MemoryCache` entries dictionary.
</details>

<details><summary>Hint 2: where?</summary>

What limits the number of entries in the cache, and what removes an entry?
</details>

<details><summary>Hint 3: why?</summary>

`MemoryCache` has **no size limit and no default expiration**: entries stay until you say otherwise. Set `SizeLimit` on the cache *and* a size per entry (`SetSize`), plus an expiration. When the limit is reached, new entries are not added (or older ones compact away) instead of growing.
</details>
