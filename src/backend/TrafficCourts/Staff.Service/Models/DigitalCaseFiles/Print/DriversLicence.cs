using Newtonsoft.Json;

namespace TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;

public class DriversLicence
{
    [JsonProperty("province")]
    public string Province { get; set; } = string.Empty;

    [JsonProperty("number")]
    public string Number { get; set; } = string.Empty;
}
