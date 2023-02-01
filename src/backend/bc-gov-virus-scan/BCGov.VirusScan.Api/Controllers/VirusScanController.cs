using MediatR;
using Microsoft.AspNetCore.Mvc;
using BCGov.VirusScan.Api.Features;

using Version = BCGov.VirusScan.Api.Features.Version;
using System.ComponentModel.DataAnnotations;
using BCGov.VirusScan.Api.Models;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;
using System.Reflection.PortableExecutable;

namespace BCGov.VirusScan.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("v1/clamav")]
    [ApiController]
    public class VirusScanController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<VirusScanController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public VirusScanController(IMediator mediator, ILogger<VirusScanController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Pings the ClamAV server
        /// </summary>
        /// <remarks>Sends a PING command to the ClamAV server</remarks>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns></returns>
        /// <response code="200">Ping was successful.</response>
        /// <response code="500">There was an error pinging ClamAV.</response>
        [HttpGet("ping", Name = "ping")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PingAsync(CancellationToken cancellationToken)
        {
            var request = Ping.Request.Instance;
            var response = await _mediator.Send(request, cancellationToken);

            if (response)
            {
                return Ok();
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Scans a file for viruses
        /// </summary>
        /// <remarks>Sends an INSTREAM command to the ClamAV server and streams the upload file for scanning.</remarks>
        /// <param name="file"></param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns></returns>
        /// <response code="200">The virus scan operation completed successfully.</response>
        /// <response code="400">No file supplied.</response>
        /// <response code="500">There was an error scanning the file for virus.</response>
        [HttpPost("scan", Name = "virusScan")]
        [ProducesResponseType(typeof(VirusScanResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ScanFileAsync([Required]IFormFile file, CancellationToken cancellationToken)
        {
            var request = new Scan.Request(file);

            var response = await _mediator.Send(request, cancellationToken);

            _logger.LogDebug("Virus scan result {@Result}", response.Result);

            return Ok(response.Result);
        }


        /// <summary>
        /// Gets the ClamAV server and databas version
        /// </summary>
        /// <remarks>Sends a VERSION command to the ClamAV server and returns the ClamAV and virus defintion versions.</remarks>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns></returns>
        /// <response code="200">The version of ClamAV was retrieved successfully.</response>
        /// <response code="500">There was an error getting the version from ClamAV.</response>
        [HttpGet("version", Name = "version")]
        [ProducesResponseType(typeof(GetVersionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VersionAsync(CancellationToken cancellationToken)
        {
            var request = Version.Request.Instance;

            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }
    }
}
