using Newtonsoft.Json;

namespace TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;

public class TicketSummary
{
    /// <summary>
    /// The ticket number
    /// </summary>
    [JsonProperty("number")]
    public string Number { get; set; } = string.Empty;

    [JsonProperty("surname")]
    public string Surname { get; set; } = string.Empty;

    [JsonProperty("givenNames")]
    public string GivenNames { get; set; } = string.Empty;

    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;

    [JsonProperty("dateOfBirth")]
    public FormattedDateOnly DateOfBirth { get; set; } = FormattedDateOnly.Empty;

    [JsonProperty("issued")]
    public FormattedDateTime Issued { get; set; } = FormattedDateTime.Empty;

    [JsonProperty("submitted")]
    public FormattedDateOnly Submitted { get; set; } = FormattedDateOnly.Empty;

    [JsonProperty("courtHouse")]
    public string CourtHouse { get; set; } = string.Empty;

    [JsonProperty("icbcReceived")]
    public FormattedDateOnly IcbcReceived { get; set; } = FormattedDateOnly.Empty;

    [JsonProperty("policeDetachment")]
    public string PoliceDetachment { get; set; } = string.Empty;

    [JsonProperty("offenceLocation")]
    public string OffenceLocation { get; set; } = string.Empty;

    [JsonProperty("courtAgenyId")]
    public string CourtAgenyId { get; set; } = string.Empty;
}
