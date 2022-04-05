package ca.bc.gov.open.jag.tco.oracledatainterface.controller;

import java.util.Date;
import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import ca.bc.gov.open.jag.tco.oracledatainterface.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledatainterface.service.DisputeService;
import ca.bc.gov.open.jag.tco.oracledatainterface.service.LookupService;
import io.swagger.v3.oas.annotations.Operation;

@RestController
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
		log.info("Retrieve all disputes endpoint is called." + new Date());
		return disputeService.getAllDisputes();
	}

	/**
	 * GET endpoint that retrieves the detail of a specific dispute
	 *
	 * @param disputeId
	 * @return {@link Dispute}
	 */
	@GetMapping("/dispute/{disputeId}")
	public Dispute getDispute(@PathVariable("disputeId") int disputeId) {
		return disputeService.getDisputeById(disputeId);
	}

	/**
	 * DELETE endpoint that deletes a specified dispute
	 *
	 * @param id of the {@link Dispute} to be deleted
	 */
	@DeleteMapping("/dispute/{disputeId}")
	public void deleteDispute(@PathVariable("disputeId") int disputeId) {
		disputeService.delete(disputeId);
	}

	/**
	 * POST endpoint that post the dispute detail to save in the database
	 *
	 * @param dispute to be saved
	 * @return id of the saved {@link Dispute}
	 */
	@PostMapping("/dispute")
	public int saveDispute(@RequestBody Dispute dispute) {
		disputeService.saveOrUpdate(dispute);
		return dispute.getId();
	}

	/**
	 * PUT endpoint that updates the dispute detail
	 *
	 * @param dispute to be updated
	 * @return {@link Dispute}
	 */
	@PutMapping("/dispute")
	public Dispute update(@RequestBody Dispute dispute) {
		disputeService.saveOrUpdate(dispute);
		return dispute;
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
