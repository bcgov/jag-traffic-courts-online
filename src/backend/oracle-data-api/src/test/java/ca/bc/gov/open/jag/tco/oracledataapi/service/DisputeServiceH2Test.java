package ca.bc.gov.open.jag.tco.oracledataapi.service;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertThrows;

import java.util.List;
import java.util.NoSuchElementException;
import java.util.UUID;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.EnumSource;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.error.NotAllowedException;
import ca.bc.gov.open.jag.tco.oracledataapi.model.ContactType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequest;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequestStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequestType;

@ConditionalOnProperty(name = "repository.dispute", havingValue = "h2", matchIfMissing = true)
class DisputeServiceH2Test extends BaseTestSuite {

	@Autowired
	private DisputeService disputeService;

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "NEW", "REJECTED", "VALIDATED" })
	public void testSetStatusToPROCESSING_200(DisputeStatus disputeStatus) {
		Long id = saveDispute(disputeStatus);
		disputeService.setStatus(id, DisputeStatus.PROCESSING);
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "CANCELLED", "PROCESSING" })
	public void testSetStatusToPROCESSING_405(DisputeStatus disputeStatus) {
		Long id = saveDispute(disputeStatus);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, DisputeStatus.PROCESSING);
		});
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "NEW" })
	public void testSetStatusToVALIDATED_200(DisputeStatus disputeStatus) {
		Long id = saveDispute(disputeStatus);
		disputeService.setStatus(id, DisputeStatus.VALIDATED);
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "CANCELLED", "PROCESSING", "VALIDATED" })
	public void testSetStatusToVALIDATED_405(DisputeStatus disputeStatus) {
		Long id = saveDispute(disputeStatus);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, DisputeStatus.VALIDATED);
		});
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "NEW", "VALIDATED" })
	public void testSetStatusToREJECTED_200(DisputeStatus disputeStatus) {
		Long id = saveDispute(disputeStatus);
		disputeService.setStatus(id, DisputeStatus.REJECTED);
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "CANCELLED", "PROCESSING", "REJECTED" })
	public void testSetStatusToREJECTED_405(DisputeStatus disputeStatus) {
		Long id = saveDispute(disputeStatus);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, DisputeStatus.REJECTED);
		});
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "NEW", "PROCESSING", "REJECTED", "VALIDATED" })
	public void testSetStatusToCANCELLED_200(DisputeStatus disputeStatus) {
		Long id = saveDispute(disputeStatus);
		disputeService.setStatus(id, DisputeStatus.CANCELLED);
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "CANCELLED" })
	public void testSetStatusToCANCELLED_405(DisputeStatus disputeStatus) {
		Long id = saveDispute(disputeStatus);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, DisputeStatus.CANCELLED);
		});
	}

	@Test
	public void testSetStatusToNEW_405() {
		Long id = saveDispute(DisputeStatus.NEW);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, DisputeStatus.NEW);
		});
	}

	@Test
	public void testSetStatusToNULL_405() {
		Long id = saveDispute(null);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, null);
		});
	}

	@Test
	public void testDisputeUpdateRequest() throws Exception {
		Dispute dispute = createAndSaveDispute();
		String noticeOfDisputeGuid = dispute.getNoticeOfDisputeGuid();

		String json = "{ \"address_line1\": \"123 Main Street\", \"address_line2\": \"\", \"address_line3\": \"\" }";
		DisputeUpdateRequest updateRequest = new DisputeUpdateRequest();
		updateRequest.setDisputeId(Long.valueOf(1L));
		updateRequest.setStatus(DisputeUpdateRequestStatus.PENDING);
		updateRequest.setUpdateType(DisputeUpdateRequestType.DISPUTANT_ADDRESS);
		updateRequest.setUpdateJson(json);
		DisputeUpdateRequest savedUpdateReq = disputeService.saveDisputeUpdateRequest(noticeOfDisputeGuid, updateRequest);
		Long disputeUpdateRequestId = savedUpdateReq.getDisputeUpdateRequestId();

		assertNotNull(disputeUpdateRequestId);

		List<DisputeUpdateRequest> updateRequests = disputeService.findDisputeUpdateRequestByDisputeIdAndStatus(dispute.getDisputeId(), null);

		assertEquals(1, updateRequests.size());
		savedUpdateReq = updateRequests.get(0);

		assertEquals(dispute.getDisputeId(), savedUpdateReq.getDisputeId().longValue());
		assertEquals(DisputeUpdateRequestStatus.PENDING, savedUpdateReq.getStatus());
		assertEquals(DisputeUpdateRequestType.DISPUTANT_ADDRESS, savedUpdateReq.getUpdateType());
		assertEquals(json, savedUpdateReq.getUpdateJson());

		savedUpdateReq = disputeService.updateDisputeUpdateRequest(disputeUpdateRequestId, DisputeUpdateRequestStatus.ACCEPTED);
		assertEquals(DisputeUpdateRequestStatus.ACCEPTED, savedUpdateReq.getStatus());
	}

	@Test
	public void testDisputeUpdateRequest_404() throws Exception {
		assertThrows(NoSuchElementException.class, () -> {
			disputeService.saveDisputeUpdateRequest("some-guid", new DisputeUpdateRequest());
		});
	}

	private Long saveDispute(DisputeStatus disputeStatus) {
		Dispute dispute = new Dispute();
		dispute.setStatus(disputeStatus);
		dispute.setContactTypeCd(ContactType.INDIVIDUAL);

		return disputeRepository.save(dispute).getDisputeId();
	}

	private Dispute createAndSaveDispute() {
		String noticeOfDisputeGuid = UUID.randomUUID().toString();

		Dispute dispute = new Dispute();
		dispute.setStatus(DisputeStatus.NEW);
		dispute.setContactTypeCd(ContactType.INDIVIDUAL);
		dispute.setNoticeOfDisputeGuid(noticeOfDisputeGuid);
		return disputeRepository.save(dispute);
	}

}
