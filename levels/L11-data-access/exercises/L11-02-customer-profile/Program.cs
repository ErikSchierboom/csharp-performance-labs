using CustomerProfile;
using PerfLab.Harness;
using PerfLab.Harness.Data;

return Lab.Run(new LabSpec(
    Name: "L11-02-customer-profile",
    Workload: Workload.Run,
    ExpectedChecksum: 80000241823,
    MaxMedianMs: 50,
    MaxAllocatedMb: 35,
    MaxP99Ms: 5,
    MaxMetrics: new()
    {
        [DbMetrics.RowsPerRequest] = 41, 
        [DbMetrics.CommandsPerRequest] = 3
    }), args);
