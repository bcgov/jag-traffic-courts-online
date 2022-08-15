namespace TrafficCourts.Staff.Service.Authentication;

/// <summary>
/// Policies defined for the application. When used 
/// </summary>
public class Policies
{
    /// <summary>
    /// Name of the policy to check if a user can assign a dispute to a JJ.
    /// </summary>
    public const string CanAssignDisputes = $"{Resources.Dispute}:{Scopes.Assign}";

    /// <summary>
    /// Can submit a dispute after it has been reviewed. Dispute will be submitted to ARC.
    /// </summary>
    public const string CanSubmitDispute = "";
    public const string CanSubmitDisputeDecision = "";
    public const string CanSubmitDisputeToJustin = "";
    public const string CanManageCourthouseTeamsAssignments = "";
    public const string CanFixData = "";
    public const string CanSendDisputeToJJ = "";
}
