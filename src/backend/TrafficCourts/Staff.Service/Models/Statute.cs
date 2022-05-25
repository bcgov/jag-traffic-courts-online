using StackExchange.Redis;

namespace TrafficCourts.Staff.Service.Models;

/// <summary>
/// A Statute is a Violation Ticket Fine Regulation as dictated by the BC Government.
/// </summary>
public class Statute
{

    public Statute(decimal code, string act, string section, string description)
    {
        Code = code;
        Act = act;
        Section = section;
        Description = description;
    }

    public decimal Code { get; set; }
    public string Act { get; set; }
    public string Section { get; set; }
    public string Description { get; set; }
}
