using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TrafficCourts.Arc.Dispute.Service.Models;
using TrafficCourts.Arc.Dispute.Service.Services;

namespace TrafficCourts.Arc.Dispute.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TcoDisputeTicketController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IArcFileService _arcFileService;
    private readonly ILogger<TcoDisputeTicketController> _logger;

    // Assign the object in the constructor for dependency injection
    public TcoDisputeTicketController(
        IMapper mapper, 
        IArcFileService arcFileService,
        ILogger<TcoDisputeTicketController> logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _arcFileService = arcFileService ?? throw new ArgumentNullException(nameof(arcFileService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
    [ProducesResponseType(typeof(IList<ArcFileRecord>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateArcFile([Required][FromBody] TcoDisputeTicket disputeData, CancellationToken cancellationToken)
    {
        CleanDisputeData(disputeData);

        if (disputeData.TicketDetails.Count == 0)
        {
            return BadRequest(); // ?
        }

        var arcFileRecords = _mapper.Map<List<ArcFileRecord>>(disputeData);

        await _arcFileService.CreateArcFileAsync(arcFileRecords, cancellationToken);

        return Ok(arcFileRecords);
    }

    private void CleanDisputeData(TcoDisputeTicket disputeData)
    {
        // ticket counts are currently soft deleted by setting the Act property to null
        // Remove TicketDetails with soft deleted counts
        int removedTicketCounts = disputeData.TicketDetails.RemoveAll(td => td.IsDeleted);
        var ticketCounts = disputeData.TicketDetails.Select(td => td.Count).ToList();

        // TCVP-3184 - Remove dispute counts where there is not a corresponding ticket count
        int removedDisputeCounts = disputeData.DisputeCounts.RemoveAll(dc => !ticketCounts.Contains(dc.Count));

        if (removedTicketCounts > 0)
        {
            if (disputeData.TicketDetails.Count == 0)
            {
                _logger.LogWarning("Removed all {RemovedCount} ticket count records on {TicketNumber}. Dispute cant be sent to ARC",
                    removedTicketCounts,
                    disputeData.TicketFileNumber);
            }
            else
            {
                _logger.LogInformation("Removed {RemovedCount} ticket count records on {TicketNumber}. {NumberOfCounts} will be sent to ARC",
                    removedTicketCounts, 
                    disputeData.TicketFileNumber, 
                    disputeData.TicketDetails.Count);
            }
        }

        if (removedDisputeCounts > 0)
        {
            if (disputeData.DisputeCounts.Count == 0)
            {
                _logger.LogWarning("Removed all {RemovedCount} dispute records on {TicketNumber}. Dispute cant be sent to ARC",
                    removedDisputeCounts,
                    disputeData.TicketFileNumber);
            }
            else
            {
                _logger.LogInformation("Removed {RemovedCount} dispute records on {TicketNumber}. {NumberOfCounts} will be sent to ARC",
                    removedDisputeCounts,
                    disputeData.TicketFileNumber,
                    disputeData.DisputeCounts.Count);
            }
        }

        if (disputeData.TicketDetails.Count != disputeData.DisputeCounts.Count)
        {
            // generally the TicketDetails.Count >= disputeData.DisputeCounts.Count
            _logger.LogWarning("The number of count on the ticket and number of disputed counts do not match on {TicketNumber}. Ticket count: {TicketCount}, Dispute count: {DisputeCount}",
                disputeData.TicketFileNumber,
                disputeData.TicketDetails.Count,
                disputeData.DisputeCounts.Count);
        }
    }
}
