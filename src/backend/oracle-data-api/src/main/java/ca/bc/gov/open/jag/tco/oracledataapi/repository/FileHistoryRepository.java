package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import java.util.List;

import org.springframework.data.jpa.repository.JpaRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.model.FileHistory;

public interface FileHistoryRepository extends JpaRepository<FileHistory, Long> {

	/** Fetch all records older than the given date. */
    public List<FileHistory> findByTicketNumber(String ticketNumber);

}
