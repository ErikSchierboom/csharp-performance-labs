using System.Text;

namespace AuditLog;

public static class Workload
{
    public static long Run()
    {
        string path = Path.Combine(Path.GetTempPath(), "perflab-audit-" + Environment.ProcessId + ".log");
        if (File.Exists(path)) File.Delete(path);
        using (var writer = new StreamWriter(path, append: false, new UTF8Encoding(false), bufferSize: 64 * 1024))
        {
            for (int i = 0; i < 20_000; i++)
                writer.Write($"{i:D6}|user-{i % 50}|action-{i % 9}\n");                  // buffered: many writes per system call
        }
        long length = new FileInfo(path).Length;
        File.Delete(path);
        return length;
    }
}
