# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotMemory compare: the growth is `Document` and its `byte[]`, but `Metadata` grew too. Open the retention path for a `Document`.
</details>

<details><summary>Hint 2: where?</summary>

The dominator is the tagging table. What is used as the **key**, and what does a `Dictionary` do with its keys?
</details>

<details><summary>Hint 3: why?</summary>

A `Dictionary<object,…>` holds its keys strongly, so a document can never be collected while it's a key. You want a table whose entries disappear when the *key object* becomes unreachable. Look at `ConditionalWeakTable<TKey,TValue>`.
</details>
