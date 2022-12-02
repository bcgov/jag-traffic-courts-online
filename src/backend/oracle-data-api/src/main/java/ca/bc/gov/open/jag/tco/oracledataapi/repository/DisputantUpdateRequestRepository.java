package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import java.util.List;

import org.springframework.data.jpa.repository.JpaRepository;

import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputantUpdateRequest;

public interface DisputantUpdateRequestRepository extends JpaRepository<DisputantUpdateRequest, Long> {

	/** Fetch all records that matches the DisputeId. */
	public List<DisputantUpdateRequest> findByDisputeId(Long disputeId);

}
