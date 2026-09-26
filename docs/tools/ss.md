# ss
> Linux only

**Answers:** which TCP connections exist, and in which state? Use it when a service is slow to *connect* rather than slow to compute, or to see whether connections are reused or opened fresh each time.

`ss` ships with Linux (package `iproute2`). No install needed.

## Commands
```bash
ss -tan                                  # all TCP sockets (-t TCP, -a all states, -n numeric ports)
ss -tan state established                # only open connections
ss -Htan state time-wait | wc -l         # count sockets in TIME-WAIT (-H drops the header line)
ss -s                                    # totals per state
ss -tanp                                 # also show the owning process (pid)
```
Run a count **before** and **after** an exercise (or while it runs in profile mode) and compare.

## Reading it
- **ESTAB:** open connections. A client that reuses connections keeps a few of these open.
- **TIME-WAIT:** connections that were closed recently. The side that closes first keeps these for about a minute. Hundreds or thousands of them mean connections are being opened and closed at a high rate.
- The **peer address and port** tell you which side is which. The exercises' test servers listen on `127.0.0.1` with a random port.

## Traps
- Other programs on your machine have connections too. Filter by port (`ss -tan '( dport = :5000 or sport = :5000 )'`) or compare before and after.
- TIME-WAIT sockets outlive the process. Wait a minute between runs, or you'll count the previous run too.
- **Windows:** `netstat -an | findstr TIME_WAIT`, or in PowerShell `Get-NetTCPConnection -State TimeWait`.

## Docs
[ss man page](https://man7.org/linux/man-pages/man8/ss.8.html)
