package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import java.util.List;

import org.springframework.data.jpa.repository.JpaRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.model.EmailHistory;

public interface EmailHistoryRepository extends JpaRepository<EmailHistory, Long> {

	/** Fetch all records older than the given date. */
    public List<EmailHistory> findByTicketNumber(String ticketNumber);

}
