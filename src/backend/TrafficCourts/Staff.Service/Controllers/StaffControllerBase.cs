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
    protected bool ValidateTimeZone(string? timeZone, [NotNullWhen(true)] out TimeZoneInfo? timeZoneInfo, [NotNullWhen(false)] out IActionResult? result)
    {
        if(string.IsNullOrWhiteSpace(timeZone))
        {
            result = Problem("Time zone not supplied. Add time zone as a header X-Timezone.", statusCode: 400);
            timeZoneInfo = null;
            return false; // Indicates failure
        }

        if (!TimeZoneInfo.TryFindSystemTimeZoneById(timeZone, out timeZoneInfo))
        {
            // If the time zone is not valid, return a BadRequest with an appropriate messager
            result = Problem("Invalid time zone in header X-Timezone. Time zone must be a valid IANA or Windows time zone id.", statusCode: 400);
            return false;
        }

        result = null;
        return true; // Indicates success
    }
}
