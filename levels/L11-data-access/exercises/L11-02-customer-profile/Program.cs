using CustomerProfile;
using PerfLab.Harness;
using PerfLab.Harness.Data;

return Lab.Run(new LabSpec(
    Name: "L11-02-customer-profile",
    Workload: Workload.Run,
    ExpectedChecksum: 80000241823,
    MaxMetrics: new() { [DbMetrics.RowsPerRequest] = 41, [DbMetrics.CommandsPerRequest] = 3, [Metrics.P99] = 5, [Metrics.Time] = 50, [Metrics.Alloc] = 35 }), args);
