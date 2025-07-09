using Microsoft.AspNetCore.Mvc;
using TrafficCourts.Hotfix.DataMigration.Hotfixes;

namespace TrafficCourts.Hotfix.DataMigration.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class HotfixController : ControllerBase
    {
        private readonly ILogger<HotfixController> _logger;
        private readonly IHotfixManager _hotfixManager;

        public HotfixController(ILogger<HotfixController> logger, IHotfixManager hotfixManager)
        {
            _logger = logger;
            _hotfixManager = hotfixManager;
        }

        [HttpGet("/list")]
        public List<string> Get()
        {
            _logger.LogInformation("Retrieving list of Hotfixes");
            return _hotfixManager.GetHotfixNames();
        }

        [HttpPost("/run/{HotfixName}/{fixVersion}")]
        public async Task<IActionResult> RunHotfix(
            string HotfixName,
            string fixVersion,
            [FromQuery] bool dryRun = true,
            [FromQuery] string environment = "dev", 
            [FromQuery] int batchSize = 100,
            [FromBody] Dictionary<string, object>? additionalData = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Running Hotfix: {HotfixName} with fixVersion={FixVersion}, dryRun={DryRun}, environment={Environment}, batchSize={BatchSize}", 
                HotfixName, fixVersion, dryRun, environment, batchSize);

            try
            {
                additionalData ??= new Dictionary<string, object>();

                var result = await _hotfixManager.ExecuteHotfixAsync(
                    HotfixName, 
                    fixVersion,
                    dryRun, 
                    environment, 
                    batchSize, 
                    additionalData, 
                    cancellationToken);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Hotfix {HotfixName} is not recognized.", HotfixName);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running Hotfix: {HotfixName}", HotfixName);
                return StatusCode(500, $"Error running Hotfix {HotfixName}: {ex.Message}");
            }
        }
    }
}
