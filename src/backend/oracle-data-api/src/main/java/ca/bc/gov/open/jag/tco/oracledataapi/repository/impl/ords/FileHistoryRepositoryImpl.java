package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.ords;

import java.util.ArrayList;
import java.util.List;
import java.util.function.Supplier;
import java.util.stream.Collectors;

import javax.ws.rs.InternalServerErrorException;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.stereotype.Repository;

import ca.bc.gov.open.jag.tco.oracledataapi.mapper.AuditLogEntryMapper;
import ca.bc.gov.open.jag.tco.oracledataapi.model.FileHistory;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.AuditLogEntryApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.handler.ApiException;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.AuditLogEntryListResponse;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.AuditLogEntryResponseResult;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.FileHistoryRepository;
import net.logstash.logback.argument.StructuredArguments;

@Qualifier("fileHistoryRepository")
@Repository
public class FileHistoryRepositoryImpl implements FileHistoryRepository{

	private static Logger logger = LoggerFactory.getLogger(FileHistoryRepositoryImpl.class);

	// Delegate, OpenAPI generated client
	private final AuditLogEntryApi auditLogEntryApi;

	public FileHistoryRepositoryImpl(AuditLogEntryApi auditLogEntryApi) {
		this.auditLogEntryApi = auditLogEntryApi;
	}

	@Override
	public List<FileHistory> findByTicketNumber(String ticketNumber) {

		AuditLogEntryListResponse response = auditLogEntryApi.v1AuditLogEntryListGet(ticketNumber);

		List<FileHistory> fileHistoriesToReturn = new ArrayList<FileHistory>();
		if (response != null && !response.getAuditLogEntries().isEmpty()) {
			logger.debug("Successfully returned audit log entries from ORDS");

			fileHistoriesToReturn = response.getAuditLogEntries().stream()
					.map(auditLog -> AuditLogEntryMapper.INSTANCE.convert(auditLog))
					.collect(Collectors.toList());
		}

		return fileHistoriesToReturn;
	}

	@Override
	public Long save(FileHistory fileHistory) {
		if (fileHistory == null) {
			throw new IllegalArgumentException("FileHistory body is null.");
		}

		ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.AuditLogEntry auditLogEntry = AuditLogEntryMapper.INSTANCE.convert(fileHistory);
		try {
			AuditLogEntryResponseResult result = assertNoExceptions(() -> auditLogEntryApi.v1ProcessAuditLogEntryPost(auditLogEntry));
			if (result.getAuditLogEntryId() != null) {
				logger.debug("Successfully saved the audit log entry through ORDS");
				return Long.valueOf(result.getAuditLogEntryId()).longValue();
			}
		} catch (ApiException e) {
			logger.error("ERROR inserting FileHistory to ORDS with data: {}", StructuredArguments.fields(fileHistory), e);
			throw new InternalServerErrorException(e);
		}

		return null;
	}

	/**
	 * A helper method that will throw an appropriate InternalServerErrorException based on the AuditLogEntryResponseResult. Any RuntimeExceptions throw will propagate up to caller.
	 * @return
	 */
	private AuditLogEntryResponseResult assertNoExceptions(Supplier<AuditLogEntryResponseResult> m) {
		AuditLogEntryResponseResult result = m.get();

		if (result == null) {
			// Missing response object.
			throw new InternalServerErrorException("Invalid AuditLogEntryResponseResult object");
		} else if (result.getException() != null) {
			// Exception in response exists
			throw new InternalServerErrorException(result.getException());
		} else if (!"1".equals(result.getStatus())) {
			// Status is not 1 (success)
			throw new InternalServerErrorException("Status is not 1 (success)");
		} else {
			return result;
		}
	}

}
