package ca.bc.gov.open.jag.tco.oracledataapi.controller.v1_0;

import java.util.Date;
import java.util.List;

import javax.validation.Valid;
import javax.validation.constraints.NotBlank;
import javax.validation.constraints.Size;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.service.DisputeService;
import ca.bc.gov.open.jag.tco.oracledataapi.service.LookupService;
import io.swagger.v3.oas.annotations.Operation;
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
	 *
	 * @return list of all dispute tickets
	 */
	@GetMapping("/disputes")
	public List<Dispute> getAllDisputes() {
		log.debug("Retrieve all disputes endpoint is called." + new Date());
		return disputeService.getAllDisputes();
	}

	/**
	 * GET endpoint that retrieves the detail of a specific dispute
	 *
	 * @param id
	 * @return {@link Dispute}
	 */
	@GetMapping("/dispute/{id}")
	public Dispute getDispute(@PathVariable Integer id) {
		return disputeService.getDisputeById(id);
	}

	/**
	 * DELETE endpoint that deletes a specified dispute
	 *
	 * @param id of the {@link Dispute} to be deleted
	 */
	@DeleteMapping("/dispute/{id}")
	public void deleteDispute(@PathVariable Integer id) {
		disputeService.delete(id);
	}

	/**
	 * POST endpoint that post the dispute detail to save in the database
	 *
	 * @param dispute to be saved
	 * @return id of the saved {@link Dispute}
	 */
	@PostMapping("/dispute")
	public Integer saveDispute(@RequestBody Dispute dispute) {
		disputeService.save(dispute);
		return dispute.getId();
	}

	/**
	 * PUT endpoint that updates the dispute detail, setting the status to REJECTED.
	 *
	 * @param dispute to be updated
	 * @param id of the saved {@link Dispute} to update
	 * @return {@link Dispute}
	 */
	@Operation(summary = "Updates the status of a particular Dispute record to REJECTED.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok"),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "404", description = "Dispute record not found. Update failed."),
		@ApiResponse(responseCode = "405", description = "A Dispute status can only be set to REJECTED iff status is NEW, CANCELLED, or REJECTED and the rejected reason must be <= 256 characters. Update failed.")
	})
	@PutMapping("/dispute/{id}/reject")
	public void rejectDispute(@PathVariable Integer id, @Valid @RequestBody @NotBlank @Size(min=1, max=256) String rejectedReason) {
		disputeService.setStatus(id, DisputeStatus.REJECTED, rejectedReason);
	}

	/**
	 * PUT endpoint that updates the dispute detail, setting the status to CANCELLED.
	 *
	 * @param dispute to be updated
	 * @param id of the saved {@link Dispute} to update
	 * @return {@link Dispute}
	 */
	@Operation(summary = "Updates the status of a particular Dispute record to CANCELLED.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok"),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "404", description = "Dispute record not found. Update failed."),
		@ApiResponse(responseCode = "405", description = "A Dispute status can only be set to CANCELLED iff status is REJECTED or PROCESSING. Update failed.")
	})
	@PutMapping("/dispute/{id}/cancel")
	public void cancelDispute(@PathVariable Integer id) {
		disputeService.setStatus(id, DisputeStatus.CANCELLED);
	}

	/**
	 * PUT endpoint that updates the dispute detail, setting the status to PROCESSING.
	 *
	 * @param dispute to be updated
	 * @param id of the saved {@link Dispute} to update
	 * @return {@link Dispute}
	 */
	@Operation(summary = "Updates the status of a particular Dispute record to PROCESSING.")
	@ApiResponses({
		@ApiResponse(responseCode = "200", description = "Ok"),
		@ApiResponse(responseCode = "400", description = "Bad Request."),
		@ApiResponse(responseCode = "404", description = "Dispute record not found. Update failed."),
		@ApiResponse(responseCode = "405", description = "A Dispute status can only be set to PROCESSING iff status is NEW or PROCESSING. Update failed.")
	})
	@PutMapping("/dispute/{id}/submit")
	public void submitDispute(@PathVariable Integer id) {
		disputeService.setStatus(id, DisputeStatus.PROCESSING);
	}

	/**
	 * GET endpoint that refreshes all codetables cached in redis
	 */
	@GetMapping("/codetable/refresh")
	@Operation(
			summary = "An endpoint hook to trigger a redis rebuild of cached codetable data.",
			description = "The codetables in redis are cached copies of data pulled from Oracle to ensure TCO remains stable. This data is periodically refreshed, but can be forced by hitting this endpoint."
			)
	public void codeTableRefresh() {
		lookupService.refresh();
	}

}
