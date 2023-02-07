package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import java.util.List;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;

import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequest;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequestStatus;

public interface DisputeUpdateRequestRepository extends JpaRepository<DisputeUpdateRequest, Long> {

	/** Fetch all records that match the DisputeId. */
	public List<DisputeUpdateRequest> findByDisputeId(Long disputeId);
	
	/** Fetch all records that matches the optional DisputeId, optionally filtered by status. */
	@Query("from DisputeUpdateRequest where (:disputeId is null or disputeId = :disputeId) and (:status is null or status = :status)")
	public List<DisputeUpdateRequest> findByOptionalDisputeIdAndOptionalStatus(Long disputeId, DisputeUpdateRequestStatus status);
}
