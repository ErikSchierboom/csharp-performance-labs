# L5-04 · HttpClient per call

## Symptom
Making 300 small requests to a service takes **~40 ms**, mostly *not* your code and *not* the server's work. The server reports **hundreds of separate TCP connections** for what is one caller. Against a real server over TLS, each new connection costs a network round trip or more.

## Goal
Same result (checksum), and:

| Budget | Value |
|---|---|
| Median time | 46 ref-ms |
| Median allocated | 8 MB |
| connections | ≤ 7 |

## Note
The exercise starts a tiny HTTP server on `127.0.0.1` inside the process and the harness counts the **distinct TCP connections** it sees (`connections`). Real servers are slower than loopback, so the real-world effect is bigger than what you'll measure.

## Extra credit
Run `ss -tan | grep -c TIME-WAIT` (Linux) before and after each version.
