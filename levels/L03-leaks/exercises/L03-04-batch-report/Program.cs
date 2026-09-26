using BatchReport;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L03-04-batch-report",
    Workload: Workload.Run,
    ExpectedChecksum: 111625,
    MaxMedianMs: 14,
    MaxAllocatedMb: 90,
    MaxGen2Collections: 10,
    MaxRetainedMb: 0), args);
