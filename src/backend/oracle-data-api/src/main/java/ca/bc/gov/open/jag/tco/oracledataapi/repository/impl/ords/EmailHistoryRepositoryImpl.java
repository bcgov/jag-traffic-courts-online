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

import ca.bc.gov.open.jag.tco.oracledataapi.mapper.OutgoingEmailMapper;
import ca.bc.gov.open.jag.tco.oracledataapi.model.EmailHistory;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.OutgoingEmailApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.handler.ApiException;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.OutgoingEmailListResponse;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.OutgoingEmailResponseResult;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.EmailHistoryRepository;

@Qualifier("disputeRepository")
@Repository
public class EmailHistoryRepositoryImpl implements EmailHistoryRepository {

	private static Logger logger = LoggerFactory.getLogger(EmailHistoryRepositoryImpl.class);

	// Delegate, OpenAPI generated client
	private final OutgoingEmailApi outgoingEmailApi;

	public EmailHistoryRepositoryImpl(OutgoingEmailApi outgoingEmailApi) {
		this.outgoingEmailApi = outgoingEmailApi;
	}

	@Override
	public List<EmailHistory> findByTicketNumber(String ticketNumber) {
		OutgoingEmailListResponse response = outgoingEmailApi.v1OutgoingEmailListGet(ticketNumber);

		List<EmailHistory> outgoingEmailsToReturn = new ArrayList<EmailHistory>();
		if (response != null && !response.getOutgoingEmails().isEmpty()) {
			logger.debug("Successfully returned outgoing email entries from ORDS");

			outgoingEmailsToReturn = response.getOutgoingEmails().stream()
					.map(outgoingEmail -> OutgoingEmailMapper.INSTANCE.convert(outgoingEmail))
					.collect(Collectors.toList());
		}

		return outgoingEmailsToReturn;
	}

	@Override
	public Long save(EmailHistory emailHistory) {
		if (emailHistory == null) {
			throw new IllegalArgumentException("EmailHistory body is null.");
		}

		ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.OutgoingEmail outgoingEmail = OutgoingEmailMapper.INSTANCE.convert(emailHistory);
		try {
			OutgoingEmailResponseResult result = assertNoExceptions(() -> outgoingEmailApi.v1ProcessOutgoingEmailPost(outgoingEmail));
			if (result.getOutgoingEmailId() != null) {
				logger.debug("Successfully saved the outgoing email through ORDS");
				return Long.valueOf(result.getOutgoingEmailId()).longValue();
			}
		} catch (ApiException e) {
			logger.error("ERROR inserting EmailHistory to ORDS with data: {}", emailHistory.toString(), e);
			throw new InternalServerErrorException(e);
		}

		return null;
	}

	/**
	 * A helper method that will throw an appropriate InternalServerErrorException based on the OutgoingEmailResponseResult. Any RuntimeExceptions throw will propagate up to caller.
	 * @return
	 */
	private OutgoingEmailResponseResult assertNoExceptions(Supplier<OutgoingEmailResponseResult> m) {
		OutgoingEmailResponseResult result = m.get();

		if (result == null) {
			// Missing response object.
			throw new InternalServerErrorException("Invalid OutgoingEmailResponseResult object");
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
