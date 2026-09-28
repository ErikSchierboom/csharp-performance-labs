# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

The harness's `maxQueued` shows the queue length. In dotMemory, the growth is `byte[]` reachable from the channel. Also look at the *rates*: how fast do items enter, and how fast do they leave?
</details>

<details><summary>Hint 2: where?</summary>

`Channel.CreateUnbounded` never pushes back on the producer. Which *option* changes that?
</details>

<details><summary>Hint 3: why?</summary>

With no back-pressure, a producer faster than the consumer just moves work into memory. A **bounded** queue makes the producer wait, so the slowest stage sets the pace and memory stays flat. What must the producer use to *wait* instead of failing when the queue is full?
</details>
