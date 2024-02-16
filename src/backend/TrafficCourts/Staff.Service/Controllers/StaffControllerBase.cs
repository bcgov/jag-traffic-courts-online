using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TrafficCourts.Staff.Service.Controllers;

/// <summary>
/// Base controller type for all staff api controllers.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public abstract class StaffControllerBase : ControllerBase
{
}
