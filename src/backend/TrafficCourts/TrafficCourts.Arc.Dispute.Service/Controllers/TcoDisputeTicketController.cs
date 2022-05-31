using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using TrafficCourts.Arc.Dispute.Service.Models;
using TrafficCourts.Arc.Dispute.Service.Services;

namespace TrafficCourts.Arc.Dispute.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TcoDisputeTicketController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IArcFileService _arcFileService;

    // Assign the object in the constructor for dependency injection
    public TcoDisputeTicketController(IMapper mapper, IArcFileService arcFileService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _arcFileService = arcFileService ?? throw new ArgumentNullException(nameof(arcFileService));
    }

    /// <summary>
    /// Creates ARC file and uploads it to a SFTP location by extracting TCO Dispute Ticket data from the JSON request.
    /// </summary>
    /// <param name="disputeData">A JSON object that contains TCO Dispute Ticket data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <response code="200">TCO Dispute Ticket data extracted from JSON request, created the ARC file in proper format and uploaded it to the SFTP location successfully.</response>
    /// <response code="400">The request JSON data is empty or not valid that does not contain the required data to output an ARC file in proper format.</response>
    [HttpPost]
    [ProducesResponseType(typeof(IList<ArcFileRecord>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateArcFile([Required][FromBody] TcoDisputeTicket disputeData, CancellationToken cancellationToken)
    {
        var arcFileRecords = _mapper.Map<List<ArcFileRecord>>(disputeData);

        await _arcFileService.CreateArcFile(arcFileRecords, cancellationToken);

        return Ok(arcFileRecords);
    }
}
