using OrderLines;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L2-02 Order lines",
    Workload: Workload.Run,
    ExpectedChecksum: 29484049425886904,
    MaxMedianMs: 65,
    MaxAllocatedMb: 4), args);
