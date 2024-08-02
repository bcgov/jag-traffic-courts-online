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
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.stereotype.Repository;

import ca.bc.gov.open.jag.tco.oracledataapi.mapper.DisputeMapper;
import ca.bc.gov.open.jag.tco.oracledataapi.mapper.ViolationTicketMapper;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeListItem;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeResult;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequestStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.EmptyObject;
import ca.bc.gov.open.jag.tco.oracledataapi.model.ViolationTicketCount;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.ViolationTicketApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.handler.ApiException;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.DisputeListResponse;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.ResponseResult;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.ViolationTicket;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.ViolationTicketListResponse;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputeRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.util.DateUtil;
import net.logstash.logback.argument.StructuredArguments;
import net.logstash.logback.util.StringUtils;

@ConditionalOnProperty(name = "repository.dispute", havingValue = "ords", matchIfMissing = true)
@Qualifier("disputeRepository")
@Repository
public class DisputeRepositoryImpl implements DisputeRepository {

	private static Logger logger = LoggerFactory.getLogger(DisputeRepositoryImpl.class);

	// Delegate, OpenAPI generated client
	private final ViolationTicketApi violationTicketApi;

	private DisputeMapper disputeMapper;

	@Autowired
	public DisputeRepositoryImpl(ViolationTicketApi violationTicketApi, DisputeMapper disputeMapper) {
		this.violationTicketApi = violationTicketApi;
		this.disputeMapper = disputeMapper;
	}

	@Override
	public List<DisputeListItem> findByCreatedTsAfter(Date newerThan) {
		return findByStatusNotAndCreatedTsAfterAndNoticeOfDisputeGuid(null, newerThan, null);
	}

	@Override
	public List<DisputeListItem> findByStatusNot(DisputeStatus excludeStatus) {
		return findByStatusNotAndCreatedTsAfterAndNoticeOfDisputeGuid(excludeStatus, null, null);
	}

	@Override
	public List<DisputeListItem> findByStatusNotAndCreatedTsAfterAndNoticeOfDisputeGuid(DisputeStatus excludeStatus,
			Date newerThan, String noticeOfDisputeGuid) {
		String newerThanDate = null;
		String statusShortName = excludeStatus != null && !DisputeStatus.UNKNOWN.equals(excludeStatus) ? excludeStatus.toShortName() : null;

		if (newerThan != null) {
			SimpleDateFormat simpleDateFormat = new SimpleDateFormat(DateUtil.DATE_FORMAT);
			newerThanDate = simpleDateFormat.format(newerThan);
		}

		try {
			DisputeListResponse response = violationTicketApi.disputeListGet(newerThanDate, statusShortName, null,
					noticeOfDisputeGuid, null);
			return extractDisputeListItems(response);

		} catch (ApiException e) {
			logger.error("ERROR retrieving Disputes from ORDS");
			throw new InternalServerErrorException(e);
		}
	}

	@Override
	public List<Dispute> findByNoticeOfDisputeGuid(String noticeOfDisputeGuid) {
		return findByNoticeOfDisputeGuidImpl(null, null, noticeOfDisputeGuid);
	}

	@Override
	public List<DisputeResult> findByTicketNumberAndTime(String ticketNumber, Date issuedTime) {
		SimpleDateFormat simpleDateFormat = new SimpleDateFormat(DateUtil.TIME_FORMAT);
		String time = simpleDateFormat.format(issuedTime);
		ViolationTicketListResponse response = violationTicketApi.violationTicketListGet(null, null, ticketNumber, null,
				time);
		List<Dispute> extractedDisputes = extractDisputes(response);

		// Convert Disputes to DisputeResult objects
		List<DisputeResult> disputeResults = extractedDisputes.stream()
				.map(dispute -> new DisputeResult(
						dispute.getDisputeId(),
						dispute.getNoticeOfDisputeGuid(),
						dispute.getStatus(),
						StringUtils.isBlank(dispute.getEmailAddress()) ? null : dispute.getEmailAddressVerified(),
						dispute.getRequestCourtAppearanceYn()))
				.collect(Collectors.toList());

		return disputeResults;
	}

	@Override
	public List<DisputeResult> findByTicketNumber(String ticketNumber) {
		return findByStatusNotAndTicketNumber(null, ticketNumber);
	}
	
