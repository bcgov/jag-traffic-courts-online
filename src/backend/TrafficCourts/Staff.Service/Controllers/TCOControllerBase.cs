using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrafficCourts.Staff.Service.Authentication;

namespace TrafficCourts.Staff.Service.Controllers
{
    [ApiController]
    [Authorize(Roles = Roles.User)]
    [Route("api/[controller]")]
    public abstract class TCOControllerBase<TController> : ControllerBase
    {
        protected readonly ILogger<TController> _logger;

        protected TCOControllerBase(ILogger<TController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}