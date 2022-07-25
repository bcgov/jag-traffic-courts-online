package ca.bc.gov.open.jag.tco.oracledataapi.service;

import static org.junit.jupiter.api.Assertions.assertThrows;

import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.EnumSource;
import org.springframework.beans.factory.annotation.Autowired;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.error.NotAllowedException;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;

class JJDisputeServiceTest extends BaseTestSuite {

	@Autowired
	private JJDisputeService jjDisputeService;

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "NEW", "REVIEW", "IN_PROGRESS" })
	void testSetStatusToIN_PROGRESS_200(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.IN_PROGRESS);
		jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus);
	}

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "COMPLETED" })
	void testSetStatusToIN_PROGRESS_405(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.IN_PROGRESS);
		assertThrows(NotAllowedException.class, () -> {
			jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus);
		});
	}
	
	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "NEW", "REVIEW", "IN_PROGRESS" })
	void testSetStatusToREVIEW_200(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.REVIEW);
		jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus);
	}

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "COMPLETED" })
	void testSetStatusToREVIEW_405(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.REVIEW);
		assertThrows(NotAllowedException.class, () -> {
			jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus);
		});
	}
	
	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "REVIEW" })
	void testSetStatusToCOMPLETED_200(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.COMPLETED);
		jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus);
	}

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "NEW", "COMPLETED", "IN_PROGRESS" })
	void testSetStatusToCOMPLETED_405(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.COMPLETED);
		assertThrows(NotAllowedException.class, () -> {
			jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus);
		});
	}

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "NEW" })
	void testSetStatusToNEW_200(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.NEW);
		jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus);
	}

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "REVIEW", "COMPLETED", "IN_PROGRESS" })
	void testSetStatusToNEW_405(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.NEW);
		assertThrows(NotAllowedException.class, () -> {
			jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus);
		});
	}

	private JJDispute saveDispute(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDispute = RandomUtil.createJJDispute();
		jjDispute.setStatus(jjDisputeStatus);

		return jjDisputeRepository.save(jjDispute);
	}

}
