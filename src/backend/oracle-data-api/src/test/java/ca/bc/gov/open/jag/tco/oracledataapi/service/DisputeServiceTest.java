package ca.bc.gov.open.jag.tco.oracledataapi.service;

import static org.junit.jupiter.api.Assertions.assertThrows;

import java.util.UUID;

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
	@EnumSource(value = DisputeStatus.class, names = { "NEW", "REJECTED" })
	void testSetStatusToPROCESSING_200(DisputeStatus disputeStatus) {
		UUID id = saveDispute(disputeStatus);
		disputeService.setStatus(id, DisputeStatus.PROCESSING);
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "CANCELLED", "PROCESSING" })
	void testSetStatusToPROCESSING_405(DisputeStatus disputeStatus) {
		UUID id = saveDispute(disputeStatus);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, DisputeStatus.PROCESSING);
		});
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "NEW" })
	void testSetStatusToREJECTED_200(DisputeStatus disputeStatus) {
		UUID id = saveDispute(disputeStatus);
		disputeService.setStatus(id, DisputeStatus.REJECTED);
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "CANCELLED", "PROCESSING", "REJECTED" })
	void testSetStatusToREJECTED_405(DisputeStatus disputeStatus) {
		UUID id = saveDispute(disputeStatus);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, DisputeStatus.REJECTED);
		});
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "NEW", "PROCESSING", "REJECTED" })
	void testSetStatusToCANCELLED_200(DisputeStatus disputeStatus) {
		UUID id = saveDispute(disputeStatus);
		disputeService.setStatus(id, DisputeStatus.CANCELLED);
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "CANCELLED" })
	void testSetStatusToCANCELLED_405(DisputeStatus disputeStatus) {
		UUID id = saveDispute(disputeStatus);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, DisputeStatus.CANCELLED);
		});
	}

	@Test
	void testSetStatusToNEW_405() {
		UUID id = saveDispute(DisputeStatus.NEW);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, DisputeStatus.NEW);
		});
	}

	@Test
	void testSetStatusToNULL_405() {
		UUID id = saveDispute(null);
		assertThrows(NotAllowedException.class, () -> {
			disputeService.setStatus(id, null);
		});
	}

	private UUID saveDispute(DisputeStatus disputeStatus) {
		Dispute dispute = new Dispute();
		dispute.setStatus(disputeStatus);

		return disputeRepository.save(dispute).getId();
	}

}
