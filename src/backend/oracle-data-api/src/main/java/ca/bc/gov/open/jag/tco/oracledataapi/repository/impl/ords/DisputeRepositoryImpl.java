package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.ords;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.List;
import java.util.NoSuchElementException;
import java.util.Optional;
import java.util.function.Supplier;

import javax.ws.rs.InternalServerErrorException;

import org.hibernate.cfg.NotYetImplementedException;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.stereotype.Repository;

import ca.bc.gov.open.jag.tco.oracledataapi.api.ViolationTicketApi;
import ca.bc.gov.open.jag.tco.oracledataapi.api.handler.ApiException;
import ca.bc.gov.open.jag.tco.oracledataapi.api.model.ResponseResult;
import ca.bc.gov.open.jag.tco.oracledataapi.api.model.ViolationTicket;
import ca.bc.gov.open.jag.tco.oracledataapi.mapper.DisputeMapper;
import ca.bc.gov.open.jag.tco.oracledataapi.mapper.ViolationTicketMapper;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeResult;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputeRepository;

@ConditionalOnProperty(name = "ords.enabled", havingValue = "true", matchIfMissing = false)
@Qualifier("disputeRepository")
@Repository
public class DisputeRepositoryImpl implements DisputeRepository {

	public static final String DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";

	private static Logger logger = LoggerFactory.getLogger(DisputeRepositoryImpl.class);

	// Delegate, OpenAPI generated client
	private final ViolationTicketApi violationTicketApi;

	public DisputeRepositoryImpl(ViolationTicketApi violationTicketApi) {
		this.violationTicketApi = violationTicketApi;
	}

	@Override
	public Iterable<Dispute> findByCreatedTsBefore(Date olderThan) {
		throw new NotYetImplementedException();
	}

	@Override
	public Iterable<Dispute> findByStatusNot(DisputeStatus excludeStatus) {
		throw new NotYetImplementedException();
	}

	@Override
	public Iterable<Dispute> findByStatusNotAndCreatedTsBefore(DisputeStatus excludeStatus, Date olderThan) {
		throw new NotYetImplementedException();
	}

	@Override
	@Deprecated
	public List<Dispute> findByEmailVerificationToken(String emailVerificationToken) {
		throw new NotYetImplementedException();
	}

	@Override
	public List<Dispute> findByNoticeOfDisputeId(String noticeOfDisputeId) {
		throw new NotYetImplementedException();
	}

	@Override
	public List<DisputeResult> findByTicketNumberAndTime(String ticketNumber, Date time) {
		throw new NotYetImplementedException();
	}

	@Override
	public void deleteAll() {
		// no-op. Not needed for ORDS.
	}

	@Override
	public void deleteById(Long disputeId) {
		if (disputeId == null) {
			throw new IllegalArgumentException("DisputeId is null.");
		}

		// Propagate any ApiException to caller
		ResponseResult result = violationTicketApi.v1DeleteViolationTicketDelete(disputeId);

		if (result == null) {
			throw new InternalServerErrorException("Invalid ResponseResult object");
		}
		else if (result.getException() != null) {
			// Known error if no data found
			if ("0".equals(result.getStatus()) && result.getException().startsWith("ORA-01403")) {
				throw new NoSuchElementException(result.getException());
			}
			// Unknown error
			else {
				throw new InternalServerErrorException(result.getException());
			}
		}
		else if (!"1".equals(result.getStatus())) {
			throw new InternalServerErrorException("Dispute deletion status is not 1 (success)");
		}
	}

	@Override
	public Iterable<Dispute> findAll() {
		throw new NotYetImplementedException();
	}

	@Override
	public Optional<Dispute> findById(Long id) {
		if (id == null) {
			throw new IllegalArgumentException("Dispute ID is null.");
		}
		try {
			ViolationTicket violationTicket = violationTicketApi.v1ViolationTicketGet(null, id);
			if (violationTicket == null || violationTicket.getViolationTicketId() == null) {
				return Optional.empty();
			}
			else {
				logger.debug("Successfully returned the violation ticket from ORDS with dispute id {}", id);
				Dispute dispute = DisputeMapper.INSTANCE.convertViolationTicketDtoToDispute(violationTicket);
				return Optional.ofNullable(dispute);
			}
		} catch (ApiException e) {
			logger.error("ERROR retrieving Dispute from ORDS with dispute id {}", id, e);
			throw new InternalServerErrorException(e);
		}
	}

	@Override
	public Dispute save(Dispute dispute) {
		return saveAndFlush(dispute);
	}

	@Override
	public Dispute saveAndFlush(Dispute entity) {
		if (entity == null) {
			throw new IllegalArgumentException("Dispute body is null.");
		}

		ViolationTicket violationTicket = ViolationTicketMapper.INSTANCE.convertDisputeToViolationTicketDto(entity);
		try {
			ResponseResult result = assertNoExceptions(() -> violationTicketApi.v1ProcessViolationTicketPost(violationTicket));
			if (result.getDisputeId() != null) {
				logger.debug("Successfully saved the dispute through ORDS with dispute id {}", result.getDisputeId());
				return findById(Long.valueOf(result.getDisputeId()).longValue()).orElse(null);
			}
		} catch (ApiException e) {
			logger.error("ERROR inserting Dispute to ORDS with dispute data: {}", violationTicket.toString(), e);
			throw new InternalServerErrorException(e);
		}

		return null;
	}

	@Override
	public void assignDisputeToUser(Long disputeId, String userName) {
		assertNoExceptions(() -> violationTicketApi.v1AssignViolationTicketPost(disputeId, userName));
	}

	@Override
	public void unassignDisputes(Date olderThan) {
		SimpleDateFormat simpleDateFormat = new SimpleDateFormat(DATE_TIME_FORMAT);
		String dateStr = simpleDateFormat.format(olderThan);

		assertNoExceptions(() -> violationTicketApi.v1UnassignViolationTicketPost(dateStr));
	}

	@Override
	public void setStatus(Long disputeId, DisputeStatus disputeStatus, String rejectedReason) {
		assertNoExceptions(() -> violationTicketApi.v1ViolationTicketStatusPost(disputeId, disputeStatus.toShortName(), rejectedReason));
	}

	@Override
	public void flushAndClear() {
		// no-op. Not needed for ORDS.
	}

	/**
	 * A helper method that will throw an appropriate InternalServerErrorException based on the ResponseResult. Any RuntimeExceptions throw will propagate up to caller.
	 * @return
	 */
	private ResponseResult assertNoExceptions(Supplier<ResponseResult> m) {
		ResponseResult result = m.get();

		if (result == null) {
			// Missing response object.
			throw new InternalServerErrorException("Invalid ResponseResult object");
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
