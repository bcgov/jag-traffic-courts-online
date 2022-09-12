package ca.bc.gov.open.jag.tco.oracledataapi.controller.v1_0;

import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.security.core.userdetails.User;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import ca.bc.gov.open.jag.tco.oracledataapi.model.CustomUserDetails;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.service.JJDisputeService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;

@RestController(value = "JJDisputeControllerV1_0")
@RequestMapping("/api/v1.0/jj")
public class JJDisputeController {

	@Autowired
	private JJDisputeService jjDisputeService;

	private Logger logger = LoggerFactory.getLogger(JJDisputeController.class);

	/**
	 * GET endpoint that retrieves a jj dispute by id from the database
	 * @param ticketNumber the primary key of the jj dispute to retrieve.
	 * @return a single jj dispute
	 */
	@GetMapping("/dispute/{id}")
	public JJDispute getJJDispute(
			@Parameter(description = "The primary key of the jj dispute to retrieve")
			String id) {
		logger.debug("getJJDispute called");
		return jjDisputeService.getJJDisputeById(id);
	}

	/**
	 * GET endpoint that retrieves a jj dispute by id from the database
	 * @param ticketNumber the primary key of the jj dispute to retrieve.
	 * @return a single jj dispute
	 */
	@GetMapping("/dispute/getJJDisputeToReview/{id}")
	public ResponseEntity<JJDispute> getJJDisputeToReview(
			@Parameter(description = "The primary key of the jj dispute to retrieve")
			String id,
			@Parameter(description = "The VTC User this will be assigned to")
			@AuthenticationPrincipal User user) {
		logger.debug("getJJDispute called");
		
		/* Assign VTC User */
		if (!jjDisputeService.assignJJDisputeToVtc(id, user)) {
			return new ResponseEntity<>(null, HttpStatus.CONFLICT);
		}

		return new ResponseEntity<JJDispute>(jjDisputeService.getJJDisputeById(id), HttpStatus.OK);
	}

	/**
	 * GET endpoint that retrieves all the jj disputes optionally filtered by jjAssignedTo from the database
	 * @param jjAssignedTo if specified, will filter the result set to those assigned to the specified jj staff.
	 * @return list of all jj disputes
	 */
	@GetMapping("/disputes")
	public List<JJDispute> getAllJJDisputes(
			@RequestParam(required = false)
			@Parameter(description = "If specified, will retrieve the records which are assigned to the specified jj staff")
			String jjAssignedTo) {
		logger.debug("getAllJJDisputes called");

		return jjDisputeService.getAllJJDisputes(jjAssignedTo);
	}
	
	/**
	 * PUT endpoint that updates the JJ Dispute detail with administrative resolution details for each JJ Disputed Count, setting the new value for the fields passed in the body.
	 *
	 * @param jj dispute to be updated
	 * @param id (ticket number) of the saved {@link JJDispute} to update
	 * @return updated {@link JJDispute}
	 */
	@Operation(summary = "Updates the properties of a particular JJ Dispute record based on the given values.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok"),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "404", description = "JJDispute record not found. Update failed."),
		@ApiResponse(responseCode = "405", description = "An invalid JJ Dispute status is provided. Update failed.")
	})
	@PutMapping("/dispute/{ticketNumber}")
	public JJDispute updateJJDispute(@PathVariable String ticketNumber, JJDispute jjDispute, @AuthenticationPrincipal User user) {
		logger.debug("PUT /dispute/{ticketNumber} called");
		return jjDisputeService.updateJJDispute(ticketNumber, jjDispute, (CustomUserDetails) user);
	}
	
	/**
	 * PUT endpoint that updates the JJ Dispute status for a JJ Disputed Count by a VTC, setting the new value for the fields passed in the body.
	 *
	 * @param jj dispute to be updated
	 * @param id (ticket number) of the saved {@link JJDispute} to update
	 * @return updated {@link JJDispute}
	 */
	@Operation(summary = "Updates the status of a particular JJ Dispute record based on the given values.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok"),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "404", description = "JJDispute record not found. Update failed."),
		@ApiResponse(responseCode = "405", description = "An invalid JJ Dispute status is provided. Update failed.")
	})
	@PutMapping("/dispute/reviewJJDecision/{ticketNumber}")
	public ResponseEntity<JJDispute> reviewJJDecision(@PathVariable String ticketNumber, @RequestBody JJDispute jjDispute, @AuthenticationPrincipal User user) {
		logger.debug("PUT /dispute/reviewJJDecision/{ticketNumber} called");
		
		/* Can only be updated by assigned VTC */
		if (!jjDisputeService.assignJJDisputeToVtc(ticketNumber, user)) {
			return new ResponseEntity<>(null, HttpStatus.CONFLICT);
		}

		return new ResponseEntity<JJDispute>(jjDisputeService.updateJJDispute(ticketNumber, jjDispute, (CustomUserDetails) user), HttpStatus.OK);
	}
}
