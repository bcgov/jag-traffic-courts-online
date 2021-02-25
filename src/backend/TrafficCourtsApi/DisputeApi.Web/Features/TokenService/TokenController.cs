using System.Linq;
using System.Threading.Tasks;
using DisputeApi.Web.Features.TokenService.Model;
using DisputeApi.Web.Features.TokenService.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace DisputeApi.Web.Features.TokenService.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ITokensService _tokensService;
        public TokensController(
            ILogger<TokensController> logger, ITokensService tokensService)
        {
            _logger = logger;
            _tokensService = tokensService;
        }

        [HttpGet]
        [Route("getToken")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Token), StatusCodes.Status200OK)]
        [OpenApiTag("Dispute API")]
        public async Task<IActionResult> GetToken()
        {
            return Ok(await _tokensService.CreateToken());
        }

    }
}
