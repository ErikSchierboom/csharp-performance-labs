namespace ContactImport;

public record ContactRow(string Email, string Name);
public record Contact(string Email, string Name);

public static class ContactImporter
{
    /// <summary>Imports rows, keeping the first occurrence of each email (case-insensitive).</summary>
    public static List<Contact> Import(IEnumerable<ContactRow> rows)
    {
        var seen = new List<string>();
        var result = new List<Contact>();

        foreach (var row in rows)
        {
            var email = row.Email.Trim();

            if (seen.Any(s => string.Equals(s, email, StringComparison.OrdinalIgnoreCase)))
                continue;

            seen.Add(email);
            result.Add(new Contact(email.ToLowerInvariant(), row.Name.Trim()));
        }

        return result;
    }
}

public static class Workload
{
    public static long Run()
    {
        var rows = CreateRows(20_000);
        var contacts = ContactImporter.Import(rows);

        long sum = contacts.Count;
        foreach (var c in contacts) sum += c.Name.Length + c.Email.Length;
        return sum;
    }

    static List<ContactRow> CreateRows(int n)
    {
        var rng = new Random(7);
        var rows = new List<ContactRow>(n);
        for (int i = 0; i < n; i++)
        {
            int id = rng.Next(0, 12_000);            // ~12k distinct people, lots of repeats
            var email = $"user{id}@example.com";
            if (rng.Next(3) == 0) email = email.ToUpperInvariant();   // same address, different casing
            rows.Add(new ContactRow("  " + email + " ", $" Person {id} "));
        }
        return rows;
    }
}
