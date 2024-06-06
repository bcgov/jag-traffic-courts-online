using FastEndpoints;
using TrafficCourts.Common.Authorization;

namespace TrafficCourts.Staff.Service.Features.Permissions;

public class PermissionsEndpoint : EndpointWithoutRequest<IEnumerable<PermissionDto>>
{
    private readonly IKeycloakAuthorizationService _authorizationService;

    public PermissionsEndpoint(IKeycloakAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
    }

    public override void Configure()
    {
        Get("/api/permissions");
        Description(x =>
        {
            x.WithSummary("Gets the current users permissions");
            x.WithTags("Permissions");
            x.Produces<IEnumerable<PermissionDto>>(200);
        });
    }

    public override async Task<IEnumerable<PermissionDto>> ExecuteAsync(CancellationToken ct)
    {
        IList<Resource> resources = await _authorizationService.GetPermissionsAsync(ct);

        IEnumerable<PermissionDto> response = ApplicationPermissions.GetPermissions(resources);

        return response;
    }
}