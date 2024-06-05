namespace TrafficCourts.Staff.Service.Features.Permissions;


public class Permission
{
    /// <summary>
    /// The name of permission
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The resource for this permission
    /// </summary>
    public string Resource { get; set; }
    
    /// <summary>
    /// The scope for this permission.
    /// </summary>
    public string Scope { get; set; }

    /// <summary>
    /// Creates a new permission instance.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="resource"></param>
    /// <param name="scope"></param>
    public Permission(string name, string resource, string scope)
    {
        Name = name;
        Resource = resource;
        Scope = scope;

        ApplicationPermissions.Add(this);
    }
}
