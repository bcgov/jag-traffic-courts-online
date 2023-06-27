package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import java.util.List;

import ca.bc.gov.open.jag.tco.oracledataapi.model.EmailHistory;

public interface EmailHistoryRepository {

	/**
	 * Fetch all email history records based on the given ticket number.
	 *
	 * @param ticketNumber
	 * @return list of {@link EmailHistory}
	 */
	public List<EmailHistory> findByTicketNumber(String ticketNumber);

	/**
	 * Saves an entity and flushes changes instantly.
	 *
	 * @param entity entity to be saved. Must not be {@literal null}.
	 * @return id of the saved entity
	 */
	public Long save(EmailHistory entity);
}
