using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TrafficCourts.Staff.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    [HttpGet]
    [Produces(typeof(int[]))]
    public Task<IActionResult> GetAync()
    {
        IActionResult result = Ok(new int[] { 0, 1, 2, 3 });
        return Task.FromResult(result);
    }
}
