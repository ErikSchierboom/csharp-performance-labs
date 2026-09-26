using WorksOnMyLaptop;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L07-03-works-on-my-laptop",
    Workload: Workload.Run,
    ExpectedChecksum: 305971328,
    MaxMedianMs: 300,
    MaxAllocatedMb: 5000,
    MaxMetrics: new() { ["committedMB"] = 25, ["workingSetMB"] = 150 }), args);
