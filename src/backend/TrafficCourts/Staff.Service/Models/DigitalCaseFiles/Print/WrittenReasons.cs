using Newtonsoft.Json;

namespace TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;

public class WrittenReasons
{
    [JsonProperty("fineReduction")]
    public string FineReduction { get; set; } = string.Empty;

    [JsonProperty("timeToPay")]
    public string TimeToPay { get; set; } = string.Empty;
}
