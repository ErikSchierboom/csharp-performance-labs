# L09-01 - Solution

## What the profile shows
- **Allocation by type:** controller activation, `ActionContext`/filter pipeline objects, formatter and result objects per request in the MVC version; a small handful in the minimal endpoint.

## Root cause
Pipeline features you don't use still run: MVC's controller activation, filter pipeline and JSON output formatting are per-request work with per-request allocation.

## Fix
A minimal endpoint returning a fixed result (`Results.Text` here). Don't take this as 'never use MVC': use it to **calibrate** what a request costs, then judge each real endpoint against that floor.

## Take-aways
1. **Know the floor.** Measure the empty endpoint first; every extra byte per request above it belongs to your code or a framework feature you chose.
2. Frameworks have a per-request tax proportional to the features in the pipeline; it's a budget line, not a bug.
3. Measure per request (total ÷ requests), not per run.
4. A harness that includes the client understates the server's share: keep that in mind when you read the numbers.

## Extra credit
Return the object from a minimal API (`Results.Ok(new { message = "hello" })`) instead of a pre-built string. Where does the difference come from?

## Go further
Add response compression, CORS, auth (a dummy scheme) and logging middleware one at a time. What does each add per request?
