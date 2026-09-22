using BatchReport;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L3-04-batch-report: Batch report (a leak that isn't)",
    Workload: Workload.Run,
    ExpectedChecksum: 111625,
    MaxMedianMs: 33,
    MaxAllocatedMb: 201,
    MaxGen2Collections: 12,
    MaxRetainedMb: 1), args);
