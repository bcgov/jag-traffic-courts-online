namespace TrafficCourts.Coms.Client;

public static class FileVirusScanPropertyExtensions
{
    public static bool VirusScanIsClean(this Coms.Client.File file) => VirusScanIsStatus(file, "clean");
    public static bool VirusScanIsInfected(this Coms.Client.File file) => VirusScanIsStatus(file, "infected");
    public static bool VirusScanIsError(this Coms.Client.File file) => VirusScanIsStatus(file, "error");

    public static string GetVirusScanStatus(this Coms.Client.File file)
    {
        ArgumentNullException.ThrowIfNull(file);

        var status = GetProperty(FileProperty.VirusScanStatus, file.Tags);
        if (status is null)
        {
            // get this property from metadata 
            status = GetProperty(FileProperty.VirusScanStatus, file.Metadata);
            if (status is null) status = "undefined";
        }

        return status;
    }

    public static void SetVirusScanClean(this Coms.Client.File file)
    {
        ArgumentNullException.ThrowIfNull(file);

        file.Tags[FileProperty.VirusScanStatus] = "clean";
        // In case the document re-scanned and it used to be infected then remove virus-name
        file.Tags.Remove(FileProperty.VirusName);
    }

    public static void SetVirusScanInfected(this Coms.Client.File file, string virusName)
    {
        ArgumentNullException.ThrowIfNull(file);

        file.Tags[FileProperty.VirusScanStatus] = "infected";
        file.Tags[FileProperty.VirusName] = virusName;
    }

    public static void SetVirusScanError(this Coms.Client.File file)
    {
        ArgumentNullException.ThrowIfNull(file);

        file.Tags[FileProperty.VirusScanStatus] = "error";
        file.Tags.Remove(FileProperty.VirusName);
    }

    /// <summary>
    /// Gets the named property from the property set.
    /// </summary>
    /// <param name="name">The name of the property to find.</param>
    /// <param name="properties">The property set to look for the property.</param>
    /// <returns>null if the value is not found, is empty or whitespace.</returns>
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

    /// <summary>
    /// Checks to see of the virus scan status matches the expected status. The comparison is done case insensitevly.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="statusToCheck"></param>
    /// <returns></returns>
    private static bool VirusScanIsStatus(Coms.Client.File file, string statusToCheck)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(statusToCheck);

        string status = GetVirusScanStatus(file);
        
        return StringComparer.OrdinalIgnoreCase.Equals(status, statusToCheck);
    }
}
