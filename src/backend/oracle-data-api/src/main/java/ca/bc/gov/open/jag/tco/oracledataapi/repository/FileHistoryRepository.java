package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import java.util.List;

import ca.bc.gov.open.jag.tco.oracledataapi.model.FileHistory;

public interface FileHistoryRepository {


	/**
	 * Fetches all fileHistory records for the given ticketNumber through ORDS.
	 *
	 * @param ticketNumber
	 * @return List of {@link FileHistory}
	 */
	public List<FileHistory> findByTicketNumber(String ticketNumber);

	/**
	 * Saves a fileHistory entity through ORDS.
	 *
	 * @param fileHistory entity to be saved. Must not be {@literal null}.
	 * @return id of the saved entity
	 */
	public Long save(FileHistory fileHistory);

}
