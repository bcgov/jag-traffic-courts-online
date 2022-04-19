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

class DisputeServiceTest extends BaseTestSuite {

	@Autowired
	private DisputeService disputeService;

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "NEW", "PROCESSING" })
	void testSetStatusToPROCESSING_200(DisputeStatus disputeStatus) {
		Integer id = saveDispute(disputeStatus);
		disputeService.setStatus(id, DisputeStatus.PROCESSING);
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "REJECTED", "CANCELLED" })
	void testSetStatusToPROCESSING_405(DisputeStatus disputeStatus) {
		Integer id = saveDispute(disputeStatus);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, DisputeStatus.PROCESSING);
		});
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "NEW", "REJECTED", "CANCELLED" })
	void testSetStatusToREJECTED_200(DisputeStatus disputeStatus) {
		Integer id = saveDispute(disputeStatus);
		disputeService.setStatus(id, DisputeStatus.REJECTED);
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "PROCESSING" })
	void testSetStatusToREJECTED_405(DisputeStatus disputeStatus) {
		Integer id = saveDispute(disputeStatus);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, DisputeStatus.REJECTED);
		});
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "REJECTED", "PROCESSING" })
	void testSetStatusToCANCELLED_200(DisputeStatus disputeStatus) {
		Integer id = saveDispute(disputeStatus);
		disputeService.setStatus(id, DisputeStatus.CANCELLED);
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "NEW", "CANCELLED" })
	void testSetStatusToCANCELLED_405(DisputeStatus disputeStatus) {
		Integer id = saveDispute(disputeStatus);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, DisputeStatus.CANCELLED);
		});
	}

	@Test
	void testSetStatusToNEW_405() {
		Integer id = saveDispute(DisputeStatus.NEW);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, DisputeStatus.NEW);
		});
	}

	@Test
	void testSetStatusToNULL_405() {
		Integer id = saveDispute(null);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, null);
		});
	}

	private Integer saveDispute(DisputeStatus disputeStatus) {
		Dispute dispute = new Dispute();
		dispute.setStatus(disputeStatus);

		return disputeRepository.save(dispute).getId();
	}

}
