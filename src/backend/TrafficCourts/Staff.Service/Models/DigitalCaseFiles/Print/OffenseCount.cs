using Newtonsoft.Json;

namespace TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;

public class OffenseCount
{
    [JsonProperty("count")]
    public string Count { get; set; } = string.Empty;

    /// <summary>
    /// The offense date. This will be the same as the disute issued date.
    /// Duplicating the field makes it easier for binding to the screen.
    /// </summary>
    [JsonProperty("offense")]
    public FormattedDateOnly Offense { get; set; } = FormattedDateOnly.Empty;

    [JsonProperty("plea")]
    public string Plea { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("due")]
    public FormattedDateOnly Due { get; set; } = FormattedDateOnly.Empty;

    [JsonProperty("fine")]
    public decimal? Fine { get; set; }

    [JsonProperty("requestFineReduction")]
    public string RequestFineReduction { get; set; } = string.Empty;

    [JsonProperty("requestTimeToPay")]
    public string RequestTimeToPay { get; set; } = string.Empty;
}
