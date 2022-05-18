package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import java.util.Date;
import java.util.UUID;

import org.springframework.data.repository.CrudRepository;

import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;

public interface DisputeRepository extends CrudRepository<Dispute, UUID> {

	/** Fetch all records older than the given date. */
    public Iterable<Dispute> findByCreatedTsBefore(Date olderThan);

	/** Fetch all records whose assignedTs has a timestamp older than the given date. */
    public Iterable<Dispute> findByAssignedTsBefore(Date assignedTs);

}
