namespace Leaderboard;

public static class Workload
{
    public static long Run()
    {
        var rng = new Random(31);
        var maxScore = 0;
        long checksum = 0;
        for (int i = 0; i < 6_000; i++)
        {
            var points = rng.Next(1_000_000);
            maxScore = Math.Max(maxScore, points);
            checksum += maxScore % 1000;
        }
        return checksum;
    }
}
