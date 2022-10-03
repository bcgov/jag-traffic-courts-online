using Microsoft.AspNetCore.Mvc;
using System.Net;
using TrafficCourts.Common.OpenAPIs.KeycloakAdminApi.v18_0;
using TrafficCourts.Staff.Service.Services;

namespace TrafficCourts.Staff.Service.Controllers;

public class KeycloakController : StaffControllerBase<KeycloakController>
{
    private readonly IKeycloakService _keycloakService;

    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <param name="keycloakService"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
    public KeycloakController(IKeycloakService keycloakService, ILogger<KeycloakController> logger) : base(logger)
    {
        _keycloakService = keycloakService ?? throw new ArgumentNullException(nameof(keycloakService));
    }

    /// <summary>
    /// Returns all Users with the given group name.
    /// </summary>
    /// <param name="groupName">A unique group name to query.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A collection of Users records</returns>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    [HttpGet("{groupName}/users")]
    [ProducesResponseType(typeof(IList<UserRepresentation>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUsersAsync(string groupName, CancellationToken cancellationToken)
    {
        try
        {
            ICollection<UserRepresentation> userRepresentations = await _keycloakService.UsersByGroupAsync(groupName, cancellationToken);
            return Ok(userRepresentations);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving Users by GroupName from Keycloak");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

}