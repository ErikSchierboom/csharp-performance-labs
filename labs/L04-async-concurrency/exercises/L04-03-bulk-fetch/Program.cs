using BulkFetch;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L04-03-bulk-fetch",
    Workload: Workload.Run,
    ExpectedChecksum: 999000,
    ScaleTime: false,
    MaxMetrics: new() { [Metrics.PeakInFlight] = 50, [Metrics.Time] = 150, [Metrics.Alloc] = 1 }), args);
