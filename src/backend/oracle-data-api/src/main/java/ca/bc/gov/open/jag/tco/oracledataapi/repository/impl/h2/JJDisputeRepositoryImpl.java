package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.h2;

import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.data.jpa.repository.JpaRepository;

import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRepository;

@ConditionalOnProperty(name = "repository.jjdispute", havingValue = "h2", matchIfMissing = true)
@Qualifier("jjDisputeRepository")
public interface JJDisputeRepositoryImpl extends JJDisputeRepository, JpaRepository<JJDispute, String> {
}
