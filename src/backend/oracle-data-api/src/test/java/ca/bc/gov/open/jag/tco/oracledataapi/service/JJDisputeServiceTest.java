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
	@EnumSource(value = JJDisputeStatus.class, names = { "NEW", "IN_PROGRESS" })
	void testSetStatusToIN_PROGRESS_200(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.IN_PROGRESS);
		jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus, this.getPrincipal());
	}

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "ACCEPTED", "CONFIRMED", "DATA_UPDATE", "REQUIRE_COURT_HEARING", "REQUIRE_MORE_INFO", "REVIEW" })
	void testSetStatusToIN_PROGRESS_405(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.IN_PROGRESS);
		assertThrows(NotAllowedException.class, () -> {
			jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus, this.getPrincipal());
		});
	}

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "CONFIRMED", "REVIEW" })
	void testSetStatusToREVIEW_200(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.REVIEW);
		jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus, this.getPrincipal());
	}

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "ACCEPTED", "DATA_UPDATE", "IN_PROGRESS", "NEW", "REQUIRE_COURT_HEARING", "REQUIRE_MORE_INFO" })
	void testSetStatusToREVIEW_405(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.REVIEW);
		assertThrows(NotAllowedException.class, () -> {
			jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus, this.getPrincipal());
		});
	}

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "REVIEW", "NEW", "IN_PROGRESS", "CONFIRMED" })
	void testSetStatusToCONFIRMED_200(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.CONFIRMED);
		jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus, this.getPrincipal());
	}

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "ACCEPTED", "DATA_UPDATE", "REQUIRE_COURT_HEARING", "REQUIRE_MORE_INFO" })
	void testSetStatusToCONFIRMED_405(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.CONFIRMED);
		assertThrows(NotAllowedException.class, () -> {
			jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus, this.getPrincipal());
		});
	}

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "NEW" })
	void testSetStatusToNEW_200(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.NEW);
		jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus, this.getPrincipal());
	}

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "ACCEPTED", "CONFIRMED", "DATA_UPDATE", "REVIEW", "REQUIRE_COURT_HEARING", "IN_PROGRESS", "REQUIRE_MORE_INFO" })
	void testSetStatusToNEW_405(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.NEW);
		assertThrows(NotAllowedException.class, () -> {
			jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus, this.getPrincipal());
		});
	}

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "NEW", "IN_PROGRESS", "REVIEW", "DATA_UPDATE" })
	void testSetStatusToDATA_UPDATE_200(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.DATA_UPDATE);
		jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus, this.getPrincipal());
	}

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "ACCEPTED", "CONFIRMED", "REQUIRE_COURT_HEARING", "REQUIRE_MORE_INFO" })
	void testSetStatusToDATA_UPDATE_405(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.DATA_UPDATE);
		assertThrows(NotAllowedException.class, () -> {
			jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus, this.getPrincipal());
		});
	}
	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "NEW", "IN_PROGRESS", "REVIEW", "HEARING_SCHEDULED", "CONFIRMED", "REQUIRE_COURT_HEARING" })
	void testSetStatusToREQUIRE_COURT_HEARING_200(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.REQUIRE_COURT_HEARING);
		jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus, this.getPrincipal());
	}

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "ACCEPTED", "DATA_UPDATE", "REQUIRE_MORE_INFO" })
	void testSetStatusToREQUIRE_COURT_HEARING_405(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.REQUIRE_COURT_HEARING);
		assertThrows(NotAllowedException.class, () -> {
			jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus, this.getPrincipal());
		});
	}
	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "NEW", "IN_PROGRESS", "REVIEW", "REQUIRE_MORE_INFO" })
	void testSetStatusToREQUIRE_MORE_INFO_200(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.REQUIRE_MORE_INFO);
		jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus, this.getPrincipal());
	}

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "ACCEPTED", "CONFIRMED", "DATA_UPDATE", "REQUIRE_COURT_HEARING" })
	void testSetStatusToREQUIRE_MORE_INFO_405(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.REQUIRE_MORE_INFO);
		assertThrows(NotAllowedException.class, () -> {
			jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus, this.getPrincipal());
		});
	}

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "CONFIRMED", "ACCEPTED" })
	void testSetStatusToACCEPTED_200(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.ACCEPTED);
		jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus, this.getPrincipal());
	}

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "DATA_UPDATE", "NEW", "REVIEW", "REQUIRE_COURT_HEARING", "IN_PROGRESS", "REQUIRE_MORE_INFO" })
	void testSetStatusToACCEPTED_405(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.ACCEPTED);
		assertThrows(NotAllowedException.class, () -> {
			jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus, this.getPrincipal());
		});
	}

	private JJDispute saveDispute(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDispute = RandomUtil.createJJDispute();
		jjDispute.setStatus(jjDisputeStatus);

		return jjDisputeRepository.saveAndFlush(jjDispute);
	}

}
