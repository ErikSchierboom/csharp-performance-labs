namespace Leaderboard;

public record Score(int Player, int Points);

public static class Workload
{
    public static long Run()
    {
        var rng = new Random(31);
        var scores = new List<Score>();
        long checksum = 0;
        for (int i = 0; i < 6_000; i++)
        {
            scores.Add(new Score(i, rng.Next(1_000_000)));
            scores.Sort((a, b) => b.Points.CompareTo(a.Points));      // keep the board sorted...
            checksum += scores[0].Points % 1000;                      // ...so we can read the current leader
        }
        return checksum;
    }
}
