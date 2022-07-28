package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import java.util.List;

import org.springframework.data.repository.CrudRepository;

import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;

public interface JJDisputeRepository extends CrudRepository<JJDispute, String>{

	/** Fetch all records which have the specified jjAssigned. */
    public List<JJDispute> findByJjAssignedToIgnoreCase(String jjAssigned);
}
