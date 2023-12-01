package ca.bc.gov.open.jag.tco.oracledataapi.controller.v1_0;

import java.security.Principal;
import java.util.List;

import javax.validation.constraints.Pattern;
import javax.validation.constraints.Size;

import org.apache.commons.lang3.StringUtils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.TicketImageDataDocumentType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.TicketImageDataJustinDocument;
import ca.bc.gov.open.jag.tco.oracledataapi.service.JJDisputeService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import net.logstash.logback.argument.StructuredArguments;

@RestController(value = "JJDisputeControllerV1_0")
@RequestMapping("/api/v1.0/jj")
public class JJDisputeController {

	@Autowired
	private JJDisputeService jjDisputeService;

	private Logger logger = LoggerFactory.getLogger(JJDisputeController.class);

	/**
	 * GET endpoint that retrieves a jj dispute by ticketNumber from the database
	 * without assigning it to a VTC for review
	 * @param ticketNumber the primary key of the jj dispute to retrieve
	 * @param checkVTCAssigned, boolean (optional) check assignment to VTC
	 * @param principal logged in user to assign
	 * @return a single jj dispute
	 */
	@GetMapping("/dispute/{ticketNumber}/{assignVTC}")
	public ResponseEntity<JJDispute> getJJDispute(
			@Parameter(description = "The primary key of the jj dispute to retrieve")
			@PathVariable("ticketNumber") String ticketNumber,
			@PathVariable("assignVTC") boolean checkVTCAssigned,
			Principal principal) {
		logger.debug("GET /jj/dispute/{}/{} called", StructuredArguments.value("ticketNumber", ticketNumber), StructuredArguments.value("checkVTCAssigned", checkVTCAssigned));

		if (checkVTCAssigned && !jjDisputeService.assignJJDisputeToVtc(ticketNumber, principal)) {
			return new ResponseEntity<>(null, HttpStatus.CONFLICT);
		}

		return new ResponseEntity<JJDispute>(jjDisputeService.getJJDisputeByTicketNumber(ticketNumber), HttpStatus.OK);
	}

	/**
	 * GET endpoint that retrieves all the jj disputes optionally filtered by jjAssignedTo from the database
	 * @param jjAssignedTo if specified, will filter the result set to those assigned to the specified jj staff.
	 * @param ticketNumber (Optional). Used with ViolationTime, if specified will filter by TicketNumber. (Format is XX00000000)
	 * @return list of all jj disputes
	 */
	@GetMapping("/disputes")
	public List<JJDispute> getJJDisputes(
			@RequestParam(required = false)
			@Parameter(description = "If specified, will retrieve the records which are assigned to the specified jj staff")
			String jjAssignedTo,
			@RequestParam(required = false)
			@Pattern(regexp = "[A-Z]{2}\\d{8}")
			@Parameter(description = "If specified will filter by TicketNumber. (Format is XX00000000)", example = "AX12345678")
			String ticketNumber) {
		logger.debug("GET /jj/disputes called");

		return jjDisputeService.getJJDisputes(jjAssignedTo, ticketNumber);
	}

