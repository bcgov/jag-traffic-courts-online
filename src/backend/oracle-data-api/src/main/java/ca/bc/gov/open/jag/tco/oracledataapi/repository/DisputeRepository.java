package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import java.util.Date;
import java.util.List;
import java.util.UUID;

import org.springframework.data.repository.CrudRepository;

import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;

public interface DisputeRepository extends CrudRepository<Dispute, UUID> {

	/** Fetch all records older than the given date. */
    public Iterable<Dispute> findByCreatedTsBefore(Date olderThan);

    /** Fetch all records which do not have the specified status. */
    public Iterable<Dispute> findByStatusNot(DisputeStatus excludeStatus);

    /** Fetch all records which do not have the specified status and older than the given date. */
    public Iterable<Dispute> findByStatusNotAndCreatedTsBefore(DisputeStatus excludeStatus, Date olderThan);

	/** Fetch all records whose assignedTs has a timestamp older than the given date. */
    public Iterable<Dispute> findByAssignedTsBefore(Date olderThan);
    
    /** Fetch all records which have the specified jjAssigned. */
    public List<Dispute> findByJjAssignedIgnoreCase(String jjAssigned);

}
