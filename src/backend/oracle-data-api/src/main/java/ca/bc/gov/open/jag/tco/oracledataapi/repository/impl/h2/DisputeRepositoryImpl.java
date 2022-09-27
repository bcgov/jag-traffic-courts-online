package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.h2;

import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.data.jpa.repository.JpaRepository;

import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputeRepository;

@ConditionalOnProperty(name = "ords.enabled", havingValue = "false", matchIfMissing = true)
@Qualifier("disputeRepository")
public interface DisputeRepositoryImpl extends DisputeRepository, JpaRepository<Dispute, Long> {
}
