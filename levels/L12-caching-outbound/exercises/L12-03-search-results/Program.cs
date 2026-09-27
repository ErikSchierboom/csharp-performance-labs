using SearchResults;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L12-03-search-results",
    Workload: Workload.Run,
    ExpectedChecksum: 600013800000,
    MaxMedianMs: 50,
    MaxAllocatedMb: 70,
    MaxRetainedMb: 5,
    MaxP99Ms: 1,
    Reset: Workload.Reset), args);
