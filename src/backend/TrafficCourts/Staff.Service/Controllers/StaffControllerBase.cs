using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Staff.Service.Controllers;

/// <summary>
/// Base controller type for all staff api controllers.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public abstract class StaffControllerBase : ControllerBase
{
    protected bool ValidateTimeZone(string timeZone, [NotNullWhen(true)] out TimeZoneInfo? timeZoneInfo, [NotNullWhen(false)] out IActionResult? result)
    {
        if (!TimeZoneInfo.TryFindSystemTimeZoneById(timeZone, out timeZoneInfo))
        {
            result = BadRequest("Invalid time zone. Time zone must be a valid IANA or Windows time zone id.");
            return false;
        }

        result = null;
        return true; // Indicates success
    }
}
