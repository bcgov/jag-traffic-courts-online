using Microsoft.AspNetCore.Mvc;
using TrafficCourts.TicketSearch;

namespace TrafficCourts.Hotfix.DataMigration.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class HotfixController : ControllerBase
    {
        private readonly ILogger<HotfixController> _logger;

        private readonly ITicketSearchService _ticketSearchService;

        public HotfixController(ILogger<HotfixController> logger, ITicketSearchService ticketSearchService)
        {
            _logger = logger;
            _ticketSearchService = ticketSearchService;
        }

        [HttpGet("/list")]
        public List<string> Get()
        {
            _logger.LogInformation("Retrieving list of Hotfixs");
            return ["00001_Fix_Missing_Counts_On_OCCAM_Violation_Tickets.sql"];
        }

        [HttpPost("/run/{HotfixName}")]
        public IActionResult RunHotfix(string HotfixName)
        {
            _logger.LogInformation("Running Hotfix: {HotfixName}", HotfixName);

            switch (HotfixName)
            {
                case "00001_Fix_Missing_Counts_On_OCCAM_Violation_Tickets":
                    try
                    {
                        // await _ticketSearchService.SearchAsync(HotfixName);
                        return Ok($"Hotfix {HotfixName} completed successfully.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error running Hotfix: {HotfixName}", HotfixName);
                        return StatusCode(500, $"Error running Hotfix {HotfixName}: {ex.Message}");
                    }

                default:
                    _logger.LogWarning("Hotfix {HotfixName} is not recognized.", HotfixName);
                    return NotFound($"Hotfix {HotfixName} not found.");

            }
        }
    }
}
