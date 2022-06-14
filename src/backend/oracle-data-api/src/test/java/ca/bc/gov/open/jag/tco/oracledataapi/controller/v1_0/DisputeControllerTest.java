package ca.bc.gov.open.jag.tco.oracledataapi.controller.v1_0;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.junit.jupiter.api.Assertions.assertThrows;

import java.security.Principal;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.List;
import javax.transaction.Transactional;
import javax.validation.ConstraintViolationException;

import org.apache.commons.collections4.IterableUtils;
import org.apache.commons.lang3.RandomStringUtils;
import org.apache.commons.lang3.time.DateUtils;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Qualifier;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;

class DisputeControllerTest extends BaseTestSuite {

	@Autowired
	@Qualifier("DisputeControllerV1_0")
	private DisputeController disputeController;

	@Test
	public void testSaveDispute() {
		// Assert db is empty and clean
		List<Dispute> allDisputes = IterableUtils.toList(disputeController.getAllDisputes(null, null));
		assertEquals(0, allDisputes.size());

		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		Long disputeId = disputeController.saveDispute(dispute);

		// Assert db contains the single created record
		allDisputes = IterableUtils.toList(disputeController.getAllDisputes(null, null));
		assertEquals(1, allDisputes.size());
		assertEquals(disputeId, allDisputes.get(0).getId());
		assertEquals(dispute.getSurname(), allDisputes.get(0).getSurname());

		// Delete record
		disputeController.deleteDispute(disputeId);

		// Assert db contains is empty again
		allDisputes = IterableUtils.toList(disputeController.getAllDisputes(null, null));
		assertEquals(0, allDisputes.size());
	}

	@Test
	public void testRejectDisputeSuccess() {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		Long disputeId = disputeController.saveDispute(dispute);
		Principal principal = getPrincipal("testUser");

		// Retrieve it from the controller's endpoint
		dispute = disputeController.getDispute(disputeId, principal).getBody();
		assertEquals(disputeId, dispute.getId());
		assertEquals(DisputeStatus.NEW, dispute.getStatus());

		// Set the status to REJECTED
		disputeController.rejectDispute(disputeId, "Just because", principal);

		// Assert status and reason are set.
		dispute = disputeController.getDispute(disputeId, principal).getBody();
		assertEquals(DisputeStatus.REJECTED, dispute.getStatus());
		assertEquals("Just because", dispute.getRejectedReason());
	}

	@Test
	public void testRejectDisputeFail() {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		Long disputeId = disputeController.saveDispute(dispute);
		Principal principal = getPrincipal("testUser");

		// Retrieve it from the controller's endpoint
		dispute = disputeController.getDispute(disputeId, principal).getBody();
		assertEquals(disputeId, dispute.getId());
		assertEquals(DisputeStatus.NEW, dispute.getStatus());

		// try using an empty reason (should fail with 405 error)
		assertThrows(ConstraintViolationException.class, () -> {
			disputeController.rejectDispute(disputeId, "", principal);
		});

		// try using an long reason, > 256 (should fail with 405 error)
		assertThrows(ConstraintViolationException.class, () -> {
			disputeController.rejectDispute(disputeId, RandomStringUtils.random(257), principal);
		});

		String longString = RandomStringUtils.random(256);

		// Set the status to REJECTED
		disputeController.rejectDispute(disputeId, longString, principal);

		// Assert status and reason are set.
		dispute = disputeController.getDispute(disputeId, principal).getBody();
		assertEquals(DisputeStatus.REJECTED, dispute.getStatus());
		assertEquals(longString, dispute.getRejectedReason());
	}

	@Test
	public void testSubmitDispute() {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		Long disputeId = disputeController.saveDispute(dispute);
		Principal principal = getPrincipal("testUser");

		// Retrieve it from the controller's endpoint
		dispute = disputeController.getDispute(disputeId, principal).getBody();
		assertEquals(disputeId, dispute.getId());
		assertEquals(DisputeStatus.NEW, dispute.getStatus());

		// Set the status to PROCESSING
		disputeController.submitDispute(disputeId, principal);

		// Assert status is set, rejected reason is NOT set.
		dispute = disputeController.getDispute(disputeId, principal).getBody();;
		assertEquals(DisputeStatus.PROCESSING, dispute.getStatus());
		assertNull(dispute.getRejectedReason());
	}

	@Test
	public void testCancelDispute() {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		Long disputeId = disputeController.saveDispute(dispute);
		Principal principal = getPrincipal("testUser");

		// Retrieve it from the controller's endpoint
		dispute = disputeController.getDispute(disputeId, principal).getBody();
		assertEquals(disputeId, dispute.getId());
		assertEquals(DisputeStatus.NEW, dispute.getStatus());

		// Set the status to PROCESSING
		disputeController.submitDispute(disputeId, principal);

		// Set the status to CANCELLED (can only be set to Cancelled after it's first been set to PROCESSING.
		disputeController.cancelDispute(disputeId, principal);

		// Assert status is set, rejected reason is NOT set.
		dispute = disputeController.getDispute(disputeId, principal).getBody();
		assertEquals(DisputeStatus.CANCELLED, dispute.getStatus());
		assertNull(dispute.getRejectedReason());
	}

