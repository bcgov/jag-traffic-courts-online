using Newtonsoft.Json;

namespace TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;

public class LegalCounsel
{
    [JsonProperty("name")]
    public string FullName { get; set; } = string.Empty;

    [JsonProperty("firm")]
    public string FirmName { get; set; } = string.Empty;
}
