using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrafficCourts.Staff.Service.Authentication;

// Controllers can use this class instead of ControllerBase to add Authorization with Roles and the usual routing prefix
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