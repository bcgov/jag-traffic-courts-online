using Newtonsoft.Json;

namespace TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;

public class FormattedDateOnly
{
    private const string DateFormat = "MM\\/dd\\/yyyy";

    public static FormattedDateOnly Empty = new FormattedDateOnly();

    protected FormattedDateOnly()
    {
        Date = string.Empty;
        Value = DateTime.MinValue;
    }

    public FormattedDateOnly(DateTime dateTime)
    {
        Date = dateTime.ToString(DateFormat);
        Value = dateTime;
    }

    public FormattedDateOnly(DateTime? dateTime)
    {
        Date = dateTime?.ToString(DateFormat) ?? string.Empty;
        Value = dateTime ?? DateTime.MinValue;
    }

    public FormattedDateOnly(DateTimeOffset dateTime)
    {
        Date = dateTime.ToString(DateFormat);
        Value = dateTime.DateTime;
    }

    public FormattedDateOnly(DateTimeOffset? dateTime)
    {
        Date = dateTime?.ToString(DateFormat) ?? string.Empty;
        Value = dateTime?.DateTime ?? DateTime.MinValue;
    }

    [JsonIgnore]
    public DateTime Value { get; }

    [JsonProperty("date")]
    public string Date { get; set; }
}
