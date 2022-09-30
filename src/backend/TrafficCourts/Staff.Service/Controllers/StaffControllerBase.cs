using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TrafficCourts.Staff.Service.Controllers;

/// <summary>
/// Base controller type for all staff api controllers.
/// </summary>
/// <typeparam name="TController"></typeparam>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public abstract class StaffControllerBase<TController> : ControllerBase 
    where TController : StaffControllerBase<TController>
{
    protected readonly ILogger<TController> _logger;

    protected StaffControllerBase(ILogger<TController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
