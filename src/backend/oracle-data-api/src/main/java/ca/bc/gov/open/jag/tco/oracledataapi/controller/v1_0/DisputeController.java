package ca.bc.gov.open.jag.tco.oracledataapi.controller.v1_0;

import java.security.Principal;
import java.util.Date;
import java.util.List;

import javax.validation.Valid;
import javax.validation.constraints.NotBlank;
import javax.validation.constraints.Pattern;
import javax.validation.constraints.Size;

import org.apache.commons.lang3.StringUtils;
import org.apache.commons.lang3.time.DateFormatUtils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeResult;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequest;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequestStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.service.DisputeService;
import ca.bc.gov.open.jag.tco.oracledataapi.service.LookupService;
import ca.bc.gov.open.jag.tco.oracledataapi.util.DateUtil;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;

@RestController(value = "DisputeControllerV1_0")
@RequestMapping("/api/v1.0")
@Validated
public class DisputeController {

	@Autowired
	private DisputeService disputeService;

	@Autowired
	private LookupService lookupService;

	private Logger logger = LoggerFactory.getLogger(DisputeController.class);

	/**
	 * GET endpoint that retrieves all the dispute detail from the database
	 * @param olderThan if specified, will filter the result set to those older than this date.
	 * @return list of all dispute tickets
	 */
	@Operation(summary = "Returns all Dispute records based on the specified optional parameters.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok. Disputes are returned."),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "500", description = "Internal Server Error. Getting disputes failed.")
	})
	@GetMapping("/disputes")
	public ResponseEntity<List<Dispute>> getAllDisputes(
			@RequestParam(required = false)
			@DateTimeFormat(pattern = "yyyy-MM-dd")
			@Parameter(description = "If specified, will retrieve records older than this date (specified by yyyy-MM-dd)", example = "2022-03-15")
			Date olderThan,
			@RequestParam(required = false)
			@Parameter(description = "If specified, will retrieve records which do not have the specified status", example = "CANCELLED")
			DisputeStatus excludeStatus) {
		logger.debug("GET /disputes called");
		List<Dispute> allDisputes = disputeService.getAllDisputes(olderThan, excludeStatus);
		return new ResponseEntity<List<Dispute>>(allDisputes, HttpStatus.OK);
	}

	/**
	 * GET endpoint that retrieves the detail of a specific dispute
	 *
	 * @param id
	 * @param principal the logged-in user
	 * @return {@link Dispute}
	 */
	@GetMapping("/dispute/{id}/{isAssign}")
	public ResponseEntity<Dispute> getDispute(
			@PathVariable Long id, 
			@PathVariable Boolean isAssign,
			Principal principal) {
		logger.debug("GET /dispute/{}/{} called", id, isAssign);
		if (isAssign == true) { // only assign if parameter is true
			if (!disputeService.assignDisputeToUser(id, principal)) {
				return new ResponseEntity<>(null, HttpStatus.CONFLICT);
			}
		}
		return new ResponseEntity<Dispute>(disputeService.getDisputeById(id), HttpStatus.OK);
	}
	
	/**
	 * GET endpoint that finds Dispute statuses by TicketNumber and IssuedTime.
	 *
	 * @param ticketNumber of the Dispute.ticketNumber to search for
	 * @param issuedTime the time portion of the Dispute.issuedTs field
	 * @return {@link Dispute}
	 */
	@Operation(summary = "Finds Dispute statuses by TicketNumber and IssuedTime or noticeOfDisputeGuid if specified.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok."),
		@ApiResponse(responseCode = "400", description = "Bad Request, check parameters."),
		@ApiResponse(responseCode = "500", description = "Internal Server Error. Search failed.")
	})
	@GetMapping("/dispute/status")
	public ResponseEntity<List<DisputeResult>> findDisputeStatuses(
			@RequestParam(required = false)
			@Pattern(regexp = "^$|([A-Z]{2}\\d{8})")
			@Parameter(description = "The Dispute.ticketNumber to search for (of the format XX00000000)", example = "AX12345678")
			String ticketNumber,
			@RequestParam(required = false)
			@DateTimeFormat(pattern="HH:mm")
			@Parameter(description = "The time portion of the Dispute.issuedTs field to search for (of the format HH:mm). Time is in UTC.", example = "14:53", schema = @Schema(type="string"))
			Date issuedTime,
			@RequestParam(required = false)
			@Parameter(description = "The noticeOfDisputeGuid of the Dispute to retreive.")
			String noticeOfDisputeGuid) {
		logger.debug("GET /disputes/status?ticketNumber={}&issuedTime={}&noticeOfDisputeGuid={} called", ticketNumber, issuedTime == null ? "" : DateFormatUtils.format(issuedTime, DateUtil.TIME_FORMAT), noticeOfDisputeGuid);
		if (StringUtils.isBlank(ticketNumber) && issuedTime == null && StringUtils.isBlank(noticeOfDisputeGuid)) {
			throw new IllegalArgumentException("Either ticketNumber/time or noticeOfDisputeGuid must be specified.");
		}
		else if (!StringUtils.isBlank(ticketNumber) && issuedTime == null) {
			throw new IllegalArgumentException("If ticketNumber is specified, so must issuedTime.");
		}
		else if (StringUtils.isBlank(ticketNumber) && issuedTime != null) {
			throw new IllegalArgumentException("If issuedTime is specified, so must ticketNumber.");
		}
		List<DisputeResult> results = disputeService.findDisputeStatuses(ticketNumber, issuedTime, noticeOfDisputeGuid);
		logger.debug("  found {} record(s).", results.size());
		return new ResponseEntity<List<DisputeResult>>(results, HttpStatus.OK);
	}

	/**
	 * DELETE endpoint that deletes a specified dispute
	 *
	 * @param id of the {@link Dispute} to be deleted
	 */
	@Operation(summary = "Deletes a particular Dispute record.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok. Dispute record deleted."),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "404", description = "Dispute record not found. Delete failed."),
		@ApiResponse(responseCode = "500", description = "Internal Server Error. Delete failed.")
	})
	@DeleteMapping("/dispute/{id}")
	public void deleteDispute(@PathVariable Long id) {
		logger.debug("DELETE /dispute/{} called", id);
		disputeService.delete(id);
	}

	/**
	 * POST endpoint that post the dispute detail to save in the database
	 *
	 * @param dispute to be saved
	 * @return id of the saved {@link Dispute}
	 */
	@PostMapping("/dispute")
	public Long saveDispute(@RequestBody Dispute dispute) {
		logger.debug("POST /dispute called");
		return disputeService.save(dispute).getDisputeId();
	}

	/**
	 * PUT endpoint that updates the dispute detail, setting the status to REJECTED.
	 *
	 * @param dispute to be updated
	 * @param id of the saved {@link Dispute} to update
	 * @param rejectedReason the note explaining why the status was set to REJECTED.
	 * @param principal the logged-in user
	 * @return {@link Dispute}
	 */
	@Operation(summary = "Updates the status of a particular Dispute record to REJECTED.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok. Updated Dispute record returned."),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "404", description = "Dispute record not found. Update failed."),
		@ApiResponse(responseCode = "405", description = "A Dispute status can only be set to REJECTED iff status is NEW or VALIDATED and the rejected reason must be <= 256 characters. Update failed."),
		@ApiResponse(responseCode = "409", description = "The Dispute has already been assigned to a different user. Dispute cannot be modified until assigned time expires.")
	})
	@PutMapping("/dispute/{id}/reject")
	public ResponseEntity<Dispute> rejectDispute(@PathVariable Long id, @Valid @RequestBody @NotBlank @Size(min = 1, max = 256) String rejectedReason,
			Principal principal) {
		logger.debug("PUT /dispute/{id}/reject called");
		if (!disputeService.assignDisputeToUser(id, principal)) {
			return new ResponseEntity<>(null, HttpStatus.CONFLICT);
		}
		return new ResponseEntity<Dispute>(disputeService.setStatus(id, DisputeStatus.REJECTED, rejectedReason), HttpStatus.OK);
	}

	/**
	 * PUT endpoint that updates the dispute detail, setting the status to VALIDATED.
	 *
	 * @param dispute to be updated
	 * @param id of the saved {@link Dispute} to update
	 * @param principal the logged-in user
	 * @return {@link Dispute}
	 */
	@Operation(summary = "Updates the status of a particular Dispute record to VALIDATED.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok. Updated Dispute record returned."),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "404", description = "Dispute record not found. Update failed."),
		@ApiResponse(responseCode = "405", description = "A Dispute status can only be set to VALIDATED iff status is NEW. Update failed."),
		@ApiResponse(responseCode = "409", description = "The Dispute has already been assigned to a different user. Dispute cannot be modified until assigned time expires.")
	})
	@PutMapping("/dispute/{id}/validate")
	public ResponseEntity<Dispute> validateDispute(@PathVariable Long id, Principal principal) {
		logger.debug("PUT /dispute/{}/validate called", id);
		if (!disputeService.assignDisputeToUser(id, principal)) {
			return new ResponseEntity<>(null, HttpStatus.CONFLICT);
		}
		return new ResponseEntity<Dispute>(disputeService.setStatus(id, DisputeStatus.VALIDATED), HttpStatus.OK);
	}

	@Operation(summary = "Retrieves Dispute by the noticeOfDisputeGuid (UUID).")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok. Dispute retrieved."),
		@ApiResponse(responseCode = "404", description = "Dispute could not be found."),
		@ApiResponse(responseCode = "500", description = "Internal server error occured.")
	})
	@GetMapping("/dispute/noticeOfDispute/{id}")
	public ResponseEntity<Dispute> getDisputeByNoticeOfDisputeGuid(
			@PathVariable(name = "id") @Parameter(description = "The noticeOfDisputeGuid of the Dispute to retreive.") String noticeOfDisputeGuid) {
		logger.debug("GET /dispute/noticeOfDispute/{id} called");
		try {
			Dispute dispute = disputeService.getDisputeByNoticeOfDisputeGuid(noticeOfDisputeGuid);
			if (dispute == null) {
				return new ResponseEntity<>(null, HttpStatus.NOT_FOUND);
			}
			return new ResponseEntity<Dispute>(dispute, HttpStatus.OK);
		} catch (Exception e) {
			logger.error("ERROR retrieving Dispute with noticeOfDisputeGuid {}", noticeOfDisputeGuid, e);
			return new ResponseEntity<>(null, HttpStatus.INTERNAL_SERVER_ERROR);
		}
	}

	@Operation(summary = "Updates the emailVerification flag of a particular Dispute to true.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok. Email verified."),
		@ApiResponse(responseCode = "404", description = "Dispute with specified id not found"),
		@ApiResponse(responseCode = "500", description = "Internal server error occured.")
	})
	@PutMapping("/dispute/{id}/email/verify")
	public ResponseEntity<Void> verifyDisputeEmail(@PathVariable(name="id") @Parameter(description = "The id of the Dispute to update.") Long id) {
		logger.debug("PUT /dispute/{}/email/verify called", id);
		disputeService.verifyEmail(id);
		return ResponseEntity.ok().build();
	}

	@Operation(summary = "Updates the email address of a particular Dispute.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok. Email reset."),
		@ApiResponse(responseCode = "400", description = "If the email address is > 100 characters"),
		@ApiResponse(responseCode = "404", description = "Dispute with specified id not found"),
		@ApiResponse(responseCode = "500", description = "Internal server error occured.")
	})
	@PutMapping("/dispute/{id}/email/reset")
	public ResponseEntity<Dispute> resetDisputeEmail(
			@PathVariable(name="id")
			@Parameter(description = "The id of the Dispute to update.")
			Long id,
			@RequestParam(name="email", required = false)
			@Size(min = 1, max = 100)
			@Parameter(description = "The new email address of the Disputant.")
			String email) {
		logger.debug("PUT /dispute/{}/email/reset called", id);
		return new ResponseEntity<Dispute>(disputeService.resetEmail(id, email), HttpStatus.OK);
	}

	/**
	 * PUT endpoint that updates the dispute detail, setting the status to CANCELLED.
	 *
	 * @param dispute to be updated
	 * @param id of the saved {@link Dispute} to update
	 * @param principal the logged-in user
	 * @return {@link Dispute}
	 */
	@Operation(summary = "Updates the status of a particular Dispute record to CANCELLED.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok. Updated Dispute record returned."),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "404", description = "Dispute record not found. Update failed."),
		@ApiResponse(responseCode = "405", description = "A Dispute status can only be set to CANCELLED iff status is NEW, REJECTED or PROCESSING. Update failed."),
		@ApiResponse(responseCode = "409", description = "The Dispute has already been assigned to a different user. Dispute cannot be modified until assigned time expires.")
	})
	@PutMapping("/dispute/{id}/cancel")
	public ResponseEntity<Dispute> cancelDispute(@PathVariable Long id, Principal principal) {
		logger.debug("PUT /dispute/{id}/cancel called");
		if (!disputeService.assignDisputeToUser(id, principal)) {
			return new ResponseEntity<>(null, HttpStatus.CONFLICT);
		}
		return new ResponseEntity<Dispute>(disputeService.setStatus(id, DisputeStatus.CANCELLED), HttpStatus.OK);
	}

	/**
	 * PUT endpoint that updates the dispute detail, setting the status to PROCESSING.
	 *
	 * @param dispute to be updated
	 * @param id of the saved {@link Dispute} to update
	 * @param principal the logged-in user
	 * @return {@link Dispute}
	 */
	@Operation(summary = "Updates the status of a particular Dispute record to PROCESSING.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok. Updated Dispute record returned."),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "404", description = "Dispute record not found. Update failed."),
		@ApiResponse(responseCode = "405", description = "A Dispute status can only be set to PROCESSING iff status is NEW or REJECTED. Update failed."),
		@ApiResponse(responseCode = "409", description = "The Dispute has already been assigned to a different user. Dispute cannot be modified until assigned time expires.")
	})
	@PutMapping("/dispute/{id}/submit")
	public ResponseEntity<Dispute> submitDispute(@PathVariable Long id, Principal principal) {
		logger.debug("PUT /dispute/{id}/submit called");
		if (!disputeService.assignDisputeToUser(id, principal)) {
			return new ResponseEntity<>(null, HttpStatus.CONFLICT);
		}
		return new ResponseEntity<Dispute>(disputeService.setStatus(id, DisputeStatus.PROCESSING), HttpStatus.OK);
	}

	/**
	 * PUT endpoint that updates the dispute detail, setting the new value for the fields passed in the body.
	 *
	 * @param dispute to be updated
	 * @param id of the saved {@link Dispute} to update
	 * @param principal the logged-in user
	 * @return updated {@link Dispute}
	 */
	@Operation(summary = "Updates the properties of a particular Dispute record based on the given values.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok"),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "404", description = "Dispute record not found. Update failed."),
		@ApiResponse(responseCode = "409", description = "The Dispute has already been assigned to a different user. Dispute cannot be modified until assigned time expires.")
	})
	@PutMapping("/dispute/{id}")
	public ResponseEntity<Dispute> updateDispute(@PathVariable Long id, @RequestBody Dispute dispute, Principal principal) {
		logger.debug("PUT /dispute/{} called", id);
		if (!disputeService.assignDisputeToUser(id, principal)) {
			return new ResponseEntity<>(null, HttpStatus.CONFLICT);
		}
		return new ResponseEntity<Dispute>(disputeService.update(id, dispute), HttpStatus.OK);
	}

	/**
	 * GET endpoint that refreshes all codetables cached in redis.  Called by an external cronjob trigger.
	 */
	@GetMapping("/codetable/refresh")
	@Operation(
			summary = "An endpoint hook to trigger a redis rebuild of cached codetable data.",
			description = "The codetables in redis are cached copies of data pulled from Oracle to ensure TCO remains stable. This data is periodically refreshed, but can be forced by hitting this endpoint."
			)
	public void codeTableRefresh() {
		logger.debug("GET /codetable/refresh called");
		lookupService.refresh();
	}

	/**
	 * GET endpoint that unassigns all Disputes that were assigned for more than 1 hour.  Called by an external cronjob trigger.
	 */
	@GetMapping("/disputes/unassign")
	@Operation(
			summary = "An endpoint hook to trigger the Unassign Dispute job.",
			description = "A Dispute can be assigned to a specific user that \"locks\" the record for others. This endpoing manually triggers the Unassign Dispute job that clears the assignment of all Disputes that were assigned for more than 1 hour."
			)
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok. Dispute record unassigned."),
		@ApiResponse(responseCode = "500", description = "Internal Server Error. Unassigned failed.")
	})
	public void unassignDisputes() {
		logger.debug("GET /disputes/unassign called");
		disputeService.unassignDisputes();
	}

	/**
	 * POST endpoint that inserts a DisputeUpdateRequest into persistent storage linked to the Dispute identified by the noticeOfDisputeGuid.
	 *
	 * @param dispute to be saved
	 * @return id of the saved {@link DisputeUpdateRequest}
	 */
	@PostMapping("/dispute/{guid}/updateRequest")
	@Operation(summary = "An endpoint that inserts a DisputeUpdateRequest into persistent storage.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok. DisputeUpdateRequest record saved."),
		@ApiResponse(responseCode = "404", description = "Dispute could not be found."),
		@ApiResponse(responseCode = "500", description = "Internal Server Error. Save failed.")
	})
	public ResponseEntity<Long> saveDisputeUpdateRequest(
			@PathVariable(name = "guid")
			@Parameter(description = "The noticeOfDisputeGuid of the Dispute to associate with.")
			String noticeOfDisputeGuid,
			@RequestBody
			DisputeUpdateRequest disputeUpdateRequest) {
		logger.debug("POST /dispute/{}/updateRequest called", noticeOfDisputeGuid);

		Long disputeUpdateRequestId = disputeService.saveDisputeUpdateRequest(noticeOfDisputeGuid, disputeUpdateRequest).getDisputeUpdateRequestId();
		return new ResponseEntity<Long>(disputeUpdateRequestId, HttpStatus.OK);
	}

	/**
	 * Get endpoint that retrieves all <code>DisputeUpdateRequest</code>s from persistent storage that is associated with the optionally given Dispute via optional disputeId.
	 *
	 * @param disputeUpdateRequest id to be retrieved
	 * @return id of the saved {@link DisputeUpdateRequest}
	 */
	@GetMapping("/dispute/updateRequests")
	@Operation(summary = "An endpoint that retrieves all DisputeUpdateRequest optionally for a given Dispute, optionally filtered by DisputeUpdateRequestStatus.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok. DisputeUpdateRequest record saved."),
		@ApiResponse(responseCode = "500", description = "Internal Server Error. Save failed.")
	})
	public ResponseEntity<List<DisputeUpdateRequest>> getDisputeUpdateRequests(
			@RequestParam(required=false)
			@Parameter(description = "If specified, filter request by the disputeId of the Dispute.")
			Long id,
			@RequestParam(required = false)
			@Parameter(description = "If specified, filter request by DisputeUpdateRequestStatus", example = "PENDING")
			DisputeUpdateRequestStatus status) {
		logger.debug("GET /dispute/updateRequests called", id, status);
		return new ResponseEntity<List<DisputeUpdateRequest>>(disputeService.findDisputeUpdateRequestByDisputeIdAndStatus(id, status), HttpStatus.OK);
	}

	/**
	 * PUT endpoint that updates the status of a DisputeUpdateRequest record.
	 *
	 * @param disputeUpdateRequestStatus to be saved
	 * @return id of the saved {@link DisputeUpdateRequest}
	 */
	@PutMapping("/dispute/updateRequest/{id}")
	@Operation(summary = "An endpoint that updates the status of a DisputeUpdateRequest record.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok. DisputeUpdateRequest updated."),
		@ApiResponse(responseCode = "404", description = "DisputeUpdateRequest could not be found."),
		@ApiResponse(responseCode = "500", description = "Internal Server Error. Save failed.")
	})
	public ResponseEntity<DisputeUpdateRequest> updateDisputeUpdateRequestStatus(
			@PathVariable(name = "id")
			@Parameter(description = "The id of the DisputeUpdateRequest record to update.")
			Long updateRequestId,
			@RequestParam
			@Parameter(description = "The status of the request record should be updated to.", example = "ACCEPTED")
			DisputeUpdateRequestStatus disputeUpdateRequestStatus) {
		logger.debug("PUT /dispute/updateRequest/{} called", updateRequestId);

		DisputeUpdateRequest disputeUpdateRequest = disputeService.updateDisputeUpdateRequest(updateRequestId, disputeUpdateRequestStatus);
		return new ResponseEntity<DisputeUpdateRequest>(disputeUpdateRequest, HttpStatus.OK);
	}

}
