using BulkFetch;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L04-03-bulk-fetch",
    Workload: Workload.Run,
    ExpectedChecksum: 999000,
    MaxMedianMs: 150,
    MaxAllocatedMb: 1,
    ScaleTime: false,
    MaxMetrics: new() { [Metrics.PeakInFlight] = 50 }), args);
