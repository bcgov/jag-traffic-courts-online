package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.h2;

import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.data.jpa.repository.JpaRepository;

import ca.bc.gov.open.jag.tco.oracledataapi.model.EmailHistory;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.EmailHistoryRepository;

@ConditionalOnProperty(name = "repository.history", havingValue = "h2", matchIfMissing = true)
@Qualifier("emailHistoryRepository")
public interface EmailHistoryRepositoryImpl extends EmailHistoryRepository, JpaRepository<EmailHistory, Long> {
}
