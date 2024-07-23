using TrafficCourts.Common.Authorization;
using TrafficCourts.Staff.Service.Authentication;

namespace TrafficCourts.Staff.Service.Features.Permissions;
/// <summary>
/// Contains the completed list of permissions.
/// </summary>
public static class ApplicationPermissions
{
    public static readonly Permission ACCEPT_DISPUTE = new(nameof(ACCEPT_DISPUTE), Resources.Dispute, Scopes.Accept);
    public static readonly Permission CANCEL_DISPUTE = new(nameof(CANCEL_DISPUTE), Resources.Dispute, Scopes.Cancel);
    public static readonly Permission READ_DISPUTE = new(nameof(READ_DISPUTE), Resources.Dispute, Scopes.Read);
    public static readonly Permission REJECT_DISPUTE = new(nameof(REJECT_DISPUTE), Resources.Dispute, Scopes.Reject);
    public static readonly Permission SUBMIT_DISPUTE = new(nameof(SUBMIT_DISPUTE), Resources.Dispute, Scopes.Submit);
    public static readonly Permission UPDATE_DISPUTE = new(nameof(UPDATE_DISPUTE), Resources.Dispute, Scopes.Update);
    public static readonly Permission VALIDATE_DISPUTE = new(nameof(VALIDATE_DISPUTE), Resources.Dispute, Scopes.Validate);

    public static readonly Permission ACCEPT_DISPUTE_DCF = new(nameof(ACCEPT_DISPUTE_DCF), Resources.JJDispute, Scopes.Accept);
    public static readonly Permission READ_DISPUTE_DCF = new(nameof(READ_DISPUTE_DCF), Resources.JJDispute, Scopes.Read);
    public static readonly Permission UPDATE_DISPUTE_DCF = new(nameof(UPDATE_DISPUTE_DCF), Resources.JJDispute, Scopes.Update);

    public static readonly Permission VIEW_JJ_WORKBENCH_ASSIGNMENTS = new(nameof(VIEW_JJ_WORKBENCH_ASSIGNMENTS), string.Empty, Scopes.View);
    public static readonly Permission VIEW_JJ_WORKBENCH_DECISION = new(nameof(VIEW_JJ_WORKBENCH_DECISION), string.Empty, Scopes.View);
    public static readonly Permission VIEW_JJ_WORKBENCH_INBOX = new(nameof(VIEW_JJ_WORKBENCH_INBOX), string.Empty, Scopes.View);

    public static readonly Permission VIEW_STAFF_WORKBENCH_DECISION_INBOX = new(nameof(VIEW_STAFF_WORKBENCH_DECISION_INBOX), string.Empty, Scopes.View);
    public static readonly Permission VIEW_STAFF_WORKBENCH_DECISION_VALIDATION = new(nameof(VIEW_STAFF_WORKBENCH_DECISION_VALIDATION), string.Empty, Scopes.View);

    public static readonly Permission VIEW_STAFF_WORKBENCH_TICKET_INBOX = new(nameof(VIEW_STAFF_WORKBENCH_TICKET_INBOX), string.Empty, Scopes.View);
    public static readonly Permission VIEW_STAFF_WORKBENCH_TICKET_VALIDATION = new(nameof(VIEW_STAFF_WORKBENCH_TICKET_VALIDATION), string.Empty, Scopes.View);

    /// <summary>
    /// Stores the unique permission names to avoid duplicates
    /// </summary>
    private static HashSet<string> _permissionNames = [];

    private static List<Permission> _permissions = [];

    public static void Add(Permission permission)
    {
        if (!_permissionNames.Add(permission.Name))
        {
            throw new InvalidOperationException($"Permission name {permission.Name} is duplicated");
        }

        _permissions.Add(permission);
    }

    public static IEnumerable<PermissionDto> GetPermissions(IList<Resource> resources)
    {
        foreach (var permission in _permissions)
        {
            yield return new PermissionDto(permission.Name, HasPermission(permission, resources));
        }
    }

    private static bool HasPermission(Permission permission, IList<Resource> resources)
    {
        // if the number of permissions grows, we should consider creating a 
        // data structure that maps resource -> scope -> permission list
        return resources.Any(resource => resource.Name == permission.Resource && resource.Scopes.Contains(permission.Scope));
    }
}
