package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.ords;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutionException;
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
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.AuditLogEntryResponseResult;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.FileHistoryRepository;
import net.logstash.logback.argument.StructuredArguments;

@Qualifier("fileHistoryRepository")
@Repository
public class FileHistoryRepositoryImpl implements FileHistoryRepository{

	private static Logger logger = LoggerFactory.getLogger(FileHistoryRepositoryImpl.class);

	// Delegate, OCCAM OpenAPI generated client
	private final AuditLogEntryApi occamAuditLogEntryApi;
	
	// Delegate, TCO OpenAPI generated client
	private final ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.AuditLogEntryApi tcoAuditLogEntryApi;

	public FileHistoryRepositoryImpl(AuditLogEntryApi auditLogEntryApi, ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.AuditLogEntryApi tcoAuditLogEntryApi) {
		this.occamAuditLogEntryApi = auditLogEntryApi;
		this.tcoAuditLogEntryApi = tcoAuditLogEntryApi;
	}
	
	
	@Override
	public List<FileHistory> findByTicketNumber(String ticketNumber) {
	    List<FileHistory> fileHistoriesToReturn = new ArrayList<>();

	    // Fetch audit log entries in parallel
	    CompletableFuture<ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.AuditLogEntryListResponse> occamFuture = CompletableFuture.supplyAsync(() -> 
	        occamAuditLogEntryApi.v1AuditLogEntryListGet(ticketNumber)
	    );
	    
	    CompletableFuture<ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.AuditLogEntryListResponse> tcoFuture = CompletableFuture.supplyAsync(() -> 
	        tcoAuditLogEntryApi.v1AuditLogEntryListGet(ticketNumber)
	    );

	    try {
	        // Wait for both responses to complete
	        ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.AuditLogEntryListResponse occamResponse = occamFuture.get();
	        ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.AuditLogEntryListResponse tcoResponse = tcoFuture.get();

	        // Process occamResponse
	        if (occamResponse != null && !occamResponse.getAuditLogEntries().isEmpty()) {
	            List<FileHistory> occamFileHistories = occamResponse.getAuditLogEntries().stream()
	                .map(auditLog -> AuditLogEntryMapper.INSTANCE.convert(auditLog))
	                .collect(Collectors.toList());
	            fileHistoriesToReturn.addAll(occamFileHistories);
	        }

	        // Process tcoResponse
	        if (tcoResponse != null && !tcoResponse.getAuditLogEntries().isEmpty()) {
	            List<FileHistory> tcoFileHistories = tcoResponse.getAuditLogEntries().stream()
	                .map(auditLog -> AuditLogEntryMapper.INSTANCE.convert(auditLog))
	                .collect(Collectors.toList());
	            fileHistoriesToReturn.addAll(tcoFileHistories);
	        }

	    } catch (InterruptedException | ExecutionException | ApiException | ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.handler.ApiException e) {
	    	logger.error("Error fetching audit log entries for ticket number: " + ticketNumber, e);
	    }
	    
	    return fileHistoriesToReturn;
	}

	@Override
	public Long save(FileHistory fileHistory) {
		if (fileHistory == null) {
			throw new IllegalArgumentException("FileHistory body is null.");
		}
		
		if ("OCCAM".equals(fileHistory.determineSchema())) {
			ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.AuditLogEntry auditLogEntry = AuditLogEntryMapper.INSTANCE.convertToOccamAuditLogEntry(fileHistory);
			try {
				AuditLogEntryResponseResult result = assertNoExceptions(() -> occamAuditLogEntryApi.v1ProcessAuditLogEntryPost(auditLogEntry));
				if (result.getAuditLogEntryId() != null) {
					logger.debug("Successfully saved the audit log entry through ORDS");
					return Long.valueOf(result.getAuditLogEntryId()).longValue();
				}
			} catch (ApiException e) {
				logger.error("ERROR inserting FileHistory to ORDS with data: {}", StructuredArguments.fields(fileHistory), e);
				throw new InternalServerErrorException(e);
			}
		} else {
			ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.AuditLogEntry auditLogEntry = AuditLogEntryMapper.INSTANCE.convertToTcoAuditLogEntry(fileHistory);
			try {
				ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.AuditLogEntryResponseResult result = assertNoTcoExceptions(() -> tcoAuditLogEntryApi.v1ProcessAuditLogEntryPost(auditLogEntry));
				if (result.getAuditLogEntryId() != null) {
					logger.debug("Successfully saved the audit log entry through ORDS");
					return Long.valueOf(result.getAuditLogEntryId()).longValue();
				}
			} catch (ApiException e) {
				logger.error("ERROR inserting FileHistory to ORDS with data: {}", StructuredArguments.fields(fileHistory), e);
				throw new InternalServerErrorException(e);
			}
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
	
	/**
	 * A helper method that will throw an appropriate InternalServerErrorException based on the AuditLogEntryResponseResult. Any RuntimeExceptions throw will propagate up to caller.
	 * @return
	 */
	private ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.AuditLogEntryResponseResult assertNoTcoExceptions(Supplier<ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.AuditLogEntryResponseResult> m) {
		ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.AuditLogEntryResponseResult result = m.get();

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
