namespace TrafficCourts.OrdsDataService.Justin;

public record Statute
{
#pragma warning disable IDE1006 // Naming Styles
    public int stat_id { get; set; }
    public string act_cd { get; set; } = string.Empty;
    public string stat_section_txt { get; set; } = string.Empty;
    public string stat_sub_section_txt { get; set; } = string.Empty;
    public string? stat_paragraph_txt { get; set; } = string.Empty;
    public string? stat_sub_paragraph_txt { get; set; } = string.Empty;
    public string stat_short_description_txt { get; set; } = string.Empty;
    public string stat_description_txt { get; set; } = string.Empty;
    public DateTime stat_effective_dt { get; set; }
    public DateTime? stat_termination_dt { get; set; }
#pragma warning restore IDE1006 // Naming Styles
}
