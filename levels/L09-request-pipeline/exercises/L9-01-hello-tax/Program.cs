using HelloTax;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L9-01-hello-tax: Hello tax (what does a free endpoint cost?)",
    Workload: Workload.Run,
    ExpectedChecksum: 800002476000,
    MaxMedianMs: 42,
    MaxAllocatedMb: 14,
    ScaleTime: false,
    MaxP99Ms: 6), args);
