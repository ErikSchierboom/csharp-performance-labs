using LogSink;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L13-04-log-sink: Log sink (synchronous logging on the request path)",
    Workload: Workload.Run,
    ExpectedChecksum: 300000905445,
    MaxMedianMs: 53,
    MaxAllocatedMb: 25,
    MaxP99Ms: 7), args);