	@Override
	public List<DisputeResult> findByStatusNotAndTicketNumber(DisputeStatus excludeStatus, String ticketNumber) {
		String statusShortName = excludeStatus != null && !DisputeStatus.UNKNOWN.equals(excludeStatus) ? excludeStatus.toShortName() : null;
		
		ViolationTicketListResponse response = violationTicketApi.violationTicketListGet(null, statusShortName, ticketNumber, null,
				null);
		List<Dispute> extractedDisputes = extractDisputes(response);

		// Convert Disputes to DisputeResult objects
		List<DisputeResult> disputeResults = extractedDisputes.stream()
				.map(dispute -> new DisputeResult(
						dispute.getDisputeId(),
						dispute.getNoticeOfDisputeGuid(),
						dispute.getStatus(),
						StringUtils.isBlank(dispute.getEmailAddress()) ? null : dispute.getEmailAddressVerified(),
						dispute.getRequestCourtAppearanceYn()))
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
		ResponseResult result = violationTicketApi.violationTicketDelete(disputeId, EmptyObject.instance);

		if (result == null) {
			throw new InternalServerErrorException("Invalid ResponseResult object");
		} else if (result.getException() != null) {
			// Known error if no data found
			if ("0".equals(result.getStatus()) && result.getException().startsWith("ORA-01403")) {
				throw new NoSuchElementException(result.getException());
			}
			// Unknown error
			else {
				throw new InternalServerErrorException(result.getException());
			}
		} else if (!"1".equals(result.getStatus())) {
			throw new InternalServerErrorException("Dispute deletion status is not 1 (success)");
		}
	}

	@Override
	public List<DisputeListItem> getDisputeList() {
		return findByStatusNotAndCreatedTsAfterAndNoticeOfDisputeGuid(null, null, null);
	}

	@Override
	public List<Dispute> findAll() {
		return findByNoticeOfDisputeGuidImpl(null, null, null);
	}

