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

import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeCourtAppearanceAPP;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeCourtAppearanceDATT;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.YesNo;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRepository;

@ConditionalOnProperty(name = "repository.jjdispute", havingValue = "h2", matchIfMissing = true)
@Qualifier("jjDisputeRepository")
public interface JJDisputeRepositoryImpl extends JJDisputeRepository, JpaRepository<JJDispute, Long> {

	@Override
	@Modifying(clearAutomatically = true)
	@Query("update JJDispute jj set jj.jjAssignedTo = :username where jj.ticketNumber = :ticketNumber")
	public void assignJJDisputeJj(String ticketNumber, String username);

	@Override
	@Modifying(clearAutomatically = true)
	@Query("update JJDispute jj set jj.vtcAssignedTo = :username, jj.vtcAssignedTs = CURRENT_TIMESTAMP() where jj.ticketNumber = :ticketNumber")
	public void assignJJDisputeVtc(String ticketNumber, String username);

	@Override
	@Modifying(clearAutomatically = true)
	@Query("update JJDispute jj set jj.vtcAssignedTo = null, jj.vtcAssignedTs = null where jj.vtcAssignedTs < :assignedBeforeTs and (:ticketNumber is null or jj.ticketNumber = :ticketNumber)")
	public void unassignJJDisputeVtc(String ticketNumber, Date assignedBeforeTs);

	@Override
	@Query("select jj from JJDispute jj where jj.ticketNumber = :ticketNumber")
	public List<JJDispute> findByTicketNumber(@Param(value = "ticketNumber") String ticketNumber);

	@Override
	@Modifying(clearAutomatically = true)
	@Query("update JJDispute jj set jj.status = :jjDisputeStatus, jj.modifiedBy = :userId, jj.modifiedTs = CURRENT_TIMESTAMP() "
			+ "where jj.id = :jjDisputeId "

			// NOTE: this query does nothing with courtAppearanceId, seized, ... staffPartId as these are not fields in the JJDispute record.
			+ "and (1=1 or :courtAppearanceId is null or :seizedYn is null or :adjudicatorPartId is null or :aattCd is null or :dattCd is null or :staffPartId is null)")
	@Transactional
	public void setStatus(
			@Param(value = "jjDisputeId") Long disputeId,
			@Param(value = "jjDisputeStatus") JJDisputeStatus disputeStatus,
			@Param(value = "userId") String userId,
			@Param(value = "courtAppearanceId") Long courtAppearanceId,
			@Param(value = "seizedYn") YesNo seizedYn,
			@Param(value = "adjudicatorPartId") String adjudicatorPartId,
			@Param(value = "aattCd") JJDisputeCourtAppearanceAPP aattCd,
			@Param(value = "dattCd") JJDisputeCourtAppearanceDATT dattCd,
			@Param(value = "staffPartId") String staffPartId);

}
