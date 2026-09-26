using OrderLedger;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L01-boss-order-ledger",
    Workload: Workload.Run,
    ExpectedChecksum: 9648128162,
    MaxMedianMs: 5,
    MaxAllocatedMb: 9), args);