	@Override
	public Optional<Dispute> findById(Long id) {
		if (id == null) {
			throw new IllegalArgumentException("Dispute ID is null.");
		}
		try {
			ViolationTicket violationTicket = violationTicketApi.violationTicketGet(null, id);
			if (violationTicket == null || violationTicket.getViolationTicketId() == null) {
				return Optional.empty();
			} else {
				logger.debug("Successfully returned the violation ticket from ORDS with dispute id {}",
						StructuredArguments.value("disputeId", id));
				Dispute dispute = disputeMapper.convertViolationTicketDtoToDispute(violationTicket);

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
			logger.error("ERROR retrieving Dispute from ORDS with dispute id {}",
					StructuredArguments.value("disputeId", id), e);
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
			ResponseResult result = assertNoExceptions(
					() -> violationTicketApi.processViolationTicketPost(violationTicket));
			if (result.getDisputeId() != null) {
				logger.debug("Successfully saved the dispute through ORDS with dispute id {}",
						StructuredArguments.value("disputeId", result.getDisputeId()));
				return findById(Long.valueOf(result.getDisputeId()).longValue()).orElse(null);
			}
		} catch (ApiException e) {
			logger.error("ERROR inserting Dispute to ORDS with dispute data: {}",
					StructuredArguments.fields(violationTicket), e);
			throw new InternalServerErrorException(e);
		}

		return null;
	}

	@Override
	public void assignDisputeToUser(Long disputeId, String userName) {
		assertNoExceptions(
				() -> violationTicketApi.assignViolationTicketPost(disputeId, userName, EmptyObject.instance));
	}

	@Override
	public void unassignDisputes(Date olderThan) {
		SimpleDateFormat simpleDateFormat = new SimpleDateFormat(DateUtil.DATE_TIME_FORMAT);
		simpleDateFormat.setTimeZone(TimeZone.getTimeZone("UTC")); // FIXME: UTC is a time standard, not a time zone - I
																	// think this should be GMT.
		String dateStr = simpleDateFormat.format(olderThan);

		logger.debug("Unassigning Disputes older than {}", StructuredArguments.value("date", dateStr));

		assertNoExceptions(() -> violationTicketApi.unassignViolationTicketPost(EmptyObject.instance, dateStr));
	}

	@Override
	public void setStatus(Long disputeId, DisputeStatus disputeStatus, String rejectedReason) {
		assertNoExceptions(() -> violationTicketApi.violationTicketStatusPost(EmptyObject.instance, disputeId,
				disputeStatus.toShortName(), rejectedReason));
	}

	@Override
	public Dispute update(Dispute dispute) {
		if (dispute == null) {
			throw new IllegalArgumentException("Dispute body is null.");
		}

		ViolationTicket violationTicket = ViolationTicketMapper.INSTANCE.convertDisputeToViolationTicketDto(dispute);
		try {
			ResponseResult result = assertNoExceptions(
					() -> violationTicketApi.v1UpdateViolationTicketPut(violationTicket));
			if (result.getDisputeId() != null) {
				logger.debug("Successfully updated the dispute through ORDS with dispute id {}",
						StructuredArguments.value("disputeId", result.getDisputeId()));
				return findById(Long.valueOf(result.getDisputeId()).longValue()).orElse(null);
			}
		} catch (ApiException e) {
			logger.error("ERROR updating Dispute through ORDS with dispute data: {}",
					StructuredArguments.fields(violationTicket), e);
			throw new InternalServerErrorException(e);
		}

		return null;
	}
	
	@Override
	public void deleteViolationTicketCountById(Long violationTicketCountId) {
		if (violationTicketCountId == null) {
			throw new IllegalArgumentException("violationTicketCountId is null.");
		}

		assertNoExceptions(() -> violationTicketApi.violationTicketCountDelete(violationTicketCountId, EmptyObject.instance));
	}

	@Override
	public void flushAndClear() {
		// no-op. Not needed for ORDS.
	}

	private List<Dispute> findByNoticeOfDisputeGuidImpl(DisputeStatus excludeStatus, Date newerThan,
			String noticeOfDisputeGuid) {
		String newerThanDate = null;
		String statusShortName = excludeStatus != null && !DisputeStatus.UNKNOWN.equals(excludeStatus) ? excludeStatus.toShortName() : null;

		if (newerThan != null) {
			SimpleDateFormat simpleDateFormat = new SimpleDateFormat(DateUtil.DATE_FORMAT);
			newerThanDate = simpleDateFormat.format(newerThan);
		}

		try {
			ViolationTicketListResponse response = violationTicketApi.violationTicketListGet(newerThanDate,
					statusShortName, null, noticeOfDisputeGuid, null);
			return extractDisputes(response);

		} catch (ApiException e) {
			logger.error("ERROR retrieving Disputes from ORDS");
			throw new InternalServerErrorException(e);
		}
	}

	/**
	 * A helper method that will throw an appropriate InternalServerErrorException
	 * based on the ResponseResult. Any RuntimeExceptions throw will propagate up to
	 * caller.
	 *
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
					.map(violationTicket -> disputeMapper.convertViolationTicketDtoToDispute(violationTicket))
					.collect(Collectors.toList());

			// NPE fix - Some Disputes have missing counts. This should be impossible -
			// presumably bad data.
			if (disputesToReturn != null) {
				for (Dispute dispute : disputesToReturn) {
					if (dispute.getDisputeCounts() == null) {
						logger.error("Dispute missing counts. Bad data? DisputeId: {}",
								StructuredArguments.value("disputeId", dispute.getDisputeId()));
						dispute.setDisputeCounts(new ArrayList<>());
					}
				}
			}
		}
		return disputesToReturn;
	}

	/**
	 * Helper method to convert a DisputeListResponse to a list of
	 * {@link DisputeListItem}
	 *
	 * @param response
	 * @return a list of {@link DisputeListItem}
	 */
	private List<DisputeListItem> extractDisputeListItems(DisputeListResponse response) {
		List<DisputeListItem> disputeListToReturn = new ArrayList<DisputeListItem>();
		if (response != null && !response.getDisputeListItems().isEmpty()) {
			logger.debug("Successfully returned dispute list items from ORDS");

			disputeListToReturn = response.getDisputeListItems().stream().map(
					diputeListItem -> disputeMapper.convertDisputeListItemDtoToDisputeListItem(diputeListItem))
					.collect(Collectors.toList());
		}
		return disputeListToReturn;
	}

}
