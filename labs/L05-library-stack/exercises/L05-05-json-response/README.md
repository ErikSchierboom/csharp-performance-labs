# L05-05 - JSON response

*Return to Sender*

## Symptom
The `GET /orders?page=N` endpoint returns 50 orders per page as JSON (~5 KB per response). Serving all 100 pages takes **~35 ms and allocates ~8.6 MB**, about **17x the bytes actually sent**. The handler does nothing but serialise a DTO, so the cost is all in how it uses `System.Text.Json`.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 5 ref-ms |
| Median allocated | 1 MB |

## Note
`ChecksumStream` stands in for the network. It fingerprints the response bytes, so your output must stay **byte-identical**: same property names, same enum strings, same omitted nulls.
