using Newtonsoft.Json;

namespace TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;

public class Document
{
    [JsonProperty("filename")]
    public string FileName { get; set; } = string.Empty;
}