	@Test
	public void testValidateDispute() {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		Long disputeId = disputeController.saveDispute(dispute);
		Principal principal = getPrincipal("testUser");

		// Retrieve it from the controller's endpoint
		dispute = disputeController.getDispute(disputeId, principal).getBody();
		assertEquals(disputeId, dispute.getId());
		assertEquals(DisputeStatus.NEW, dispute.getStatus());

		// Set the status to VALIDATED
		disputeController.validateDispute(disputeId, principal);

		// Assert status is set, rejected reason is NOT set.
		dispute = disputeController.getDispute(disputeId, principal).getBody();
		assertEquals(DisputeStatus.VALIDATED, dispute.getStatus());
		assertNull(dispute.getRejectedReason());
	}

	@Test
	@Transactional
	public void testUpdateDispute() {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		Long disputeId = disputeController.saveDispute(dispute);
		Principal principal = getPrincipal("testUser");

		// Retrieve it from the controller's endpoint
		dispute = disputeController.getDispute(disputeId, principal).getBody();
		assertEquals(disputeId, dispute.getId());

		// Create a new dispute with different values and update the existing dispute
		Dispute updatedDispute = RandomUtil.createDispute();
		updatedDispute.setSurname("Doe");
		updatedDispute.setGivenNames("John");
		disputeController.updateDispute(disputeId, updatedDispute, principal);

		// Assert db contains only the updated dispute record.
		dispute = disputeController.getDispute(disputeId, principal).getBody();
		assertEquals("Doe", dispute.getSurname());
		assertEquals("John", dispute.getGivenNames());
		Iterable<Dispute> allDisputes = disputeController.getAllDisputes(null, null);
		assertEquals(1, IterableUtils.size(allDisputes));
	}
	
	@Test
	@Transactional
	public void testGetAllDisputesByStatusAndCreatedTs() throws ParseException {
		// Create disputes with different status and createdTs to test
		Date now = new Date();
		Principal principal = getPrincipal("testUser");
		Dispute dispute = RandomUtil.createDispute();
		dispute.setStatus(DisputeStatus.NEW);
		Long disputeId = disputeController.saveDispute(dispute);
		dispute.setCreatedTs(DateUtils.addDays(now, -1));
		disputeController.updateDispute(disputeId, dispute, principal);
		
		Dispute dispute2 = RandomUtil.createDispute();
		dispute2.setStatus(DisputeStatus.PROCESSING);
		Long dispute2Id = disputeController.saveDispute(dispute2);
		dispute2.setCreatedTs(DateUtils.addDays(now, -2));
		disputeController.updateDispute(dispute2Id, dispute2, principal);
		
		Dispute dispute3 = RandomUtil.createDispute();
		dispute3.setStatus(DisputeStatus.CANCELLED);
		Long dispute3Id = disputeController.saveDispute(dispute3);
		dispute3.setCreatedTs(DateUtils.addDays(now, -3));
		disputeController.updateDispute(dispute3Id, dispute3, principal);

		// Assert controller returns all the disputes that were saved if no parameters passed.
		Iterable<Dispute> allDisputes = disputeController.getAllDisputes(null, null);
		assertEquals(3, IterableUtils.size(allDisputes));
		
		// Assert controller returns all disputes which do not have the specified type and older than specified date.
		SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd");
		List<Dispute> allDisputesWithStatusAndOlderThan = disputeController.getAllDisputes(sdf.parse(sdf.format(DateUtils.addDays(now, -1))), DisputeStatus.CANCELLED);
		assertEquals(1, IterableUtils.size(allDisputesWithStatusAndOlderThan));
		assertEquals(allDisputesWithStatusAndOlderThan.get(0).getStatus(), DisputeStatus.PROCESSING);
		
		// Assert controller returns all disputes which do not have the specified type.
		Iterable<Dispute> allDisputesWithStatus = disputeController.getAllDisputes(null, DisputeStatus.PROCESSING);
		assertEquals(2, IterableUtils.size(allDisputesWithStatus));
		
		// Assert controller returns all disputes older than specified date.
		Iterable<Dispute> allDisputesWithOlderThan = disputeController.getAllDisputes(sdf.parse(sdf.format(DateUtils.addDays(now, -2))), null);
		assertEquals(1, IterableUtils.size(allDisputesWithOlderThan));
	}

	// Helper method to return an instance of Principal
	private Principal getPrincipal(String name) {
		return new Principal() {
			@Override
			public String getName() {
				return name;
			}
		};
	}

}
