using Newtonsoft.Json;

namespace TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;

public class WrittenReasons
{
    [JsonProperty("fineReduction")]
    public string FineReduction { get; set; } = string.Empty;

    [JsonProperty("timeToPay")]
    public string TimeToPay { get; set; } = string.Empty;

    [JsonProperty("signature")]
    public string Signature { get; set; } = string.Empty;

    [JsonProperty("signatureType")]
    public string SignatureType { get; set; } = string.Empty;

    [JsonProperty("submissionTs")]
    public FormattedDateTime SubmissionTs { get; set; } = FormattedDateTime.Empty;
}
