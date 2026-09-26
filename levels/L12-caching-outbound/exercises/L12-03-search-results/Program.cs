using SearchResults;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L12-03-search-results",
    Workload: Workload.Run,
    ExpectedChecksum: 600013800000,
    MaxMedianMs: 62,
    MaxAllocatedMb: 168,
    MaxRetainedMb: 9,
    MaxP99Ms: 4,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
