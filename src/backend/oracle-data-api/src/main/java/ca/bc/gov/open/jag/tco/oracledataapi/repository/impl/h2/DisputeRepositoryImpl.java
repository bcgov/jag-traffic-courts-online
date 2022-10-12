package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.h2;

import java.util.Date;

import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Modifying;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.transaction.annotation.Transactional;

import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputeRepository;

@ConditionalOnProperty(name = "ords.enabled", havingValue = "false", matchIfMissing = true)
@Qualifier("disputeRepository")
public interface DisputeRepositoryImpl extends DisputeRepository, JpaRepository<Dispute, Long> {

	@Override
	@Modifying
	@Query("update Dispute d set d.userAssignedTo = null, d.userAssignedTs = null where d.userAssignedTs < :olderThan")
	@Transactional
	public void unassignDisputes(@Param(value="olderThan") Date olderThan);
}
