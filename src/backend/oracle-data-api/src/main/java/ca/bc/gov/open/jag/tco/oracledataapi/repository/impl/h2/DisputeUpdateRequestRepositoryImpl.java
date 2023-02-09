package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.h2;

import java.util.List;

import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;

import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequest;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequestStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputeUpdateRequestRepository;

@ConditionalOnProperty(name = "repository.dispute", havingValue = "h2", matchIfMissing = true)
@Qualifier("disputantUpdateRequestRepository")
public interface DisputeUpdateRequestRepositoryImpl extends DisputeUpdateRequestRepository, JpaRepository<DisputeUpdateRequest, Long> {

	@Override
	@Query("from DisputeUpdateRequest where (:disputeId is null or disputeId = :disputeId) and (:status is null or status = :status)")
	public List<DisputeUpdateRequest> findByOptionalDisputeIdAndOptionalStatus(Long disputeId, DisputeUpdateRequestStatus status);
}
