using System.Data.Common;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace PerfLab.Harness.Data;

/// <summary>
/// Counts SQL commands (queries and non-queries) executed through EF Core, for the data-access exercises
/// (Level 5, Level 11+). Register once per <c>DbContextOptionsBuilder</c> via <c>.AddInterceptors(new CommandCounter())</c>.
///
/// Two independent views onto the same events, because different exercises want different things:
/// <list type="bullet">
/// <item><description><see cref="Total"/> - a single running total across an entire workload (e.g. every request in
/// a load-test batch). Safe only when nothing else is querying the database concurrently with your reset:
/// call <see cref="Reset"/> once before the batch, then read <see cref="Total"/> once after it.</description></item>
/// <item><description><see cref="Get"/> - the command count for one specific <see cref="DbContext"/> instance.
/// Use this under concurrent load (e.g. <c>WebRig.Drive</c>): each request creates its own short-lived
/// <c>DbContext</c> (see <c>Db.Create()</c> in these exercises), so keying by that instance gives each request its
/// own count with no reset race between concurrent requests.</description></item>
/// </list>
/// </summary>
public sealed class CommandCounter : DbCommandInterceptor
{
    private static int _total;

    /// <summary>The running total since the last <see cref="Reset"/>, across every <see cref="DbContext"/>.</summary>
    public static int Total => Volatile.Read(ref _total);

    /// <summary>Zeroes <see cref="Total"/>. Call once before a single-threaded (or otherwise exclusive) batch of work.</summary>
    public static void Reset() => Volatile.Write(ref _total, 0);

    private sealed class Box { public int Count; }
    private static readonly ConditionalWeakTable<DbContext, Box> PerContext = new();

    /// <summary>The number of commands executed by this <paramref name="ctx"/> instance so far. Safe under concurrent requests.</summary>
    public static int Get(DbContext ctx) => PerContext.TryGetValue(ctx, out var b) ? b.Count : 0;

    // Hooking the *Executed (after-the-fact) family, not *Executing: EF Core's async query pipeline
    // (ToListAsync/SumAsync/SingleAsync, which is what these exercises use) calls the async interceptor methods, and
    // the *Executing family only short-circuits/replaces a command, so a sync-only override silently never fires and
    // undercounts. The *Executed family doesn't have that split purpose, so a plain count here is simpler and correct
    // for both sync and async callers.
    public override DbDataReader ReaderExecuted(DbCommand c, CommandExecutedEventData e, DbDataReader r) { Count(e.Context); return r; }
    public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand c, CommandExecutedEventData e, DbDataReader r, CancellationToken ct = default) { Count(e.Context); return new(r); }
    public override int NonQueryExecuted(DbCommand c, CommandExecutedEventData e, int r) { Count(e.Context); return r; }
    public override ValueTask<int> NonQueryExecutedAsync(DbCommand c, CommandExecutedEventData e, int r, CancellationToken ct = default) { Count(e.Context); return new(r); }

    private static void Count(DbContext? ctx)
    {
        Interlocked.Increment(ref _total);
        if (ctx != null) PerContext.GetValue(ctx, _ => new Box()).Count++;
    }
}

/// <summary>
/// Counts rows actually read off the wire, per <see cref="DbContext"/> instance (see <see cref="Get"/>), for the
/// data-access exercises. Register once per <c>DbContextOptionsBuilder</c> via <c>.AddInterceptors(new RowCounter())</c>.
///
/// EF folds the duplicated parent columns of a multi-collection <c>Include</c> back into distinct entities when it
/// materialises the result, so the object graph you get back looks the same size whether the query returned 40 rows
/// or 400 (a cartesian explosion). Counting <c>Read()</c> calls on the raw <see cref="DbDataReader"/>, below EF's
/// materialisation, is the only way to see that.
///
/// Keyed by <see cref="DbContext"/> instance rather than an <c>AsyncLocal</c>: EF's async reader pipeline suspends
/// across several awaits below a query's outer <c>await</c>, and an <c>AsyncLocal</c> write made under one of those
/// suspended awaits is not visible once the outer await resumes, so it under-counts. A per-request <c>DbContext</c>
/// (see <c>Db.Create()</c> in these exercises) has no such problem: it is a plain object reference threaded through
/// by EF itself on every command.
/// </summary>
public sealed class RowCounter : DbCommandInterceptor
{
    private sealed class Box { public int Count; }
    private static readonly ConditionalWeakTable<DbContext, Box> Counts = new();

    /// <summary>The number of rows read for this <paramref name="ctx"/> instance so far.</summary>
    public static int Get(DbContext ctx) => Counts.TryGetValue(ctx, out var b) ? b.Count : 0;

    private static DbDataReader Wrap(DbContext? ctx, DbDataReader r) => ctx == null ? r : new CountingReader(r, Counts.GetValue(ctx, _ => new Box()));

    public override DbDataReader ReaderExecuted(DbCommand c, CommandExecutedEventData e, DbDataReader r) => Wrap(e.Context, r);
    public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand c, CommandExecutedEventData e, DbDataReader r, CancellationToken ct = default) => new(Wrap(e.Context, r));

    // A pass-through DbDataReader wrapper: every member forwards to the real reader except Read/ReadAsync, which
    // also increment the per-request count. This is what "counting rows below EF" costs: DbDataReader has no partial
    // interface, so every abstract member needs an override even though only two of them do anything.
    private sealed class CountingReader(DbDataReader r, Box box) : DbDataReader
    {
        public override bool Read() { var ok = r.Read(); if (ok) box.Count++; return ok; }
        public override async Task<bool> ReadAsync(CancellationToken ct) { var ok = await r.ReadAsync(ct); if (ok) box.Count++; return ok; }
        public override int Depth => r.Depth;
        public override int FieldCount => r.FieldCount;
        public override bool HasRows => r.HasRows;
        public override bool IsClosed => r.IsClosed;
        public override int RecordsAffected => r.RecordsAffected;
        public override object this[int i] => r[i];
        public override object this[string name] => r[name];
        public override string GetDataTypeName(int i) => r.GetDataTypeName(i);
        public override System.Collections.IEnumerator GetEnumerator() => r.GetEnumerator();
        public override Type GetFieldType(int i) => r.GetFieldType(i);
        public override string GetName(int i) => r.GetName(i);
        public override int GetOrdinal(string name) => r.GetOrdinal(name);
        public override bool GetBoolean(int i) => r.GetBoolean(i);
        public override byte GetByte(int i) => r.GetByte(i);
        public override long GetBytes(int i, long fo, byte[]? buf, int bo, int len) => r.GetBytes(i, fo, buf, bo, len);
        public override char GetChar(int i) => r.GetChar(i);
        public override long GetChars(int i, long fo, char[]? buf, int bo, int len) => r.GetChars(i, fo, buf, bo, len);
        public override DateTime GetDateTime(int i) => r.GetDateTime(i);
        public override decimal GetDecimal(int i) => r.GetDecimal(i);
        public override double GetDouble(int i) => r.GetDouble(i);
        public override float GetFloat(int i) => r.GetFloat(i);
        public override Guid GetGuid(int i) => r.GetGuid(i);
        public override short GetInt16(int i) => r.GetInt16(i);
        public override int GetInt32(int i) => r.GetInt32(i);
        public override long GetInt64(int i) => r.GetInt64(i);
        public override string GetString(int i) => r.GetString(i);
        public override object GetValue(int i) => r.GetValue(i);
        public override int GetValues(object[] values) => r.GetValues(values);
        public override bool IsDBNull(int i) => r.IsDBNull(i);
        public override bool NextResult() => r.NextResult();
    }
}
