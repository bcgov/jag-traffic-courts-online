using System.Text.Json.Serialization;

namespace TrafficCourts.Staff.Service.Test.Services.PrintDigialCaseFile.TestCases.watermark_footer_header;

internal class Data
{
    [JsonPropertyName("brandName")]
    public string? BrandName { get; set; }
    [JsonPropertyName("watermark")]
    public string? Watermark { get; set; }
    [JsonPropertyName("footerText")]
    public string? FooterText { get; set; }
}
