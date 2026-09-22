namespace ActivityFeed;

public record FeedItem(int Id, int Score);

public static class Feed
{
    // The feed shows the newest item first. Appending is O(1); inserting at index 0 shifts every element.
    // So append in arrival order, then reverse once at the end: O(n) instead of O(n^2).
    public static List<FeedItem> Build(int count)
    {
        var feed = new List<FeedItem>(count);
        for (int i = 0; i < count; i++)
            feed.Add(new FeedItem(i, i * 7 % 101));
        feed.Reverse();
        return feed;
    }
}

public static class Workload
{
    public static long Run()
    {
        var feed = Feed.Build(60_000);
        long checksum = 0;
        for (int i = 0; i < feed.Count; i += 97)
            checksum += feed[i].Id * 31L + feed[i].Score;
        return checksum * 1_000_003L + feed.Count;
    }
}
