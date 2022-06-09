package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import java.util.List;

import org.springframework.data.repository.CrudRepository;

import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;

public interface JJDisputeRepository extends CrudRepository<JJDispute, String>{

	/** Fetch all records which have the specified jjAssigned. */
    public List<JJDispute> findByJjAssignedToIgnoreCase(String jjAssigned);
    
    /** Fetch all records which have the specified jjGroupAssigned. */
    public List<JJDispute> findByJjGroupAssignedToIgnoreCase(String jjGroupAssigned);
    
    /** Fetch all records which have the specified jjGroupAssigned. */
    public List<JJDispute> findByJjGroupAssignedToIgnoreCaseAndJjAssignedToIgnoreCase(String jjGroupAssigned, String jjAssigned);
}
