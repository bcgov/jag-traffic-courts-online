package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.h2;

import java.util.Date;
import java.util.List;

import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRepository;

@ConditionalOnProperty(name = "repository.jjdispute", havingValue = "h2", matchIfMissing = true)
@Qualifier("jjDisputeRepository")
public interface JJDisputeRepositoryImpl extends JJDisputeRepository, JpaRepository<JJDispute, String> {

	@Override
	@Query("select jj from JJDispute jj where jj.ticketNumber = :ticketNumber and hour(jj.violationDate) = hour(:violationTime) and minute(jj.violationDate) = minute(:violationTime)")
	public List<JJDispute> findByTicketNumberAndTime(@Param(value = "ticketNumber") String ticketNumber, @Param(value = "violationTime") Date violationTime);

}
