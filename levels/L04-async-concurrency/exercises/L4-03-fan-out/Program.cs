using FanOut;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L4-03-fan-out: Fan-out",
    Workload: Workload.Run,
    ExpectedChecksum: 999000,
    MaxMedianMs: 495,
    MaxAllocatedMb: 2,
    ScaleTime: false,
    MaxMetrics: new() { ["peakInflight"] = 76 }), args);
