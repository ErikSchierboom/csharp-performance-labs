using OrderLedger;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L01-boss-order-ledger",
    Workload: Workload.Run,
    ExpectedChecksum: 9648128162,
    MaxMetrics: new() { [Metrics.Time] = 8, [Metrics.Alloc] = 9 }), args);
