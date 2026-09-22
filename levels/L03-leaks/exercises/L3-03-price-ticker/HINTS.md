# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotMemory snapshot compare, then **Key retention paths** on a surviving `Ticker`. The root will not be one of your fields.
</details>

<details><summary>Hint 2: where?</summary>

Follow the path from the root: it goes through the runtime's timer machinery. What did the `Ticker` constructor create that the runtime keeps a reference to?
</details>

<details><summary>Hint 3: why?</summary>

A running `System.Threading.Timer` is kept alive by the runtime until it is disposed or its period ends, and its callback (a lambda using `this`) references the `Ticker`. So dropping the last reference to the `Ticker` does not make it garbage. What must you call?
</details>
