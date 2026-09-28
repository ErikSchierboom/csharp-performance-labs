namespace Leaderboard;

public record Score(int Player, int Points);

public static class Workload
{
    public static long Run()
    {
        var rng = new Random(31);
        var scores = new List<Score>();
        Score? leader = null;                                          // all we ever read is the leader: track it
        long checksum = 0;
        for (int i = 0; i < 6_000; i++)
        {
            var s = new Score(i, rng.Next(1_000_000));
            scores.Add(s);
            if (leader is null || s.Points > leader.Points) leader = s;
            checksum += leader.Points % 1000;
        }
        return checksum;
    }
}
