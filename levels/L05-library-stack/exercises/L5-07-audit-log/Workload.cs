using System.Text;

namespace AuditLog;

public static class Workload
{
    public static long Run()
    {
        string path = Path.Combine(Path.GetTempPath(), "perflab-audit-" + Environment.ProcessId + ".log");
        if (File.Exists(path)) File.Delete(path);
        for (int i = 0; i < 20_000; i++)
            File.AppendAllText(path, $"{i:D6}|user-{i % 50}|action-{i % 9}\n");      // open, write, close: every time
        long length = new FileInfo(path).Length;
        File.Delete(path);
        return length;
    }
}
