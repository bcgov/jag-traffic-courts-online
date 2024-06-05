namespace TrafficCourts.Staff.Service.Features.Permissions;
/// <summary>
/// Represents a single permission.
/// </summary>
/// <param name="Name">The permission name.</param>
/// <param name="Has">A flag indicating if the user has this permission.</param>
public record PermissionDto(string Name, bool Has);
