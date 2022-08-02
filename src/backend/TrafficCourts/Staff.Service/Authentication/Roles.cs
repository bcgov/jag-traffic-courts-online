namespace TrafficCourts.Staff.Service.Authentication;

public static class Roles
{
    /// <summary>
    /// Violation Ticket Centre Staff API User
    /// </summary>
    [Obsolete("Use VtcStaff")]
    public const string VTCUser = "vtc-user";
    [Obsolete("Use JudicialJustice")]
    public const string JJUser = "vtc-user"; // TODO -- use a specific jj-user role


    public const string AdminJudicialJustice = "admin-judicial-justice";
    public const string JudicialJustice = "judicial-justice";
    public const string VtcStaff = "vtc-staff";
    public const string VtcAdminStaff = "admin-vtc-staff";
}
