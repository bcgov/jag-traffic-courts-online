package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import java.util.List;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;

import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputantUpdateRequest;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputantUpdateRequestStatus;

public interface DisputantUpdateRequestRepository extends JpaRepository<DisputantUpdateRequest, Long> {

	/** Fetch all records that match the DisputeId. */
	public List<DisputantUpdateRequest> findByDisputeId(Long disputeId);
	
	/** Fetch all records that matches the optional DisputeId, optionally filtered by status. */
	@Query("from DisputantUpdateRequest where (:disputeId is null or disputeId = :disputeId) and (:status is null or status = :status)")
	public List<DisputantUpdateRequest> findByOptionalDisputeIdAndOptionalStatus(Long disputeId, DisputantUpdateRequestStatus status);
}
