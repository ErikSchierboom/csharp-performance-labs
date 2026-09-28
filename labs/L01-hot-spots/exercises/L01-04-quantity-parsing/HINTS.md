# Hints (open one at a time)

<details><summary>Hint 1: which tool?</summary>

Sampling profile in Release. Follow the hottest path down from `ParseAll`. Ignore the data-generation code;
look at what happens *inside* the call your loop makes for each cell.
</details>

<details><summary>Hint 2: where?</summary>

Frames belonging to the runtime's exception machinery (throwing, stack-trace capture, unwinding, catch
dispatch) appear under the parse call. Names vary a little between .NET versions. Also compare the
"allocated MB" in the harness with how little data there is.
</details>

<details><summary>Hint 3: why?</summary>

Throwing an exception is orders of magnitude more expensive than returning a value, and it's happening for
roughly every second cell. Is "this cell isn't a number" an exceptional situation here, or an expected one?
Look at the "Try…" pattern in the BCL.
</details>
