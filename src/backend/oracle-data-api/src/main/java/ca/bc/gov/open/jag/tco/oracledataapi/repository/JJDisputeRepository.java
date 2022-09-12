package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import java.util.Date;
import java.util.List;
import java.util.Optional;

import org.springframework.data.jpa.repository.JpaRepository;

import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;

public interface JJDisputeRepository extends JpaRepository<JJDispute, String>{

	/** Fetch all records which have the specified jjAssigned. */
    public List<JJDispute> findByJjAssignedToIgnoreCase(String jjAssigned);

    /** Fetch all records whose assignedTs has a timestamp older than the given date. */
    public Iterable<JJDispute> findByVtcAssignedTsBefore(Date olderThan);

    /** Fetch a record for given ticket number. */
	public Optional<JJDispute> findByTicketNumber(String ticketNumber);
}
