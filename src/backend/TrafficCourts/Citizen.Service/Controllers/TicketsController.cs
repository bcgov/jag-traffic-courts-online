using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using TrafficCourts.Citizen.Service.Features.Tickets;
using TrafficCourts.Citizen.Service.Models;
using TrafficCourts.Citizen.Service.Models.Deprecated;
using TrafficCourts.Citizen.Service.Models.Search;
using TrafficCourts.Citizen.Service.Models.Tickets;

namespace TrafficCourts.Citizen.Service.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TicketsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(IMediator mediator, ILogger<TicketsController> logger)
        {
            ArgumentNullException.ThrowIfNull(mediator);
            ArgumentNullException.ThrowIfNull(logger);

            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Searches for a violation ticket that exists on file.
        /// </summary>
        /// <param name="ticketNumber">The violation ticket number. Must start with two upper case letters and end with eight digits.</param>
        /// <param name="time">The time the violation ticket number was issued. Must be formatted a valid 24-hour clock, HH:MM.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <response code="200">The violation ticket was found.</response>
        /// <response code="400">The request was not well formed. Check the parameters.</response>
        /// <response code="404">The violation ticket was not found.</response>
        /// <response code="500">There was a server error that prevented the search from completing successfully.</response>
        [HttpGet]
        [ProducesResponseType(typeof(TicketSearchResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SearchAsync(
            [FromQuery]
            [Required]
            [RegularExpression(Search.Request.TicketNumberRegex, ErrorMessage = "ticketNumber must start with two upper case letters and 6 or more numbers")] string ticketNumber,
            [FromQuery]
            [Required]
            [RegularExpression(Search.Request.TimeRegex, ErrorMessage = "time must be properly formatted 24 hour clock")] string time,
            CancellationToken cancellationToken)
        {
            Search.Request request = new(ticketNumber, time);
            Search.Response response = await _mediator.Send(request, cancellationToken);

            if (response == Search.Response.Empty)
            {
                return NotFound();
            }

            var result = response.Result.Match<IActionResult>(
                ticket => { return Ok(ticket); },
                exception => { return StatusCode(StatusCodes.Status500InternalServerError); });

            return result;
        }

        [HttpPost]
        [ProducesResponseType(typeof(OcrViolationTicket), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> AnalyseAsync([Required] IFormFile image, CancellationToken cancellationToken)
        {
            AnalyseHandler.AnalyseRequest request = new AnalyseHandler.AnalyseRequest(image);
            AnalyseHandler.AnalyseResponse response = await _mediator.Send(request, cancellationToken);
            if (response.OcrViolationTicket.GlobalValidationErrors.Count > 0)
            {
                string? detail = "";
                string? instance = null;
                int? statusCode = (int)HttpStatusCode.BadRequest;
                string? title = "Violation Ticket is not valid or could not be read.";
                string? type = null;
                response.OcrViolationTicket.GlobalValidationErrors.ForEach(_ => detail += _ + " ");

                // Return BadRequest 
                // - if the image is not an image of a TrafficViolation (could not read title)
                // - if the TicketNumber could not be extracted or is invalid (ie doesn't start with an A)
                // - if MVA is not the only checkbox selected under the 'Did commit the offence(s) indicated' section
                // - if ViolationDate is > 30 days ago
                return Problem(detail, instance, statusCode, title, type);
            }
            return Ok(response.OcrViolationTicket);
        }

        // ---------------------------------------------------------------------------
        // Obsolete actions
        // ---------------------------------------------------------------------------

        /// <summary>
        /// This API is depricated. Use /api/tickets/search instead.
        /// </summary>
        /// <param name="ticketNumber">The violation ticket number. Must start with two upper case letters and end with eight digits.</param>
        /// <param name="time">The time the violation ticket number was issued. Must be formatted a valid 24-hour clock, HH:MM.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [Obsolete($"Use {nameof(SearchAsync)}")]
        [HttpGet]
        [ProducesResponseType(typeof(Models.Deprecated.TicketDispute), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> TicketAsync(
            [FromQuery]
            [Required]
            [RegularExpression(Search.Request.TicketNumberRegex, ErrorMessage = "ticketNumber must start with two upper case letters and 6 or more numbers")] string ticketNumber,
            [FromQuery]
            [Required]
            [RegularExpression(Search.Request.TimeRegex, ErrorMessage = "time must be properly formatted 24 hour clock")] string time,
            CancellationToken cancellationToken)
        {
            Search.Request request = new(ticketNumber, time);
            Search.Response response = await _mediator.Send(request, cancellationToken);

            if (response == Search.Response.Empty)
            {
                return NotFound();
            }

            var result = response.Result.Match<IActionResult>(
                ticket => { return Ok(ticket.CreateDeprecated()); },
                exception => { return StatusCode(StatusCodes.Status500InternalServerError); });

            return result;
        }

        [Obsolete]
        [HttpGet]
        [ProducesResponseType(typeof(ShellTicket), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShellTicket([FromBody] CreateShellTicketCommand createShellTicket)
        {
            return NoContent();
        }

        [Obsolete]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResultResponse<TicketDispute>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ShellTicket([FromBody] CreateShellTicketCommand createShellTicket)
        {
            //using (LogContext.PushProperty("TicketNumber", createShellTicket.ViolationTicketNumber))
            //{
            //    try
            //    {
            //        _logger.LogInformation("get create shell ticket request.");
            //        var response = await _mediator.Send(createShellTicket);
            //        if (response.Id == 0)
            //        {
            //            ModelState.AddModelError("TicketNumber", "This ticket already exists.");
            //            return BadRequest(ApiResponse.BadRequest(ModelState));
            //        }
            //        return RedirectToAction("Ticket", new { ticketNumber = createShellTicket.ViolationTicketNumber, time = createShellTicket.ViolationTime });
            //        //return Ok();
            //    }
            //    catch (Exception e)
            //    {
            //        _logger.LogError(e, "create shell ticket failed");
            //        return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Message(e.Message));
            //    }
            //}

            return NoContent();
        }

        [Obsolete]
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResultResponse<TicketDispute>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Pay([FromQuery] TicketPaymentCommand ticketPayment)
        {
            //using (LogContext.PushProperty("TicketNumber", ticketPayment.TicketNumber))
            //{
            //    try
            //    {
            //        _logger.LogInformation("get create ticket payment request.");
            //        var response = await _mediator.Send(ticketPayment);
            //        return Ok(ApiResponse.Result(response));
            //    }
            //    catch (Exception e)
            //    {
            //        _logger.LogError(e, "Create ticket payment request failed");
            //        return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Message(e.Message));
            //    }
            //}
            return NoContent();
        }

        [Obsolete]
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResultResponse<TicketDispute>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Pay([FromQuery] TicketPaymentConfirmCommand ticketPayConfirm)
        {
            //using (LogContext.PushProperty("TransactionId", ticketPayConfirm.TransactionId))
            //{
            //    try
            //    {
            //        _logger.LogInformation("get ticket payment confirmation");
            //        var response = await _mediator.Send(ticketPayConfirm);
            //        return RedirectToAction("Ticket", new { ticketNumber = response.TicketNumber, time = response.Time });
            //    }
            //    catch (Exception e)
            //    {
            //        _logger.LogError(e, "Update ticket payment failed");
            //        return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Message(e.Message));
            //    }
            //}
            return NoContent();
        }

        [Obsolete]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResultResponse<TicketDispute>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ImageUpload([FromForm] ShellTicketImageCommand shellTicketImage)
        {
            //try
            //{
            //    _logger.LogInformation("get shell ticket image");
            //    var response = await _mediator.Send(shellTicketImage);
            //    return Ok();
            //    //return RedirectToAction("Ticket", new { ticketNumber = response.TicketNumber, time = response.Time });
            //}
            //catch (Exception e)
            //{
            //    _logger.LogError(e, "save shell ticket image failed");
            //    return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Message(e.Message));
            //}
            return NoContent();
        }
    }
}
