package ca.bc.gov.open.jag.tco.oracledataapi.controller.v1_0;

import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import ca.bc.gov.open.jag.tco.oracledataapi.model.EmailHistory;
import ca.bc.gov.open.jag.tco.oracledataapi.service.EmailHistoryService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;

@RestController(value = "EmailHistoryControllerV1_0")
@RequestMapping("/api/v1.0")
public class EmailHistoryController {

	@Autowired
	private EmailHistoryService emailHistoryService;

	private Logger logger = LoggerFactory.getLogger(EmailHistoryController.class);

	/**
	 * GET endpoint that retrieves all the emails optionally filtered by ticketNumber from the database
	 * @param ticketNumber will filter the result set to those related to a specific ticket
	 * @return list of all emails for specified ticketnumber
	 */
	@GetMapping("/emailHistory/{ticketNumber}")
	public List<EmailHistory> getEmailHistoryByTicketNumber(
			@PathVariable
			@Parameter(description = "Ticket number to retrieve related emails.")
			String ticketNumber) {
		logger.debug("getEmailHistoryForTicket called");

		return emailHistoryService.getEmailHistoryByTicketNumber(ticketNumber);
	}

	/**
	 * POST endpoint that inserts an email history record for given ticketnumber.
	 *
	 * @return inserted {@link EmailHistory}
	 */
	@Operation(summary = "Inserts an email history record for the given ticket number.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok"),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "404", description = "An invalid email history record provided. Insert failed.")
	})
	@PostMapping("/emailHistory/{ticketNumber}")
	public ResponseEntity<Long> insertEmailHistory(
			@RequestBody EmailHistory emailHistory) {
		logger.debug("POST /emailHistory/{ticketNumber} called");
		return new ResponseEntity<Long>(emailHistoryService.insertEmailHistory(emailHistory), HttpStatus.OK);
	}
}
