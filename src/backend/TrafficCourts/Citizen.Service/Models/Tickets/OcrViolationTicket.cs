using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace TrafficCourts.Citizen.Service.Models.Tickets;

/// <summary>
/// A model representation of the extracted OCR results.
/// </summary>
public class OcrViolationTicket
{

    public static readonly string ViolationTicketTitle = "violationTicketTitle";
    public static readonly string ViolationTicketNumber = "violationTicketNumber";
    public static readonly string Surname = "surname";
    public static readonly string GivenName = "givenName";
    public static readonly string DriverLicenceProvince = "driverLicenceProvince";
    public static readonly string DriverLicenceNumber = "driverLicenceNumber";
    public static readonly string ViolationDate = "violationDate";
    public static readonly string ViolationTime = "violationTime";
    public static readonly string OffenceIsMVA = "offenceIsMVA";
    public static readonly string OffenceIsMCA = "offenceIsMCA";
    public static readonly string OffenceIsCTA = "offenceIsCTA";
    public static readonly string OffenceIsWLA = "offenceIsWLA";
    public static readonly string OffenceIsFAA = "offenceIsFAA";
    public static readonly string OffenceIsLCA = "offenceIsLCA";
    public static readonly string OffenceIsTCR = "offenceIsTCR";
    public static readonly string OffenceIsOther = "offenceIsOther";
    public static readonly string Count1Description = "count1Description";
    public static readonly string Count1ActRegs = "count1ActRegs";
    public static readonly string Count1IsACT = "count1IsACT";
    public static readonly string Count1IsREGS = "count1IsREGS";
    public static readonly string Count1Section = "count1Section";
    public static readonly string Count1TicketAmount = "count1TicketAmount";
    public static readonly string Count2Description = "count2Description";
    public static readonly string Count2ActRegs = "count2ActRegs";
    public static readonly string Count2IsACT = "count2IsACT";
    public static readonly string Count2IsREGS = "count2IsREGS";
    public static readonly string Count2Section = "count2Section";
    public static readonly string Count2TicketAmount = "count2TicketAmount";
    public static readonly string Count3Description = "count3Description";
    public static readonly string Count3ActRegs = "count3ActRegs";
    public static readonly string Count3IsACT = "count3IsACT";
    public static readonly string Count3IsREGS = "count3IsREGS";
    public static readonly string Count3Section = "count3Section";
    public static readonly string Count3TicketAmount = "count3TicketAmount";
    public static readonly string HearingLocation = "hearingLocation";
    public static readonly string DetachmentLocation = "detachmentLocation";

    /// <summary>
    /// Gets or sets the saved image filename.
    /// </summary>
    public string ImageFilename { get; set; }

    /// <summary>
    /// A global confidence of correctly extracting the document. This value will be low if the title of this 
    /// Violation Ticket form is not found (or of low confidence itself) or if the main ticket number is missing or invalid.
    /// </summary>
    public float GlobalConfidence { get; set; }

    /// <summary>
    /// A list of global reasons why the global Confidence may be low (ie, missing ticket number, not a Violation Ticket, etc.)
    /// </summary>
    [JsonIgnore]
    public List<string> GlobalValidationErrors { get; set; } = new List<string>();

    /// <summary>
    /// An enumeration of all fields in this Violation Ticket.
    /// </summary>
    public Dictionary<string, Field> Fields { get; set; } = new Dictionary<string, Field>();

    public class Field
    {

        private static readonly string DateRegex = @"^(\d{2}|\d{4})\D+(\d{1,2})\D+(\d{1,2})$";
        private static readonly string TimeRegex = @"^(\d{1,2})\D*(\d{1,2})$";
        private static readonly string CurrencyRegex = @"^\$?(\d{1,3}(\,\d{3})*|(\d+))(\.\d{2})?$";

        public Field() { }

        public Field(string? value)
        {
            Value = value;
        }

        [JsonIgnore]
        public string? TagName { get; set; }

