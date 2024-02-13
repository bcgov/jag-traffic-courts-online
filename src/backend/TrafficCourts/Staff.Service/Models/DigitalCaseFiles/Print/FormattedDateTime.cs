using Newtonsoft.Json;

namespace TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;

public class FormattedDateTime : FormattedDateOnly
{
    private const string TimeFormat = "HH:mm";

    public static new FormattedDateTime Empty = new FormattedDateTime();

    private FormattedDateTime() : base()
    {
        Time = string.Empty;
    }

    public FormattedDateTime(DateTime dateTime) : base(dateTime)
    {
        Time = dateTime.ToString(TimeFormat);
    }

    public FormattedDateTime(DateTime? dateTime) : base(dateTime)
    {
        Time = dateTime?.ToString(TimeFormat) ?? string.Empty;
    }

    public FormattedDateTime(DateTimeOffset dateTime) : base(dateTime)
    {
        Time = dateTime.ToString(TimeFormat);
    }

    public FormattedDateTime(DateTimeOffset? dateTime) : base(dateTime)
    {
        Time = dateTime?.ToString(TimeFormat) ?? string.Empty;
    }

    [JsonProperty("time")]
    public string Time { get; set; }

    /// <summary>
    /// The formatted date and time
    /// </summary>
    [JsonProperty("datetime")]
    public string DateTime
    {
        get
        {
            if (Date == string.Empty && Time == string.Empty)
            {
                return string.Empty;
            }

            return $"{Date} {Time}";
        }
    }
}
