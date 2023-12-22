package ca.bc.gov.open.jag.tco.oracledataapi.service;

import static org.junit.jupiter.api.Assertions.*;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;

import org.apache.commons.lang3.time.DateUtils;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.EnumSource;
import org.springframework.beans.factory.annotation.Autowired;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.error.NotAllowedException;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeCourtAppearanceRoP;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputedCount;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Plea;
import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;

class JJDisputeServiceTest extends BaseTestSuite {

	@Autowired
	private JJDisputeService jjDisputeService;

	@PersistenceContext
	private EntityManager entityManager;

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
	@EnumSource(value = JJDisputeStatus.class, names = { "ACCEPTED", "CONCLUDED", "DATA_UPDATE", "IN_PROGRESS", "NEW", "REQUIRE_COURT_HEARING", "REQUIRE_MORE_INFO" })
	void testSetStatusToREVIEW_405(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.REVIEW);
		assertThrows(NotAllowedException.class, () -> {
			jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus, this.getPrincipal());
		});
	}
	
	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "ACCEPTED", "CONFIRMED", "CONCLUDED" })
	void testSetStatusToREVIEW_200_validAppearanceDate(JJDisputeStatus jjDisputeStatus) {
		JJDisputeCourtAppearanceRoP courtAppearance =  new JJDisputeCourtAppearanceRoP();
		Date appearanceDate = new Date();
		courtAppearance.setAppearanceTs(appearanceDate);
		JJDispute jjDisputeToUpdate = saveDisputeWithCourtAppearance(jjDisputeStatus, courtAppearance);
		JJDispute disputeInReview = jjDisputeService.setStatus(jjDisputeToUpdate.getTicketNumber(), JJDisputeStatus.REVIEW, this.getPrincipal(), null, null, true);
		assertEquals(JJDisputeStatus.REVIEW, disputeInReview.getStatus());
		assertTrue(disputeInReview.getRecalled());
	}
	
	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "ACCEPTED", "CONFIRMED", "CONCLUDED" })
	void testSetStatusToREVIEW_405_invalidAppearanceDate(JJDisputeStatus jjDisputeStatus) {
		JJDisputeCourtAppearanceRoP courtAppearance =  new JJDisputeCourtAppearanceRoP();
		Date appearanceDate = DateUtils.addDays(new Date(), -1);
		courtAppearance.setAppearanceTs(appearanceDate);
		JJDispute jjDisputeToUpdate = saveDisputeWithCourtAppearance(jjDisputeStatus, courtAppearance);
		assertThrows(NotAllowedException.class, () -> {
			jjDisputeService.setStatus(jjDisputeToUpdate.getTicketNumber(), JJDisputeStatus.REVIEW, this.getPrincipal(), null, null, true);
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

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class,names = { "DATA_UPDATE", "NEW", "REQUIRE_COURT_HEARING", "IN_PROGRESS", "REQUIRE_MORE_INFO", "CONFIRMED", "ACCEPTED",  "REVIEW", "CONCLUDED", "CANCELLED" })
	void testSetStatusToCONCLUDED_200(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.CONCLUDED);
		jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus, this.getPrincipal());
	}

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class,names = { "DATA_UPDATE", "NEW", "REQUIRE_COURT_HEARING", "IN_PROGRESS", "REQUIRE_MORE_INFO", "CONFIRMED", "ACCEPTED",  "REVIEW", "CONCLUDED", "CANCELLED" })
	void testSetStatusToCANCELLED_200(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDisputeToUpdate = saveDispute(jjDisputeStatus);
		JJDispute jjDisputeWithUpdatedStatus = saveDispute(JJDisputeStatus.CANCELLED);
		jjDisputeService.updateJJDispute(jjDisputeToUpdate.getTicketNumber(), jjDisputeWithUpdatedStatus, this.getPrincipal());
	}

	@ParameterizedTest
	@EnumSource(value = JJDisputeStatus.class, names = { "ACCEPTED" })
	void testSetStatusToACCEPTEDwithNoStaffPartId_405(JJDisputeStatus jjDisputeStatus) {
		JJDisputeCourtAppearanceRoP courtAppearance =  new JJDisputeCourtAppearanceRoP();
		JJDispute jjDisputeToUpdate = saveDisputeWithCourtAppearance(jjDisputeStatus, courtAppearance);
		// Do not provide a staff part ID for the update
		this.setPrincipal("System", false);
		assertThrows(NotAllowedException.class, () -> {
			jjDisputeService.setStatus(jjDisputeToUpdate.getTicketNumber(), jjDisputeStatus, this.getPrincipal(), null, "170225.0877", false);
		});
	}
	
	@Test
	void testUpdateJJDispute() throws Exception {
		// TCVP-1981 confirm LatestPlea and LatestPleaUpdateTs on JJDisputedCount is updated
		// Initially, the LatestPlea and LatestPleaUpdateTs are null, confirm persistence doesn't change that
		JJDispute jjDispute = saveDispute(JJDisputeStatus.NEW);		
		JJDisputedCount jjDisputedCount = RandomUtil.createJJDisputedCount(1);
		jjDisputedCount.setLatestPlea(null);
		jjDisputedCount.setLatestPleaUpdateTs(null);
		jjDisputedCount.setJjDispute(jjDispute);
		jjDispute.getJjDisputedCounts().add(jjDisputedCount);
		jjDisputeRepository.saveAndFlush(jjDispute);
		
		entityManager.detach(jjDispute);
		jjDispute = jjDisputeService.updateJJDispute(jjDispute.getTicketNumber(), jjDispute, this.getPrincipal());

		assertNull(jjDispute.getJjDisputedCounts().get(0).getLatestPlea());
		assertNull(jjDispute.getJjDisputedCounts().get(0).getLatestPleaUpdateTs());
		
		// Update the LatestPlea and confirm service layer automatically set the LatestPleaUpdateTs
		jjDispute.getJjDisputedCounts().get(0).setLatestPlea(Plea.G);
		
		jjDispute.getRemarks().size(); // lazy load remarks
		entityManager.detach(jjDispute);
		jjDispute = jjDisputeService.updateJJDispute(jjDispute.getTicketNumber(), jjDispute, this.getPrincipal());
		
		assertNotNull(jjDispute.getJjDisputedCounts().get(0).getLatestPlea());
		assertNotNull(jjDispute.getJjDisputedCounts().get(0).getLatestPleaUpdateTs());

		// Update the LatestPlea again, this time setting it to null - confirm service layer automatically set the LatestPleaUpdateTs
		jjDispute.getJjDisputedCounts().get(0).setLatestPlea(null);
		
		jjDispute.getRemarks().size(); // lazy load remarks
		entityManager.detach(jjDispute);
		jjDispute = jjDisputeService.updateJJDispute(jjDispute.getTicketNumber(), jjDispute, this.getPrincipal());

		assertNull(jjDispute.getJjDisputedCounts().get(0).getLatestPlea());
		assertNotNull(jjDispute.getJjDisputedCounts().get(0).getLatestPleaUpdateTs());
	}

	private JJDispute saveDispute(JJDisputeStatus jjDisputeStatus) {
		JJDispute jjDispute = RandomUtil.createJJDispute();
		jjDispute.setStatus(jjDisputeStatus);

		return jjDisputeRepository.saveAndFlush(jjDispute);
	}

	private JJDispute saveDisputeWithCourtAppearance(JJDisputeStatus jjDisputeStatus, JJDisputeCourtAppearanceRoP courtAppearance) {
		JJDispute jjDispute = RandomUtil.createJJDispute();
		jjDispute.setStatus(jjDisputeStatus);

		List<JJDisputeCourtAppearanceRoP> courtAppearanceList = new ArrayList<JJDisputeCourtAppearanceRoP>();
		courtAppearance.setJustinAppearanceId("59295.0734");
		courtAppearance.setId(5L);
		courtAppearance.setJjDispute(jjDispute);
		courtAppearanceList.add(courtAppearance);
		jjDispute.setJjDisputeCourtAppearanceRoPs(courtAppearanceList);

		return jjDisputeRepository.saveAndFlush(jjDispute);
	}

}
