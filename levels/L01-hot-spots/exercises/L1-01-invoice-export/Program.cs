using InvoiceExport;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L1-01 Invoice export",
    Workload: Workload.Run,
    ExpectedChecksum: 267266806800,
    MaxMedianMs: 40,
    MaxAllocatedMb: 8), args);
