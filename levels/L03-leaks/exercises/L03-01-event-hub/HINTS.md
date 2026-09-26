# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotMemory: snapshot before, run, snapshot after a forced GC, **Compare**. Sort by *new objects* or *retained size*. Which type grew by about 2,000?
</details>

<details><summary>Hint 2: where?</summary>

Open the retention path (*Key retention paths* / *Dominators*) for one surviving `Widget`. Read it from the GC root down. What is the first thing that is *not* your local variable?
</details>

<details><summary>Hint 3: why?</summary>

A `Hub` event is a delegate list; `+=` stores a delegate whose target is the subscriber. A publisher that outlives its subscribers keeps them all alive. What must a subscriber do when it's done, and how do you make that hard to forget?
</details>
