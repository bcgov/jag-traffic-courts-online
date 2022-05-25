package ca.bc.gov.open.jag.tco.oracledataapi.controller.v1_0;

import java.security.Principal;
import java.util.Date;
import java.util.List;
import java.util.UUID;

import javax.validation.Valid;
import javax.validation.constraints.NotBlank;
import javax.validation.constraints.Size;

import org.apache.commons.collections4.IterableUtils;
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
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.service.DisputeService;
import ca.bc.gov.open.jag.tco.oracledataapi.service.LookupService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
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

	private Logger log = LoggerFactory.getLogger(DisputeController.class);

	/**
	 * GET endpoint that retrieves all the dispute detail from the database
	 * @param olderThan if specified, will filter the result set to those older than this date.
	 * @return list of all dispute tickets
	 */
	@GetMapping("/disputes")
	public List<Dispute> getAllDisputes(
			@RequestParam(required = false)
			@DateTimeFormat(iso = DateTimeFormat.ISO.DATE)
			@Parameter(description = "If specified, will retrieve records older than this date (specified by yyyy-MM-dd)", example = "2022-03-15")
			Date olderThan) {
		Iterable<Dispute> allDisputes = disputeService.getAllDisputes(olderThan);
		// Swagger doesn't seem to know what an Iterable<Dispute> object is. Convert to an actual instantiated list to return a collection.
		return IterableUtils.toList(allDisputes);
	}

	/**
	 * GET endpoint that retrieves the detail of a specific dispute
	 *
	 * @param id
	 * @param principal the logged-in user
	 * @return {@link Dispute}
	 */
	@GetMapping("/dispute/{id}")
	public ResponseEntity<Dispute> getDispute(@PathVariable UUID id, Principal principal) {
		if (!disputeService.assignDisputeToUser(id, principal)) {
			return new ResponseEntity<>(null, HttpStatus.CONFLICT);
		}
		return new ResponseEntity<Dispute>(disputeService.getDisputeById(id), HttpStatus.OK);
	}

	/**
	 * DELETE endpoint that deletes a specified dispute
	 *
	 * @param id of the {@link Dispute} to be deleted
	 */
	@DeleteMapping("/dispute/{id}")
	public void deleteDispute(@PathVariable UUID id) {
		disputeService.delete(id);
	}

	/**
	 * POST endpoint that post the dispute detail to save in the database
	 *
	 * @param dispute to be saved
	 * @return id of the saved {@link Dispute}
	 */
	@PostMapping("/dispute")
	public UUID saveDispute(@RequestBody Dispute dispute) {
		disputeService.save(dispute);
		return dispute.getId();
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
		@ApiResponse(responseCode = "405", description = "A Dispute status can only be set to REJECTED iff status is NEW and the rejected reason must be <= 256 characters. Update failed."),
		@ApiResponse(responseCode = "409", description = "The Dispute has already been assigned to a different user. Dispute cannot be modified until assigned time expires.")
	})
	@PutMapping("/dispute/{id}/reject")
	public ResponseEntity<Dispute> rejectDispute(@PathVariable UUID id, @Valid @RequestBody @NotBlank @Size(min = 1, max = 256) String rejectedReason,
			Principal principal) {
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
	public ResponseEntity<Dispute> validateDispute(@PathVariable UUID id, Principal principal) {
		if (!disputeService.assignDisputeToUser(id, principal)) {
			return new ResponseEntity<>(null, HttpStatus.CONFLICT);
		}
		return new ResponseEntity<Dispute>(disputeService.setStatus(id, DisputeStatus.VALIDATED), HttpStatus.OK);
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
	public ResponseEntity<Dispute> cancelDispute(@PathVariable UUID id, Principal principal) {
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
	public ResponseEntity<Dispute> submitDispute(@PathVariable UUID id, Principal principal) {
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
	public ResponseEntity<Dispute> updateDispute(@PathVariable UUID id, @RequestBody Dispute dispute, Principal principal) {
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
	public void unassignDisputes() {
		disputeService.unassignDisputes();
	}

}
