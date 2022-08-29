package ca.bc.gov.open.jag.tco.oracledataapi.service;

import static org.junit.jupiter.api.Assertions.assertThrows;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.EnumSource;
import org.springframework.beans.factory.annotation.Autowired;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.error.NotAllowedException;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatusType;

class DisputeServiceTest extends BaseTestSuite {

	@Autowired
	private DisputeService disputeService;

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "NEW", "REJECTED", "VALIDATED" })
	void testSetStatusToPROCESSING_200(DisputeStatus disputeStatus) {
		Long id = saveDispute(disputeStatus);
		disputeService.setStatus(id, DisputeStatus.PROCESSING);
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "CANCELLED", "PROCESSING" })
	void testSetStatusToPROCESSING_405(DisputeStatus disputeStatus) {
		Long id = saveDispute(disputeStatus);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, DisputeStatus.PROCESSING);
		});
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "NEW" })
	void testSetStatusToVALIDATED_200(DisputeStatus disputeStatus) {
		Long id = saveDispute(disputeStatus);
		disputeService.setStatus(id, DisputeStatus.VALIDATED);
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "CANCELLED", "PROCESSING", "VALIDATED" })
	void testSetStatusToVALIDATED_405(DisputeStatus disputeStatus) {
		Long id = saveDispute(disputeStatus);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, DisputeStatus.VALIDATED);
		});
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "NEW", "VALIDATED" })
	void testSetStatusToREJECTED_200(DisputeStatus disputeStatus) {
		Long id = saveDispute(disputeStatus);
		disputeService.setStatus(id, DisputeStatus.REJECTED);
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "CANCELLED", "PROCESSING", "REJECTED" })
	void testSetStatusToREJECTED_405(DisputeStatus disputeStatus) {
		Long id = saveDispute(disputeStatus);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, DisputeStatus.REJECTED);
		});
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "NEW", "PROCESSING", "REJECTED", "VALIDATED" })
	void testSetStatusToCANCELLED_200(DisputeStatus disputeStatus) {
		Long id = saveDispute(disputeStatus);
		disputeService.setStatus(id, DisputeStatus.CANCELLED);
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "CANCELLED" })
	void testSetStatusToCANCELLED_405(DisputeStatus disputeStatus) {
		Long id = saveDispute(disputeStatus);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, DisputeStatus.CANCELLED);
		});
	}

	@Test
	void testSetStatusToNEW_405() {
		Long id = saveDispute(DisputeStatus.NEW);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, DisputeStatus.NEW);
		});
	}

	@Test
	void testSetStatusToNULL_405() {
		Long id = saveDispute(null);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, null);
		});
	}

	private Long saveDispute(DisputeStatus disputeStatus) {
		Dispute dispute = new Dispute();
		dispute.setStatus(disputeStatus);

		return disputeRepository.save(dispute).getDisputeId();
	}

}
