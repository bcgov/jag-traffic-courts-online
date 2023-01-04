package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import java.util.List;

import org.springframework.data.jpa.repository.JpaRepository;

import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputantUpdateRequest;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputantUpdateRequestStatus;

public interface DisputantUpdateRequestRepository extends JpaRepository<DisputantUpdateRequest, Long> {

	/** Fetch all records that match the DisputeId. */
	public List<DisputantUpdateRequest> findByDisputeId(Long disputeId);

	/** Fetch all records that match the Status. */
	public List<DisputantUpdateRequest> findByStatus(DisputantUpdateRequestStatus status);
}