	/**
	 * PUT endpoint that updates the JJ Dispute detail with administrative resolution details for each JJ Disputed Count, setting the new value for the fields passed in the body.
	 *
	 * @param jj dispute to be updated
	 * @param ticketNumber unique key of the saved {@link JJDispute} to update
	 * @param principal user doing the updating
	 * @param boolean (optional) check assignment to VTC
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
	public ResponseEntity<JJDispute> updateJJDispute(
			@PathVariable("ticketNumber") String ticketNumber,
			boolean checkVTCAssigned,
			Principal principal,
			@RequestBody JJDispute jjDispute) {
		logger.debug("PUT /jj/dispute/{} called", StructuredArguments.value("ticketNumber", ticketNumber));

		if (checkVTCAssigned && !jjDisputeService.assignJJDisputeToVtc(ticketNumber, principal)) {
			return new ResponseEntity<>(null, HttpStatus.CONFLICT);
		}

		return new ResponseEntity<JJDispute>(jjDisputeService.updateJJDispute(ticketNumber, jjDispute, principal), HttpStatus.OK);
	}

	/**
	 * PUT endpoint that updates a JJ Dispute as well as underlying Dispute data.
	 *
	 * @param jj dispute to be updated
	 * @param ticketNumber unique key of the saved {@link JJDispute} to update
	 * @param principal user doing the updating
	 * @param boolean (optional) check assignment to VTC
	 * @return updated {@link JJDispute}
	 */
	@Operation(summary = "Updates the properties of a particular JJ Dispute record as well as cascading to underlying related Dispute data.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok"),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "404", description = "JJDispute record not found. Update failed."),
		@ApiResponse(responseCode = "405", description = "An invalid JJ Dispute status is provided. Update failed.")
	})
	@PutMapping("/dispute/{ticketNumber}/cascade")
	public ResponseEntity<JJDispute> updateJJDisputeCascade(
			@PathVariable("ticketNumber") String ticketNumber,
			boolean checkVTCAssigned,
			Principal principal,
			@RequestBody JJDispute jjDispute) {
		logger.debug("PUT /jj/dispute/{}/cascade called", StructuredArguments.value("ticketNumber", ticketNumber));

		if (checkVTCAssigned && !jjDisputeService.assignJJDisputeToVtc(ticketNumber, principal)) {
			return new ResponseEntity<>(null, HttpStatus.CONFLICT);
		}

		return new ResponseEntity<JJDispute>(jjDisputeService.updateJJDisputeCascade(ticketNumber, jjDispute, principal), HttpStatus.OK);
	}

	/**
	 * PUT endpoint that updates each JJ Dispute based on the passed in ticket number to assign them to a specific JJ or unassign them if JJ not specified.
	 *
	 * @param ticketNumberList unique keys of JJ Dispute(s) to be updated/assigned
	 * @param jjUsername IDIR username of the JJ that JJ Dispute(s) will be assigned to, if specified. Otherwise JJ Disputes will be unassigned
	 */
	@Operation(summary = "Updates each JJ Dispute based on the passed in ticket numbers to assign them to a specific JJ or unassign them if JJ not specified.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok"),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "404", description = "JJDispute record(s) not found. Update failed."),
		@ApiResponse(responseCode = "500", description = "Internal server error occured.")
	})
	@PutMapping("/dispute/assign")
	public ResponseEntity<Void> assignJJDisputesToJJ(
			@RequestParam("ticketNumbers") List<String> ticketNumberList,
			@RequestParam(required = false, name = "jjUsername") String jjUsername) {
		logger.debug("PUT /jj/dispute/assign called");

		jjDisputeService.assignJJDisputesToJJ(ticketNumberList, jjUsername);

		return ResponseEntity.ok().build();
	}

	/**
	 * PUT endpoint that updates the JJDispute, setting the status to REVIEW.
	 *
	 * @param ticketNumber, unique key of the saved {@link JJDispute} to update
	 * @param remark, the note explaining why the status was set to REVIEW.
	 * @param checkVTCAssigned, boolean (optional) check assignment to VTC
	 * @param principal, the logged-in user
	 * @param recalled, indicates the dispute is re-opened by a JJ
	 * @return {@link JJDispute}
	 */
	@Operation(summary = "Updates the status of a particular JJDispute record to REVIEW.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok. Updated JJDispute record returned."),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "404", description = "JJDispute record not found. Update failed."),
		@ApiResponse(responseCode = "405", description = "A JJDispute status can only be set to REVIEW if status is CONFIRMED and the remark must be <= 256 characters OR "
				+ "if the status ACCEPTED, CONFIRMED or CONCLUDED and DCF's current hearing date = today's date. Update failed."),
		@ApiResponse(responseCode = "500", description = "Internal server error occured.")
	})
	@PutMapping("/dispute/{ticketNumber}/review")
	public ResponseEntity<JJDispute> reviewJJDispute(@PathVariable String ticketNumber,
			@RequestBody (required = false) @Size(max = 256) String remark,
			boolean checkVTCAssigned,
			Principal principal,
			boolean recalled) {
		logger.debug("PUT /jj/dispute/{}/review called", StructuredArguments.value("ticketNumber", ticketNumber));

		if (checkVTCAssigned && !jjDisputeService.assignJJDisputeToVtc(ticketNumber, principal)) {
			return new ResponseEntity<>(null, HttpStatus.CONFLICT);
		}

		return new ResponseEntity<JJDispute>(jjDisputeService.setStatus(ticketNumber, JJDisputeStatus.REVIEW, principal, remark, null, recalled), HttpStatus.OK);
	}

	/**
	 * PUT endpoint that updates the JJDispute, setting the status to CONCLUDED.
	 *
	 * @param ticketNumber, unique key of the saved {@link JJDispute} to update
	 * @param checkVTCAssigned, boolean (optional) check assignment to VTC
	 * @param principal, the logged-in user
	 * @return {@link JJDispute}
	 */
	@Operation(summary = "Updates the status of a particular JJDispute record to CONCLUDED.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok. Updated JJDispute record returned."),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "404", description = "JJDispute record not found. Update failed."),
		@ApiResponse(responseCode = "500", description = "Internal server error occured.")
	})
	@PutMapping("/dispute/{ticketNumber}/conclude")
	public ResponseEntity<JJDispute> concludeJJDispute(@PathVariable String ticketNumber,
			boolean checkVTCAssigned,
			Principal principal) {
		logger.debug("PUT /jj/dispute/{}/conclude called", StructuredArguments.value("ticketNumber", ticketNumber));

		if (checkVTCAssigned && !jjDisputeService.assignJJDisputeToVtc(ticketNumber, principal)) {
			return new ResponseEntity<>(null, HttpStatus.CONFLICT);
		}

		return new ResponseEntity<JJDispute>(jjDisputeService.setStatus(ticketNumber, JJDisputeStatus.CONCLUDED, principal, null, null, false), HttpStatus.OK);
	}
	/**
	 * PUT endpoint that updates the JJDispute, setting the status to CANCELLED.
	 *
	 * @param ticketNumber, unique key of the saved {@link JJDispute} to update
	 * @param checkVTCAssigned, boolean (optional) check assignment to VTC
	 * @param principal, the logged-in user
	 * @return {@link JJDispute}
	 */
	@Operation(summary = "Updates the status of a particular JJDispute record to CANCELLED.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok. Updated JJDispute record returned."),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "404", description = "JJDispute record not found. Update failed."),
		@ApiResponse(responseCode = "500", description = "Internal server error occured.")
	})
	@PutMapping("/dispute/{ticketNumber}/cancel")
	public ResponseEntity<JJDispute> cancelJJDispute(@PathVariable String ticketNumber,
			boolean checkVTCAssigned,
			Principal principal) {
		logger.debug("PUT /jj/dispute/{}/cancel called", StructuredArguments.value("ticketNumber", ticketNumber));

		if (checkVTCAssigned && !jjDisputeService.assignJJDisputeToVtc(ticketNumber, principal)) {
			return new ResponseEntity<>(null, HttpStatus.CONFLICT);
		}

		return new ResponseEntity<JJDispute>(jjDisputeService.setStatus(ticketNumber, JJDisputeStatus.CANCELLED, principal, null, null, false), HttpStatus.OK);
	}

	/**
	 * PUT endpoint that updates the JJDispute, setting the status to REQUIRE_COURT_HEARING and hearing Type to COURT_APPEARANCE.
	 *
	 * @param ticketNumber, unique key of the saved {@link JJDispute} to update
	 * @param remark, the note explaining why the status was set
	 * @param principal, the logged-in user
	 * @return {@link JJDispute}
	 */
	@Operation(summary = "Updates the status of a particular JJDispute record to REQUIRE_COURT_HEARING, type to COURT_APPEARANCE.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok. Updated JJDispute record returned."),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "404", description = "JJDispute record not found. Update failed."),
		@ApiResponse(responseCode = "405", description = "A JJDispute status can only be set to REQUIRE_COURT_HEARING iff status is one of the following: "
				+ "NEW, IN_PROGRESS, REVIEW, REQUIRE_COURT_HEARING and the remark must be <= 256 characters. Update failed."),
		@ApiResponse(responseCode = "500", description = "Internal server error occured.")
	})
	@PutMapping("/dispute/{ticketNumber}/requirecourthearing")
	public ResponseEntity<JJDispute> requireCourtHearingJJDispute(@PathVariable String ticketNumber,
			@RequestParam (required = false) @Size(max = 256) String remark,
			Principal principal) {
		logger.debug("PUT /jj/dispute/{}/requirecourthearing called", StructuredArguments.value("ticketNumber", ticketNumber));

		return new ResponseEntity<JJDispute>(jjDisputeService.requireCourtHearing(ticketNumber, principal, remark), HttpStatus.OK);
	}

	/**
	 * PUT endpoint that updates the JJDispute, setting the status to ACCEPTED.
	 *
	 * @param ticketNumber, unique key of the saved {@link JJDispute} to update
	 * @param checkVTCAssigned, boolean (optional) check assignment to VTC
	 * @param principal, the logged-in user
	 * @return {@link JJDispute}
	 */
	@Operation(summary = "Updates the status of a particular JJDispute record to ACCEPTED.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok. Updated JJDispute record returned."),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "404", description = "JJDispute record not found. Update failed."),
		@ApiResponse(responseCode = "405", description = "A JJDispute status can only be set to ACCEPTED iff status is CONFIRMED. Update failed."),
		@ApiResponse(responseCode = "500", description = "Internal server error occured.")
	})
	@PutMapping("/dispute/{ticketNumber}/accept")
	public ResponseEntity<JJDispute> acceptJJDispute(@PathVariable String ticketNumber,
			boolean checkVTCAssigned,
			Principal principal,
			@RequestParam(required = false) @Parameter(description = "Adjudicator's participant ID") String partId) {

		logger.debug("PUT /jj/dispute/{}/accept called", StructuredArguments.value("ticketNumber", ticketNumber));

		if (checkVTCAssigned && !jjDisputeService.assignJJDisputeToVtc(ticketNumber, principal)) {
			return new ResponseEntity<>(null, HttpStatus.CONFLICT);
		}

		return new ResponseEntity<JJDispute>(jjDisputeService.setStatus(ticketNumber, JJDisputeStatus.ACCEPTED, principal, null, partId, false), HttpStatus.OK);
	}

	/**
	 * PUT endpoint that updates the JJDispute, setting the status to CONFIRMED.
	 *
	 * @param ticketNumber, a unique key of the saved {@link JJDispute} to update
	 * @param principal, the logged-in user
	 * @return {@link JJDispute}
	 */
	@Operation(summary = "Updates the status of a particular JJDispute record to CONFIRMED.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok. Updated JJDispute record returned."),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "404", description = "JJDispute record not found. Update failed."),
		@ApiResponse(responseCode = "405", description = "A JJDispute status can only be set to CONFIRMED iff status is one of the following: "
				+ "REVIEW, NEW, HEARING_SCHEDULED, IN_PROGRESS, CONFIRMED. Update failed."),
		@ApiResponse(responseCode = "500", description = "Internal server error occured.")
	})
	@PutMapping("/dispute/{ticketNumber}/confirm")
	public ResponseEntity<JJDispute> confirmJJDispute(@PathVariable String ticketNumber, Principal principal) {
		logger.debug("PUT /jj/dispute/{}/confirm called", StructuredArguments.value("ticketNumber", ticketNumber));

		return new ResponseEntity<JJDispute>(jjDisputeService.setStatus(ticketNumber, JJDisputeStatus.CONFIRMED, principal, null, null, false), HttpStatus.OK);
	}

	/**
	 * GET endpoint that retrieves a justin document
	 * @param disputeId the primary key of the jj dispute to retrieve
	 * @param documentType of justin document to retrieve
	 * @param principal logged in user to assign
	 * @return a single justin document
	 */
	@GetMapping("/dispute/ticketImage/{ticketNumber}/{documentType}")
	public ResponseEntity<TicketImageDataJustinDocument> getTicketImageData(
			@PathVariable("ticketNumber") String ticketNumber,
			@PathVariable("documentType") TicketImageDataDocumentType documentType,
			Principal principal) {
		logger.debug("GET /jj/dispute/ticketImage/{}/{} called", StructuredArguments.value("ticketNumber", ticketNumber), StructuredArguments.value("documentType", documentType));

		return new ResponseEntity<TicketImageDataJustinDocument>(jjDisputeService.getTicketImageByTicketNumber(ticketNumber, documentType), HttpStatus.OK);
	}

	/**
	 * DELETE endpoint that deletes a specified {@link JJDispute} as well as its associated data
	 *
	 * @param id of the {@link JJDispute} to be deleted
	 * @param ticketNumber of the {@link JJDispute} to be deleted
	 */
	@Operation(summary = "Deletes a particular JJDispute record.", operationId = "DeleteJJDispute")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok. JJ Dispute record deleted."),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "500", description = "Internal Server Error. Delete failed.")
	})
	@DeleteMapping("/dispute")
	public void deleteDispute(
			@RequestParam(required = false)
			@Parameter(description = "If specified, will delete the record of the specified jj dispute by this ID. JJ Dispute ID will take precedence if both ID and ticket number supplied")
			Long jjDisputeId,
			@RequestParam(required = false)
			@Pattern(regexp = "[A-Z]{2}\\d{8}")
			@Parameter(description = "If specified, will delete the record of the specified jj dispute by this TicketNumber. (Format is XX00000000)", example = "AX12345678")
			String ticketNumber) {
		logger.debug("DELETE /jj/dispute called with jjDisputeId: {}, and ticketNumber: {}", StructuredArguments.value("jjDisputeId", jjDisputeId), StructuredArguments.value("ticketNumber", ticketNumber));
		if (jjDisputeId == null && StringUtils.isBlank(ticketNumber)) {
			throw new IllegalArgumentException("Either ticketNumber or jjDisputeId must be specified.");
		}

		jjDisputeService.delete(jjDisputeId, ticketNumber);
	}
}
