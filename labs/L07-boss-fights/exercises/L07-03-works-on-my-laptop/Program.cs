using WorksOnMyLaptop;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L07-03-works-on-my-laptop",
    Workload: Workload.Run,
    ExpectedChecksum: 305971328,
    MaxMetrics: new() { [Metrics.CommittedMb] = 30, [Metrics.WorkingSetMb] = 150, [Metrics.Time] = 300, [Metrics.Alloc] = 5000 }), args);
