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
import ca.bc.gov.open.jag.tco.oracledataapi.model.FileHistory;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.service.EmailHistoryService;
import ca.bc.gov.open.jag.tco.oracledataapi.service.FileHistoryService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;

@RestController(value = "FileHistoryControllerV1_0")
@RequestMapping("/api/v1.0/jj")
public class FileHistoryController {

	@Autowired
	private FileHistoryService fileHistoryService;

	private Logger logger = LoggerFactory.getLogger(FileHistoryController.class);

	/**
	 * GET endpoint that retrieves all the file history optionally filtered by ticketNumber from the database
	 * @param ticketNumber will filter the result set to those related to a specific ticket
	 * @return list of all file history for specified ticketnumber
	 */
	@GetMapping("/fileHistory/{ticketNumber}")
	public List<FileHistory> getFileHistoryByTicketNumber(
			@PathVariable
			@Parameter(description = "Ticket number to retrieve related file history.")
			String ticketNumber) {
		logger.debug("getFileHistoryForTicket called");

		return fileHistoryService.getFileHistoryByTicketNumber(ticketNumber);
	}
		
	/**
	 * POST endpoint that inserts a file history record for given ticketnumber.
	 *
	 * @param id (ticket number) of the saved {@link JJDispute} to update
	 * @return inserted {@link FileHistory}
	 */
	@Operation(summary = "Inserts a file history record for the given ticket number.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok"),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "404", description = "An invalid file history record provided. Insert failed.")
	})
	@PostMapping("/fileHistory/{ticketNumber}")
	public ResponseEntity<Long> insertFileHistory(
			@PathVariable("ticketNumber") String ticketNumber, 
			@RequestBody FileHistory fileHistory) {
		logger.debug("POST /fileHistory/{ticketNumber} called");
		return new ResponseEntity<Long>(fileHistoryService.insertFileHistory(ticketNumber, fileHistory), HttpStatus.OK);
	}
}
