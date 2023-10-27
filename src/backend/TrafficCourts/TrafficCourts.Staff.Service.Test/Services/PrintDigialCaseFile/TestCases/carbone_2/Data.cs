using System.Text.Json.Serialization;

namespace TrafficCourts.Staff.Service.Test.Services.PrintDigialCaseFile.TestCases.carbone_2;

internal class Data
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("mainBlurb")]
    public string? MainBlurb { get; set; }
    
    [JsonPropertyName("showSectionTwo")]
    public int ShowSectionTwo { get; set; }
    
    [JsonPropertyName("sectionTwoText")]
    public string? SectionTwoText { get; set; }

    [JsonPropertyName("this-is-my-field-name")]
    public string? ThisIsMyFieldName { get; set; }

    [JsonPropertyName("this_is_another_field_name")]
    public string? ThisIsAnotherFieldName { get; set; }
}