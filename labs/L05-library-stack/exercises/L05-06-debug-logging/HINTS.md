# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

dotMemory allocations: `string` and `DefaultInterpolatedStringHandler`/`char[]` from the log call site, even though no log output exists.
</details>

<details><summary>Hint 2: where?</summary>

The argument to `LogDebug` is an *interpolated string*. When is it evaluated: before the call, or only if the level is enabled?
</details>

<details><summary>Hint 3: why?</summary>

C# evaluates and formats arguments **before** the method runs, so `LogDebug($"…{x}…")` builds the string even when Debug is off. A *message template* with arguments defers formatting; `LoggerMessage` (or the `[LoggerMessage]` source generator) also avoids boxing and checks `IsEnabled` first. Which gives zero allocation when disabled?
</details>
