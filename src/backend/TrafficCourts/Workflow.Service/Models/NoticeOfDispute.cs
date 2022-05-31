namespace TrafficCourts.Workflow.Service.Models;

public class DisputedCount
{
    public Plea Plea { get; set; }
    public int Count { get; set; }
    public bool RequestTimeToPay { get; set; }
    public bool RequestReduction { get; set; }
    public bool AppearInCourt { get; set; }
}

public class LegalRepresentation
{
    public string LawFirmName { get; set; } = String.Empty;
    public string LawyerFullName { get; set; } = String.Empty;
    public string LawyerEmail { get; set; } = String.Empty;
    public string LawyerAddress { get; set; } = String.Empty;
    public string LawyerPhoneNumber { get; set; } = String.Empty;
}

/// <summary>
/// An enumeration of Plea Type on a DisputedCount record.
/// </summary>
public enum Plea
{
    /// <summary>
    /// If the dispuant is pleads guilty, plea will always be Guilty. The dispuant has choice to attend court or not.
    /// </summary>
    Guily,

    /// <summary>
    /// If the dispuant is pleads not guilty, the dispuant will have to attend court.
    /// </summary>
    NotGuilty
}

/// <summary>
/// An enumeration of available Statuses on a Dispute record.
/// </summary>
public enum DisputeStatus
{
    New,
    Processing,
    Rejected,
    Cancelled
}

