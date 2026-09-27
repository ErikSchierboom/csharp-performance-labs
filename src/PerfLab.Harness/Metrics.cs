namespace PerfLab.Harness;

/// <summary>
/// Named-metric strings reported via <see cref="Lab.Report"/> and gated via <see cref="LabSpec.MaxMetrics"/>, so the
/// string an exercise reports and the string its budget keys on can't drift apart, and so a name reused by more than
/// one exercise is spelled the same way everywhere. (The data-access exercises' own metrics, <c>sqlCommands</c> and
/// <c>rowsPerRequest</c>, live next to their counters in <c>PerfLab.Harness.Data.DbMetrics</c> instead.)
/// </summary>
public static class Metrics
{
    // These six are measured by the harness itself every run - never call Lab.Report for them; just gate them via
    // LabSpec.MaxMetrics like any other named metric. See LabSpec.MaxMetrics for what each one means.
    /// <summary>Built in: wall-clock time for the run. Scales with the machine factor.</summary>
    public const string Time = "time";
    /// <summary>Built in: bytes allocated during the run, in MB. Never scaled - allocation budgets are absolute and deterministic.</summary>
    public const string Alloc = "alloc";
    /// <summary>Built in: p99 latency across every <see cref="Lab.RecordLatency"/> call. Scales with the machine factor.</summary>
    public const string P99 = "p99";
    /// <summary>Built in: CPU time across all threads. Scales with the machine factor.</summary>
    public const string Cpu = "cpu";
    /// <summary>Built in: full (gen2) collections in the run.</summary>
    public const string Gen2 = "gen2";
    /// <summary>Built in: memory still reachable after a forced full GC, relative to just before the run.</summary>
    public const string Retained = "retained";

    /// <summary>Committed memory, MB (Level 7: <c>L07-03-works-on-my-laptop</c>).</summary>
    public const string CommittedMb = "committedMB";

    /// <summary>Concurrent outbound connections/handles held (Level 5 <c>L05-04-service-calls</c>; Level 12 <c>L12-01-downstream-call</c>, <c>L12-boss-catalog-service</c>; Level 13 <c>L13-01-short-responses</c>).</summary>
    public const string Connections = "connections";

    /// <summary>Calls made to a downstream/flaky dependency (Level 12: <c>L12-05-flaky-downstream</c>).</summary>
    public const string DownstreamCalls = "downstreamCalls";

    /// <summary>Calls made to an external service (Level 14: <c>L14-01-black-friday</c>).</summary>
    public const string ExternalCalls = "externalCalls";

    /// <summary>Calls into a value factory (Level 4: <c>L04-04-config-cache</c>).</summary>
    public const string FactoryCalls = "factoryCalls";

    /// <summary>Callers that gave up (e.g. a retry budget exhausted) (Level 12: <c>L12-05-flaky-downstream</c>; Level 14: <c>L14-01-black-friday</c>).</summary>
    public const string GiveUps = "giveUps";

    /// <summary>Loads performed (e.g. cache misses that fell through to the source) (Level 12: <c>L12-02-popular-items</c>).</summary>
    public const string Loads = "loads";

    /// <summary>Peak queue depth (Level 4: <c>L04-05-order-pipeline</c>, <c>L04-boss-notification-hub</c>).</summary>
    public const string MaxQueued = "maxQueued";

    /// <summary>Peak managed heap size, MB (Level 4: <c>L04-05-order-pipeline</c>).</summary>
    public const string PeakHeapMb = "peakHeapMb";

    /// <summary>Peak number of requests in flight at once (Level 10: <c>L10-05-impatient-clients</c>).</summary>
    public const string PeakInFlight = "peakInFlight";

    /// <summary>Peak number of background jobs outstanding (Level 10: <c>L10-06-background-jobs</c>).</summary>
    public const string PeakJobs = "peakJobs";

    /// <summary>Private (non-shareable) memory, MB (Level 7: <c>L07-02-phantom-leak</c>).</summary>
    public const string PrivateMb = "privateMB";

    /// <summary>Work steps that ran after the client had already given up (Level 10: <c>L10-05-impatient-clients</c>).</summary>
    public const string StepsAfterAbort = "stepsAfterAbort";

    /// <summary>Process working set, MB (Level 7: <c>L07-03-works-on-my-laptop</c>).</summary>
    public const string WorkingSetMb = "workingSetMB";
}
