package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import java.util.List;

import ca.bc.gov.open.jag.tco.oracledataapi.model.EmailHistory;

public interface EmailHistoryRepository {

	/** Fetch all records older than the given date. */
	public List<EmailHistory> findByTicketNumber(String ticketNumber);

	/**
	 * Deletes the entity with the given id.
	 *
	 * @param id must not be {@literal null}.
	 * @throws IllegalArgumentException in case the given {@literal id} is {@literal null}
	 */
	public void deleteById(Long id);

	/**
	 * Returns all instances of the type.
	 *
	 * @return all entities
	 */
	public List<EmailHistory> findAll();

	/**
	 * Saves an entity and flushes changes instantly.
	 *
	 * @param entity entity to be saved. Must not be {@literal null}.
	 * @return the saved entity
	 */
	public EmailHistory saveAndFlush(EmailHistory entity);

}
