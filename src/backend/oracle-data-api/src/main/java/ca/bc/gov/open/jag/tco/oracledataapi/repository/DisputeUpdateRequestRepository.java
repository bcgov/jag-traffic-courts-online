package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import java.util.List;
import java.util.NoSuchElementException;
import java.util.Optional;

import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequest;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequestStatus;

public interface DisputeUpdateRequestRepository {

	/** Fetch all records that matches the optional DisputeId, optionally filtered by status. */
	public List<DisputeUpdateRequest> findByOptionalDisputeIdAndOptionalStatus(Long disputeId, DisputeUpdateRequestStatus status);

	/**
	 * Saves a given entity. Use the returned instance for further operations as the save operation might have changed the entity instance completely.
	 *
	 * @param entity must not be {@literal null}.
	 * @return the saved entity; will never be {@literal null}.
	 * @throws IllegalArgumentException in case the given {@literal entity} is {@literal null}.
	 */
	public DisputeUpdateRequest save(DisputeUpdateRequest entity);

	/**
	 * Retrieves an entity by its id.
	 *
	 * @param id must not be {@literal null}.
	 * @return the entity with the given id or {@literal Optional#empty()} if none found.
	 * @throws IllegalArgumentException if {@literal id} is {@literal null}.
	 */
	public Optional<DisputeUpdateRequest> findById(Long id);

	/**
	 * Updates the given disputeUpdateRequest entity.
	 *
	 * @param disputeUpdateRequest entity to be updated. Must not be {@literal null}.
	 * @return the updated entity
	 */
	public DisputeUpdateRequest update(DisputeUpdateRequest disputeUpdateRequest);


	/**
	 * Deletes the entity with the given id.
	 *
	 * @param id must not be {@literal null}.
	 * @throws NoSuchElementException if a Dispute with the given {@literal id} is not found.
	 * @throws IllegalArgumentException in case the given {@literal id} is {@literal null}
	 */
	public void deleteById(Long id);
}
