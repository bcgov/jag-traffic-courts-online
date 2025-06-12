using Newtonsoft.Json;

namespace TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;

public class FormattedDateTime : FormattedDateOnly
{
    private const string _timeFormat24Hour = "HH:mm";
    private const string _timeFormat12Hour = "hh:mm tt";

    public static new FormattedDateTime Empty = new FormattedDateTime();

    private FormattedDateTime() : base()
    {
        Time = string.Empty;
    }

    public FormattedDateTime(DateTime dateTime, bool use12HourFormat = false) : base(dateTime)
    {
        Time = dateTime.ToString(use12HourFormat ? _timeFormat12Hour : _timeFormat24Hour);
    }

    public FormattedDateTime(DateTime? dateTime, bool use12HourFormat = false) : base(dateTime)
    {
        Time = dateTime?.ToString(use12HourFormat ? _timeFormat12Hour : _timeFormat24Hour) ?? string.Empty;
    }

    public FormattedDateTime(DateTimeOffset dateTime, bool use12HourFormat = false) : base(dateTime)
    {
        Time = dateTime.ToString(use12HourFormat ? _timeFormat12Hour : _timeFormat24Hour);
    }

    public FormattedDateTime(DateTimeOffset? dateTime, bool use12HourFormat = false) : base(dateTime)
    {
        Time = dateTime?.ToString(use12HourFormat ? _timeFormat12Hour : _timeFormat24Hour) ?? string.Empty;
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
