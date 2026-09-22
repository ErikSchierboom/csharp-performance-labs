# L5-05 · JSON response

## Symptom
Serialising 5,000 orders into a response body allocates about **10× the size of the body** (5 MB of `string`s for a ~0.5 MB payload) and takes noticeably longer than it should. The output is one byte array; the intermediate strings are pure overhead.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 11 ref-ms |
| Median allocated | 2 MB |

## Extra credit
Why must the output of your fix be byte-identical to the original? What could differ between reflection and source-generated serialisation (naming policy, `JsonSerializerOptions`)?
