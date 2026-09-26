# L03-06 - Solution

## What the profile shows
- **snapshots:** static `List<Func<int>>` → `Func<int>` delegate → closure display class → `byte[20000]`.

## Root cause
A long-lived delegate's closure holds a large local (`report`) that the callback only needed one element of.

## Fix
Copy the needed value into a small local and capture that. Also: unregister callbacks when their owner is done (L03-01), and prefer `static` lambdas with explicit state to avoid accidental capture.

## Take-aways
1. **A closure keeps everything it captures alive for as long as the delegate lives.**
2. Long-lived delegates (events, registries, timers, caches) are where this bites.
3. Capture minimal values; a `static` lambda cannot capture at all, which makes accidents a compile error.
4. Retention paths through `<>c__DisplayClass` are the fingerprint.

## Extra credit
What if the lambda captured `this` instead? What would be retained?

## Go further
Rewrite with a `static` lambda taking the value as an argument (`Func<int,int>` + state).
