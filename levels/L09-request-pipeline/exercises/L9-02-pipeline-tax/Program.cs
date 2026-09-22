using PipelineTax;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L9-02-pipeline-tax: Pipeline tax (per-request work in middleware)",
    Workload: Workload.Run,
    ExpectedChecksum: 800002415445,
    MaxMedianMs: 66,
    MaxAllocatedMb: 20,
    ScaleTime: false,
    MaxP99Ms: 6), args);
