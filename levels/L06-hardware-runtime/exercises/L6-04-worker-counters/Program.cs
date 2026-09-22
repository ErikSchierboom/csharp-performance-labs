using WorkerCounters;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L6-04-worker-counters: Worker counters (false sharing)",
    Workload: Workload.Run,
    ExpectedChecksum: 40000000,
    MaxMedianMs: 369,
    MaxAllocatedMb: 1), args);
