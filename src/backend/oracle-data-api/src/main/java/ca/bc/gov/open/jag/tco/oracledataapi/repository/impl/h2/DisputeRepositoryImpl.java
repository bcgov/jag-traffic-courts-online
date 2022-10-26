package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.h2;

import java.util.Date;
import java.util.List;

import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Modifying;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.transaction.annotation.Transactional;

import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeResult;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputeRepository;

@ConditionalOnProperty(name = "ords.enabled", havingValue = "false", matchIfMissing = true)
@Qualifier("disputeRepository")
public interface DisputeRepositoryImpl extends DisputeRepository, JpaRepository<Dispute, Long>, DisputeRepositoryCustom {

	@Override
	@Query("select new ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeResult(d.id, d.status) from Dispute d where d.ticketNumber = :ticketNumber and hour(d.issuedTs) = hour(:issuedTime) and minute(d.issuedTs) = minute(:issuedTime)")
	public List<DisputeResult> findByTicketNumberAndTime(@Param(value = "ticketNumber") String ticketNumber, @Param(value = "issuedTime") Date issuedTime);

	@Override
	@Modifying
	@Query("update Dispute d set d.userAssignedTo = :userName, d.userAssignedTs = CURRENT_TIMESTAMP() where d.disputeId = :disputeId")
	@Transactional
	public void assignDisputeToUser(@Param(value = "disputeId") Long disputeId, @Param(value = "userName") String userName);

	@Override
	@Modifying
	@Query("update Dispute d set d.userAssignedTo = null, d.userAssignedTs = null where d.userAssignedTs < :olderThan")
	@Transactional
	public void unassignDisputes(@Param(value="olderThan") Date olderThan);

	@Override
	@Modifying
	@Query("update Dispute d set d.status = :disputeStatus, d.rejectedReason = :rejectedReason where d.disputeId = :disputeId")
	@Transactional
	public void setStatus(
			@Param(value = "disputeId") Long disputeId,
			@Param(value = "disputeStatus") DisputeStatus disputeStatus,
			@Param(value = "rejectedReason") String rejectedReason);

}
