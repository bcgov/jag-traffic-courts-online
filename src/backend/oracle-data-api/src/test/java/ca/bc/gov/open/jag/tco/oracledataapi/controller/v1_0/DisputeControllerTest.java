package ca.bc.gov.open.jag.tco.oracledataapi.controller.v1_0;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.junit.jupiter.api.Assertions.assertThrows;

import java.security.Principal;
import java.util.List;
import java.util.UUID;

import javax.transaction.Transactional;
import javax.validation.ConstraintViolationException;

import org.apache.commons.collections4.IterableUtils;
import org.apache.commons.lang3.RandomStringUtils;
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
		List<Dispute> allDisputes = IterableUtils.toList(disputeController.getAllDisputes(null));
		assertEquals(0, allDisputes.size());

		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		UUID disputeId = disputeController.saveDispute(dispute);

		// Assert db contains the single created record
		allDisputes = IterableUtils.toList(disputeController.getAllDisputes(null));
		assertEquals(1, allDisputes.size());
		assertEquals(disputeId, allDisputes.get(0).getId());
		assertEquals(dispute.getSurname(), allDisputes.get(0).getSurname());

		// Delete record
		disputeController.deleteDispute(disputeId);

		// Assert db contains is empty again
		allDisputes = IterableUtils.toList(disputeController.getAllDisputes(null));
		assertEquals(0, allDisputes.size());
	}

	@Test
	public void testRejectDisputeSuccess() {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		UUID disputeId = disputeController.saveDispute(dispute);
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
		UUID disputeId = disputeController.saveDispute(dispute);
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
		UUID disputeId = disputeController.saveDispute(dispute);
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
		UUID disputeId = disputeController.saveDispute(dispute);
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
	@Transactional
	public void testUpdateDispute() {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		UUID disputeId = disputeController.saveDispute(dispute);
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
		Iterable<Dispute> allDisputes = disputeController.getAllDisputes(null);
		assertEquals(1, IterableUtils.size(allDisputes));
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
