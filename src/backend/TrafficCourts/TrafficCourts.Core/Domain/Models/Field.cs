using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace TrafficCourts.Domain.Models;

public class Field
{
    private static readonly string _dateRegex = @"^\s*(\d{2}|\d{4})\D*(\d{1,2})\D*(\d{1,2})\s*$";
    private static readonly string _timeRegex = @"^\s*(\d{1,2})\s*:?\s*(\d{1,2})\s*$";
    private static readonly string _currencyRegex = @"^\$?(\d{1,3}(\,\d{3})*|(\d+))(\.\d{2})?$";
    public static readonly string _mva = "MVA";
    public static readonly string _mvar = "MVAR";
    public static readonly string _selected = ":selected:";
    public static readonly string _unselected = ":unselected:";

    public Field() { }

    public Field(string? value)
    {
        Value = value;
    }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string? TagName { get; set; }

    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string? JsonName { get; set; }

    public string? Value { get; set; }

    public float? FieldConfidence { get; set; }

    /// <summary>
    /// A list of field-specific reasons why the field Confidence may be low
    /// </summary>
    public List<string> ValidationErrors { get; set; } = new List<string>();

    public string? Type { get; set; }

    public List<BoundingBox> BoundingBoxes { get; set; } = new List<BoundingBox>();

    /// <summary>Returns true if the given field's value is ":selected:", false if ":unselected:", otherwise null (unknown) value.</summary> 
    public bool? IsCheckboxSelected()
    {
        if (Value?.Equals(":selected:", StringComparison.OrdinalIgnoreCase) ?? false)
        {
            return true;
        }
        if (Value?.Equals(":unselected:", StringComparison.OrdinalIgnoreCase) ?? false)
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
                if (match.Groups.Count >= 4) // 3 + index 0 (the Value itself)
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
                    string newValue = Regex.Replace(Value, @"\D", ""); // replace all non digits with null
                    if (newValue.Length >= 8 && newValue.Length <= 9)
                    {
                        int year = int.Parse(newValue[..4]);
                        int month = int.Parse(newValue.Substring(4, 2));
                        int day = int.Parse(newValue.Substring(6, 2));
                        return new DateTime(year, month, day);
                        // if there are 10 could have misread field separators as ones, skip over the separator characters them
                    }
                    else if (newValue.Length == 10)
                    {
                        int year = int.Parse(newValue[..4]);
                        int month = int.Parse(newValue.Substring(5, 2));
                        int day = int.Parse(newValue.Substring(7, 2));
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
                Value = Regex.Replace(Value, @"\D", " "); // remove all non-digits

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
        return !string.IsNullOrEmpty(Value);
    }
}
