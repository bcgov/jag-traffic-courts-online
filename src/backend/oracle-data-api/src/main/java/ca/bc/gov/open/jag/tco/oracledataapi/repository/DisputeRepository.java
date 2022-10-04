package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import java.util.Date;
import java.util.List;
import java.util.NoSuchElementException;
import java.util.Optional;

import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;

public interface DisputeRepository {

	/** Fetch all records older than the given date. */
    public Iterable<Dispute> findByCreatedTsBefore(Date olderThan);

    /** Fetch all records which do not have the specified status. */
    public Iterable<Dispute> findByStatusNot(DisputeStatus excludeStatus);

    /** Fetch all records which do not have the specified status and older than the given date. */
    public Iterable<Dispute> findByStatusNotAndCreatedTsBefore(DisputeStatus excludeStatus, Date olderThan);

	/** Fetch all records whose assignedTs has a timestamp older than the given date. */
    public Iterable<Dispute> findByUserAssignedTsBefore(Date olderThan);

    /** Fetch all records that matches the emailVerificationToken. */
    public List<Dispute> findByEmailVerificationToken(String emailVerificationToken);

	/** Deletes all entities managed by the repository. */
    public void deleteAll();

	/**
	 * Deletes the entity with the given id.
	 *
	 * @param id must not be {@literal null}.
	 * @throws NoSuchElementException if a Dispute with the given {@literal id} is not found.
	 * @throws IllegalArgumentException in case the given {@literal id} is {@literal null}
	 */
	public void deleteById(Long id);

	/**
	 * Returns all instances of the type.
	 *
	 * @return all entities
	 */
	public Iterable<Dispute> findAll();

	/**
	 * Retrieves an entity by its id.
	 *
	 * @param id must not be {@literal null}.
	 * @return the entity with the given id or {@literal Optional#empty()} if none found.
	 * @throws IllegalArgumentException if {@literal id} is {@literal null}.
	 */
	public Optional<Dispute> findById(Long id);


	/**
	 * Saves a given entity. Use the returned instance for further operations as the save operation might have changed the
	 * entity instance completely.
	 *
	 * @param entity must not be {@literal null}.
	 * @return the saved entity; will never be {@literal null}.
	 * @throws IllegalArgumentException in case the given {@literal entity} is {@literal null}.
	 */
	public Dispute save(Dispute dispute);

	/**
	 * Saves an entity and flushes changes instantly.
	 *
	 * @param entity entity to be saved. Must not be {@literal null}.
	 * @return the saved entity
	 */
	public Dispute saveAndFlush(Dispute entity);

}
