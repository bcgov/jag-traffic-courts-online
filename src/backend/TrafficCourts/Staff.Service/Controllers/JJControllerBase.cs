using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrafficCourts.Staff.Service.Authentication;

// Controllers can use this class instead of ControllerBase to add Authorization with Roles and the usual routing prefix
namespace TrafficCourts.Staff.Service.Controllers
{
    [ApiController]
    [Authorize(Roles = Roles.VTCUser)] // TODO:  New jj-user role
    [Route("api/[controller]")]
    public abstract class JJControllerBase<TController> : ControllerBase
    {
        protected readonly ILogger<TController> _logger;

        protected JJControllerBase(ILogger<TController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}