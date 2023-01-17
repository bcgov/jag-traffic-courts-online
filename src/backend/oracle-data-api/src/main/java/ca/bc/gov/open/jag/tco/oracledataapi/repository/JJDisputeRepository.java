package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import java.util.Date;
import java.util.List;
import java.util.Optional;

import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;

public interface JJDisputeRepository {

	/** Fetch all records which have the specified jjAssigned. */
	public List<JJDispute> findByJjAssignedToIgnoreCase(String jjAssigned);

	/** Fetch all records whose assignedTs has a timestamp older than the given date. */
	public Iterable<JJDispute> findByVtcAssignedTsBefore(Date olderThan);

	/**
	 * Deletes all entities managed by the repository.
	 */
	public void deleteAll();

	/**
	 * Returns all instances of the type.
	 *
	 * @return all entities
	 */
	public Iterable<JJDispute> findAll();

	/**
	 * Retrieves an entity by its id.
	 *
	 * @param id must not be {@literal null}.
	 * @return the entity with the given id or {@literal Optional#empty()} if none found.
	 * @throws IllegalArgumentException if {@literal id} is {@literal null}.
	 */
	public Optional<JJDispute> findById(String id);

	/** Fetch all records that match by JJDispute.ticketNumber (should only ever be one). */
	public List<JJDispute> findByTicketNumber(String ticketNumber);

	/**
	 * Saves a given entity. Use the returned instance for further operations as the save operation might have changed the entity instance completely.
	 *
	 * @param entity must not be {@literal null}.
	 * @return the saved entity; will never be {@literal null}.
	 * @throws IllegalArgumentException in case the given {@literal entity} is {@literal null}.
	 */
	public JJDispute save(JJDispute entity);

	/**
	 * Saves an entity and flushes changes instantly.
	 *
	 * @param entity entity to be saved. Must not be {@literal null}.
	 * @return the saved entity
	 */
	public JJDispute saveAndFlush(JJDispute entity);

	/**
	 * Sets the status field on the given JJDispute.
	 *
	 * @param ticketNumber
	 * @param jjDisputeStatus
	 * @param userName
	 */
	public void setStatus(String ticketNumber, JJDisputeStatus jjDisputeStatus, String userName);

}
