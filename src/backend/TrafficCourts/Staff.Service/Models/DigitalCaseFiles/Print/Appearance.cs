using Newtonsoft.Json;

namespace TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;

public class Appearance
{
    /// <summary>
    /// The appearance date and time
    /// </summary>
    [JsonProperty("when")]
    public FormattedDateTime When { get; set; } = FormattedDateTime.Empty;

    [JsonProperty("reason")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// The appearance room
    /// </summary>
    [JsonProperty("room")]
    public string Room { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    [JsonProperty("app")]
    public string App { get; set; } = string.Empty;

    /// <summary>
    /// No appearance date
    /// </summary>
    [JsonProperty("noApp")]
    public FormattedDateTime NoApp { get; set; }

    /// <summary>
    /// Y or N
    /// </summary>
    [JsonProperty("clerk")]
    public string Clerk { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    [JsonProperty("defenseCouncil")]
    public string DefenseCouncil { get; set; } = string.Empty;

    [JsonProperty("defenseAtt")]
    public string DefenseAtt { get; set; } = string.Empty;

    [JsonProperty("crown")]
    public string Crown { get; set; } = string.Empty;

    [JsonProperty("seized")]
    public string Seized { get; set; } = string.Empty;

    [JsonProperty("judicialJustice")]
    public string JudicialJustice { get; set; } = string.Empty;

    [JsonProperty("comments")]
    public string Comments { get; set; } = string.Empty;
}
