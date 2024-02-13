using Newtonsoft.Json;

namespace TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;

public class CourtOptions
{
    [JsonProperty("counsel")]
    public LegalCounsel LegalCounsel { get; set; } = new LegalCounsel();

    [JsonProperty("witnessCount")]
    public int WitnessCount { get; set; }
    
    [JsonProperty("interpreter")]
    public string InterpreterLanguage { get; set; } = string.Empty;

    [JsonProperty("attendanceType")]
    public string DisputantAttendanceType { get; set; } = string.Empty;
}
