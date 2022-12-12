package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.ords;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.NoSuchElementException;
import java.util.Optional;
import java.util.TimeZone;
import java.util.function.Supplier;
import java.util.stream.Collectors;

import javax.ws.rs.InternalServerErrorException;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.stereotype.Repository;

import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.ViolationTicketApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.handler.ApiException;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.ResponseResult;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.ViolationTicket;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.ViolationTicketListResponse;
import ca.bc.gov.open.jag.tco.oracledataapi.mapper.DisputeMapper;
import ca.bc.gov.open.jag.tco.oracledataapi.mapper.ViolationTicketMapper;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeResult;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.ViolationTicketCount;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputeRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.util.DateUtil;

@ConditionalOnProperty(name = "repository.dispute", havingValue = "ords", matchIfMissing = false)
@Qualifier("disputeRepository")
@Repository
public class DisputeRepositoryImpl implements DisputeRepository {

	private static Logger logger = LoggerFactory.getLogger(DisputeRepositoryImpl.class);

	// Delegate, OpenAPI generated client
	private final ViolationTicketApi violationTicketApi;

	public DisputeRepositoryImpl(ViolationTicketApi violationTicketApi) {
		this.violationTicketApi = violationTicketApi;
	}

	@Override
	public List<Dispute> findByCreatedTsBefore(Date olderThan) {
		return findByStatusNotAndCreatedTsBeforeAndNoticeOfDisputeGuid(null, olderThan, null);
	}

	@Override
	public List<Dispute> findByStatusNot(DisputeStatus excludeStatus) {
		return findByStatusNotAndCreatedTsBeforeAndNoticeOfDisputeGuid(excludeStatus, null, null);
	}

	@Override
	public List<Dispute> findByStatusNotAndCreatedTsBeforeAndNoticeOfDisputeGuid(DisputeStatus excludeStatus, Date olderThan, String noticeOfDisputeGuid) {
		String olderThanDate = null ;
		String statusShortName = excludeStatus != null ? excludeStatus.toShortName() : null;

		if (olderThan != null) {
			SimpleDateFormat simpleDateFormat = new SimpleDateFormat(DateUtil.DATE_FORMAT);
			olderThanDate = simpleDateFormat.format(olderThan);
		}

		try {
			ViolationTicketListResponse response = violationTicketApi.v1ViolationTicketListGet(olderThanDate, statusShortName, null, noticeOfDisputeGuid, null);
			return extractDisputes(response);

		} catch (ApiException e) {
			logger.error("ERROR retrieving Disputes from ORDS");
			throw new InternalServerErrorException(e);
		}
	}

	@Override
	public List<Dispute> findByNoticeOfDisputeGuid(String noticeOfDisputeGuid) {
		return findByStatusNotAndCreatedTsBeforeAndNoticeOfDisputeGuid(null, null, noticeOfDisputeGuid);
	}

	@Override
	public List<DisputeResult> findByTicketNumberAndTime(String ticketNumber, Date issuedTime) {
		SimpleDateFormat simpleDateFormat = new SimpleDateFormat(DateUtil.TIME_FORMAT);
		String time = simpleDateFormat.format(issuedTime);
		ViolationTicketListResponse response = violationTicketApi.v1ViolationTicketListGet(null, null, ticketNumber, null, time);
		List<Dispute> extractedDisputes = extractDisputes(response);

		// Convert Disputes to DisputeResult objects
		List<DisputeResult> disputeResults = extractedDisputes.stream()
				.map(dispute -> new DisputeResult(dispute.getDisputeId(), dispute.getStatus()))
				.collect(Collectors.toList());

		return disputeResults;
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
	public List<Dispute> findAll() {
		return findByStatusNotAndCreatedTsBeforeAndNoticeOfDisputeGuid(null, null, null);
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

				// Set missing back reference
				if (dispute != null) {
					ca.bc.gov.open.jag.tco.oracledataapi.model.ViolationTicket vt = dispute.getViolationTicket();
					if (vt != null) {
						for (ViolationTicketCount violationTicketCount : vt.getViolationTicketCounts()) {
							if (violationTicketCount.getViolationTicket() == null) {
								violationTicketCount.setViolationTicket(vt);
							}
						}

					}
				}

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
		SimpleDateFormat simpleDateFormat = new SimpleDateFormat(DateUtil.DATE_TIME_FORMAT);
		simpleDateFormat.setTimeZone(TimeZone.getTimeZone("UTC")); //FIXME: UTC is a time standard, not a time zone - I think this should be GMT.
		String dateStr = simpleDateFormat.format(olderThan);

		logger.debug("Unassigning Disputes older than '{}'", dateStr);

		assertNoExceptions(() -> violationTicketApi.v1UnassignViolationTicketPost(dateStr));
	}

	@Override
	public void setStatus(Long disputeId, DisputeStatus disputeStatus, String rejectedReason) {
		assertNoExceptions(() -> violationTicketApi.v1ViolationTicketStatusPost(disputeId, disputeStatus.toShortName(), rejectedReason));
	}

	@Override
	public Dispute update(Dispute dispute) {
		if (dispute == null) {
			throw new IllegalArgumentException("Dispute body is null.");
		}

		ViolationTicket violationTicket = ViolationTicketMapper.INSTANCE.convertDisputeToViolationTicketDto(dispute);
		try {
			ResponseResult result = assertNoExceptions(() -> violationTicketApi.v1UpdateViolationTicketPut(violationTicket));
			if (result.getDisputeId() != null) {
				logger.debug("Successfully updated the dispute through ORDS with dispute id {}", result.getDisputeId());
				return findById(Long.valueOf(result.getDisputeId()).longValue()).orElse(null);
			}
		} catch (ApiException e) {
			logger.error("ERROR updating Dispute through ORDS with dispute data: {}", violationTicket.toString(), e);
			throw new InternalServerErrorException(e);
		}

		return null;
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

	/**
	 * Helper method to convert a ViolationTicketListResponse to a List of Disputes
	 */
	private List<Dispute> extractDisputes(ViolationTicketListResponse response) {
		List<Dispute> disputesToReturn = new ArrayList<Dispute>();
		if (response != null && !response.getViolationTickets().isEmpty()) {
			logger.debug("Successfully returned disputes from ORDS");

			disputesToReturn = response.getViolationTickets().stream()
					.map(violationTicket -> DisputeMapper.INSTANCE.convertViolationTicketDtoToDispute(violationTicket))
					.collect(Collectors.toList());

			// NPE fix - Some Disputes have missing counts. This should be impossible - presumably bad data.
			if (disputesToReturn != null) {
				for (Dispute dispute : disputesToReturn) {
					if (dispute.getDisputeCounts() == null) {
						logger.error("Dispute missing counts. Bad data? DisputeId: {}", dispute.getDisputeId());
						dispute.setDisputeCounts(new ArrayList<>());
					}
				}
			}
		}
		return disputesToReturn;
	}

}
