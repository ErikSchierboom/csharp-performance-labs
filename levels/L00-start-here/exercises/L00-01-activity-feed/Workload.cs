namespace ActivityFeed;

public record FeedItem(int Id, int Score);

public static class Feed
{
    // The feed shows the newest item first, so each new item goes to the front.
    public static List<FeedItem> Build(int count)
    {
        var feed = new List<FeedItem>();
        for (var i = 0; i < count; i++)
            feed.Insert(0, new FeedItem(i, i * 7 % 101));
        return feed;
    }
}

public static class Workload
{
    public static long Run()
    {
        var feed = Feed.Build(60_000);
        long checksum = 0;
        for (var i = 0; i < feed.Count; i += 97)
            checksum += feed[i].Id * 31L + feed[i].Score;
        return checksum * 1_000_003L + feed.Count;
    }
}
