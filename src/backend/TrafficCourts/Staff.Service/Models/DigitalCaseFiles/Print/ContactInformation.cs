using Newtonsoft.Json;

namespace TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;

public class ContactInformation
{
    [JsonProperty("surname")]
    public string Surname { get; set; } = string.Empty;
    [JsonProperty("givenNames")]
    public string GivenNames { get; set; } = string.Empty;
    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;
    [JsonProperty("driversLicence")]
    public DriversLicence DriversLicence { get; set; } = new DriversLicence();
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;
}
