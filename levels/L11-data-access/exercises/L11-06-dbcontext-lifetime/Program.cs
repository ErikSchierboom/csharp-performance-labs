using ContextLifetime;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L11-06-dbcontext-lifetime: DbContext lifetime (a leak in disguise)",
    Workload: Workload.Run,
    ExpectedChecksum: 80000240800,
    MaxMedianMs: 163,
    MaxAllocatedMb: 166,
    MaxRetainedMb: 1,
    MaxP99Ms: 9,
    Reset: Workload.Reset,
    TimedWarmup: false), args);
