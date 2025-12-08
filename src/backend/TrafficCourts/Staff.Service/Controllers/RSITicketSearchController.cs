using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using TrafficCourts.TicketSearch;
using TrafficCourts.Common.Authorization;
using TrafficCourts.Staff.Service.Authentication;

namespace TrafficCourts.Staff.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoadSafetyTicketSearchController : ControllerBase
{
    private const string TicketNumberRegex = "^[A-Z]{2}[0-9]{8}$";
    private const string TimeRegex = "^(2[0-3]|[01]?[0-9]):([0-5]?[0-9])$";

    private readonly ITicketSearchService _ticketSearchService;

    public RoadSafetyTicketSearchController(ITicketSearchService ticketSearchService)
    {
        _ticketSearchService = ticketSearchService ?? throw new ArgumentNullException(nameof(ticketSearchService));
    }

    /// <summary>
    /// Searches for a violation ticket that exists on file.
    /// </summary>
    /// <param name="ticketNumber">The violation ticket number. Must start with two upper case letters and end with eight digits.</param>
    /// <param name="issuedTime">The time the violation ticket number was issued. Must be formatted a valid 24-hour clock, HH:MM.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <response code="200">The violation ticket was found.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="404">The violation ticket was not found.</response>
    /// <response code="500">There was a server error that prevented the search from completing successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(Ticket), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.Dispute, Scopes.Submit)]
    public async Task<IActionResult> SearchAsync(
        [FromQuery]
        [Required]
        string ticketNumber,
        [FromQuery]
        [Required]
        string issuedTime,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(ticketNumber);
        ArgumentNullException.ThrowIfNull(issuedTime);

        if (!Regex.IsMatch(ticketNumber, TicketNumberRegex))
        {
            throw new ArgumentException("ticketNumber must start with two upper case letters and 8 or more numbers", nameof(ticketNumber));
        }

        // use regex as well as TimeOnly.TryParse because we dont want seconds, milliseconds, etc.
        if (!Regex.IsMatch(issuedTime, TimeRegex))
        {
            throw new ArgumentException("time must be properly formatted 24 hour clock with only hours and minutes", nameof(issuedTime));
        }

        if (!TimeOnly.TryParse(issuedTime, out var timeOnly))
        {
            throw new ArgumentException("time must be properly formatted 24 hour clock with only hours and minutes", nameof(issuedTime));
        }

        var ticket = await _ticketSearchService.SearchAsync(ticketNumber, timeOnly, cancellationToken);

        if (ticket == null)
        {
            return NotFound();
        }

        return Ok(ticket);
    }
}
