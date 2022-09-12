package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import java.util.List;

import org.springframework.data.jpa.repository.JpaRepository;

import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;

public interface JJDisputeRepository extends JpaRepository<JJDispute, String>{

	/** Fetch all records which have the specified jjAssigned. */
    public List<JJDispute> findByJjAssignedToIgnoreCase(String jjAssigned);
}
