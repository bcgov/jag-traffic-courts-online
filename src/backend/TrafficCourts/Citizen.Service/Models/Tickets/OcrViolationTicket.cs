using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace TrafficCourts.Citizen.Service.Models.Tickets;

/// <summary>
/// A model representation of the extracted OCR results.
/// </summary>
public class OcrViolationTicket
{

    public static readonly string ViolationTicketTitle = "violationTicketTitle";
    public static readonly string ViolationTicketNumber = "ticket_number";
    public static readonly string Surname = "disputant_surname";
    public static readonly string GivenName = "disputant_given_names";
    public static readonly string DriverLicenceProvince = "drivers_licence_province";
    public static readonly string DriverLicenceNumber = "drivers_licence_number";
    public static readonly string ViolationDate = "violation_date";
    public static readonly string ViolationTime = "violation_time";
    public static readonly string OffenceIsMVA = "is_mva_offence";
    public static readonly string OffenceIsMCA = "is_mca_offence";
    public static readonly string OffenceIsCTA = "is_cta_offence";
    public static readonly string OffenceIsWLA = "is_wla_offence";
    public static readonly string OffenceIsFAA = "is_faa_offence";
    public static readonly string OffenceIsLCA = "is_lca_offence";
    public static readonly string OffenceIsTCR = "is_tcr_offence";
    public static readonly string OffenceIsOther = "is_other_offence";
    public static readonly string Count1Description = "counts.count_no_1.description";
    public static readonly string Count1ActRegs = "counts.count_no_1.act_or_regulation_name_code";
    public static readonly string Count1IsACT = "counts.count_no_1.is_act";
    public static readonly string Count1IsREGS = "counts.count_no_1.is_regulation";
    public static readonly string Count1Section = "counts.count_no_1.section";
    public static readonly string Count1TicketAmount = "counts.count_no_1.ticketed_amount";
    public static readonly string Count2Description = "counts.count_no_2.description";
    public static readonly string Count2ActRegs = "counts.count_no_2.act_or_regulation_name_code";
    public static readonly string Count2IsACT = "counts.count_no_2.is_act";
    public static readonly string Count2IsREGS = "counts.count_no_2.is_regulation";
    public static readonly string Count2Section = "counts.count_no_2.section";
    public static readonly string Count2TicketAmount = "counts.count_no_2.ticketed_amount";
    public static readonly string Count3Description = "counts.count_no_3.description";
    public static readonly string Count3ActRegs = "counts.count_no_3.act_or_regulation_name_code";
    public static readonly string Count3IsACT = "counts.count_no_3.is_act";
    public static readonly string Count3IsREGS = "counts.count_no_3.is_regulation";
    public static readonly string Count3Section = "counts.count_no_3.section";
    public static readonly string Count3TicketAmount = "counts.count_no_3.ticketed_amount";
    public static readonly string HearingLocation = "court_location";
    public static readonly string DetachmentLocation = "detachment_location";
    public static readonly string DateOfService = "service_date";

    /// <summary>
    /// Gets or sets the saved image filename.
    /// </summary>
    public string? ImageFilename { get; set; }

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

    /// <summary>
    /// Return true if any of the 4 Count fields has a value.
    /// </summary>
    public bool IsCount1Populated()
    {
        return Fields[Count1Description].IsPopulated()
            || Fields[Count1ActRegs].IsPopulated()
            || Fields[Count1Section].IsPopulated()
            || Fields[Count1TicketAmount].IsPopulated();
    }
    
    /// <summary>
    /// Return true if any of the 4 Count fields has a value.
    /// </summary>
    public bool IsCount2Populated()
    {
        return Fields[Count2Description].IsPopulated()
            || Fields[Count2ActRegs].IsPopulated()
            || Fields[Count2Section].IsPopulated()
            || Fields[Count2TicketAmount].IsPopulated();
    }
    
    /// <summary>
    /// Return true if any of the 4 Count fields has a value.
    /// </summary>
    public bool IsCount3Populated()
    {
        return Fields[Count3Description].IsPopulated()
            || Fields[Count3ActRegs].IsPopulated()
            || Fields[Count3Section].IsPopulated()
            || Fields[Count3TicketAmount].IsPopulated();
    }
}

public class Field
{
    private static readonly string _dateRegex = @"^(\d{2}|\d{4})\D+(\d{1,2})\D+(\d{1,2})$";
    private static readonly string _timeRegex = @"^(\d{1,2})\D*(\d{1,2})$";
    private static readonly string _currencyRegex = @"^\$?(\d{1,3}(\,\d{3})*|(\d+))(\.\d{2})?$";

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
        if (Value?.Equals("selected", StringComparison.OrdinalIgnoreCase) ?? false)
        {
            return true;
        }
        if (Value?.Equals("unselected", StringComparison.OrdinalIgnoreCase) ?? false)
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
                Regex rg = new(_dateRegex);
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
                Regex rg = new(_timeRegex);
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
                Regex rg = new(_currencyRegex);
                if (Regex.IsMatch(Value, _currencyRegex))
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

    public bool IsPopulated()
    {
        return !String.IsNullOrEmpty(Value);
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