        [JsonIgnore]
        public string? JsonName { get; set; }

        public string? Value { get; set; }

        public float? FieldConfidence { get; set; }

        /// <summary>
        /// A list of field-specific reasons why the field Confidence may be low
        /// </summary>
        public List<string> ValidationErrors { get; set; } = new List<string>();

        public string? Type { get; set; }

        public List<BoundingBox> BoundingBoxes { get; set; } = new List<BoundingBox>();

        /// <summary>Returns true if the given field's value is "selected", false if "unselected", otherwise null (unknown) value.</summary> 
        public bool? IsCheckboxSelected()
        {
            if (Value?.Equals("selected") ?? false)
            {
                return true;
            }
            if (Value?.Equals("unselected") ?? false)
            {
                return false;
            }
            return null;
        }

        /// <summary>Returns a valid DateTime object if the Value string represents a date and is of the form 'yyyy MM dd', null otherwise.</summary>
        public DateTime? GetDate()
        {
            if (Value is not null)
            {
                try
                {
                    Regex rg = new(DateRegex);
                    Match match = rg.Match(Value);
                    if (match.Groups.Count == 4) // 3 + index 0 (the Value itself)
                    {
                        int year = int.Parse(match.Groups[1].Value);
                        if (year < 100)
                        {
                            year += 2000;
                        }
                        int month = int.Parse(match.Groups[2].Value);
                        int day = int.Parse(match.Groups[3].Value);
                        return new DateTime(year, month, day);
                    }
                    else
                    {
                        // pattern didn't match.  Try extracting all digits. If there are 8, convert to a date.
                        string newValue = Regex.Replace(Value, @"\D", "");
                        if (newValue.Length == 8)
                        {
                            int year = int.Parse(newValue[..4]);
                            int month = int.Parse(newValue.Substring(4, 2));
                            int day = int.Parse(newValue.Substring(6, 2));
                            return new DateTime(year, month, day);
                        }
                    }
                }
                catch (System.Exception)
                {
                    // No-op.  Will return null.
                }
            }
            return null;
        }

        /// <summary>Returns a valid DateTime object if the Value string represents a date and is of the form 'HH mm', null otherwise.</summary>
        public TimeSpan? GetTime()
        {
            if (Value is not null)
            {
                try
                {
                    Regex rg = new(TimeRegex);
                    Match match = rg.Match(Value);
                    if (match.Groups.Count == 3 && Value.Length > 2) // 2 + index 0 (the Value itself)
                    {
                        int hour = int.Parse(match.Groups[1].Value);
                        int minute = int.Parse(match.Groups[2].Value);
                        return new(hour, minute, 0);
                    }
                    else
                    {
                        // pattern didn't match.  Try extracting all digits. If there are 4, convert to a time.
                        string newValue = Regex.Replace(Value, @"\D", "");
                        if (newValue.Length == 4)
                        {
                            int hour = int.Parse(newValue[..2]);
                            int minute = int.Parse(newValue.Substring(2, 2));
                            return new(hour, minute, 0);
                        }
                    }
                }
                catch (System.Exception)
                {
                    // No-op.  Will return null.
                }
            }
            return null;
        }

        /// <summary>Returns a valid float if the Value string represents a currency and is of the form '$xx.xx' (or similar), null otherwise.</summary>
        public float? GetCurrency()
        {
            if (Value is not null)
            {
                try
                {
                    Regex rg = new(CurrencyRegex);
                    if (Regex.IsMatch(Value, CurrencyRegex))
                    {
                        return float.Parse(Value.Replace("$", "").Replace(",", ""));
                    }
                }
                catch (System.Exception)
                {
                    // No-op.  Will return null.
                }
            }
            return null;
        }
    }

    public class BoundingBox
    {
        public List<Point> Points { get; set; } = new List<Point>();
    }

    public class Point
    {
        public Point(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public float X { get; set; }
        public float Y { get; set; }
    }

}
