package ca.bc.gov.open.jag.tco.oracledataapi.controller.v1_0;

import java.util.List;

import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import ca.bc.gov.open.jag.tco.oracledataapi.error.DeprecatedException;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import io.swagger.v3.oas.annotations.Operation;

@RestController(value = "DisputeControllerV1_0")
@RequestMapping("/api/v1.0")
public class DisputeController {

	/**
	 * GET endpoint that retrieves all the dispute detail from the database
	 *
	 * @return list of all dispute tickets
	 */
	@GetMapping("/disputes")
	@Deprecated
	public List<Dispute> getAllDisputes() {
		throw new DeprecatedException();
	}

	/**
	 * GET endpoint that retrieves the detail of a specific dispute
	 *
	 * @param disputeId
	 * @return {@link Dispute}
	 */
	@GetMapping("/dispute/{disputeId}")
	@Deprecated
	public Dispute getDispute(@PathVariable("disputeId") int disputeId) {
		throw new DeprecatedException();
	}

	/**
	 * DELETE endpoint that deletes a specified dispute
	 *
	 * @param id of the {@link Dispute} to be deleted
	 */
	@DeleteMapping("/dispute/{disputeId}")
	@Deprecated
	public void deleteDispute(@PathVariable("disputeId") int disputeId) {
		throw new DeprecatedException();
	}

	/**
	 * POST endpoint that post the dispute detail to save in the database
	 *
	 * @param dispute to be saved
	 * @return id of the saved {@link Dispute}
	 */
	@PostMapping("/dispute")
	@Deprecated
	public int saveDispute(@RequestBody Dispute dispute) {
		throw new DeprecatedException();
	}

	/**
	 * PUT endpoint that updates the dispute detail
	 *
	 * @param dispute to be updated
	 * @return {@link Dispute}
	 */
	@PutMapping("/dispute")
	@Deprecated
	public Dispute update(@RequestBody Dispute dispute) {
		throw new DeprecatedException();
	}

	/**
	 * GET endpoint that refreshes all codetables cached in redis
	 */
	@GetMapping("/codetable/refresh")
	@Operation(summary = "An endpoint hook to trigger a redis rebuild of cached codetable data.", description = "The codetables in redis are cached copies of data pulled from Oracle to ensure TCO remains stable. This data is periodically refreshed, but can be forced by hitting this endpoint.")
	@Deprecated
	public void codeTableRefresh() {
		throw new DeprecatedException();
	}

}
