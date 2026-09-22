using ChattyWriter;
using PerfLab.Harness;

return Lab.Run(new LabSpec(
    Name: "L9-03-chatty-writer: Chatty writer (many small writes)",
    Workload: Workload.Run,
    ExpectedChecksum: 300014235000,
    MaxMedianMs: 106,
    MaxAllocatedMb: 237,
    MaxP99Ms: 8), args);
