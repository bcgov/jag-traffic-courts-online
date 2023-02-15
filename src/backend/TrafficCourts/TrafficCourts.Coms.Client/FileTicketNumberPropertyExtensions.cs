namespace TrafficCourts.Coms.Client;

public static class FileTicketNumberPropertyExtensions
{

    public static Coms.Client.File SetTicketNumber(this Coms.Client.File file, string ticketNumber)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(ticketNumber);

        file.Tags[FileProperty.TicketNumber] = ticketNumber;

        return file;
    }

    public static string? GetTicketNumber(this FileSearchResult file)
    {
        ArgumentNullException.ThrowIfNull(file);

        string? ticketNumber = GetProperty(FileProperty.TicketNumber, file.Tags);
        return ticketNumber;
    }

    public static string? GetTicketNumber(this Coms.Client.File file)
    {
        ArgumentNullException.ThrowIfNull(file);

        string? ticketNumber = GetProperty(FileProperty.TicketNumber, file.Tags);
        return ticketNumber;
    }

    public static string? GetTicketNumber(this Dictionary<string, string> properties)
    {
        ArgumentNullException.ThrowIfNull(properties);

        string? ticketNumber = GetProperty(FileProperty.TicketNumber, properties);
        return ticketNumber;
    }

    /// <summary>
    /// Gets the named property from the property set. If the value 
    /// </summary>
    /// <param name="name">The name of the property to find.</param>
    /// <param name="properties">The property set to look for the property.</param>
    /// <returns></returns>
    private static string? GetProperty(string name, Dictionary<string, string> properties)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(properties);

        properties.TryGetValue(name, out string? value);

        if (string.IsNullOrWhiteSpace(value))
        {
            value = null;
        }

        return value;
    }

}