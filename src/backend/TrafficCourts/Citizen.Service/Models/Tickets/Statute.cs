namespace TrafficCourts.Citizen.Service.Models.Tickets;

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