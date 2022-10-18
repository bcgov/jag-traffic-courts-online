package ca.bc.gov.open.jag.tco.oracledataapi.controller.v1_0;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.junit.jupiter.api.Assertions.assertThrows;

import java.security.Principal;
import java.text.ParseException;
import java.util.Date;
import java.util.List;

import javax.transaction.Transactional;
import javax.validation.ConstraintViolationException;

import org.apache.commons.collections4.IterableUtils;
import org.apache.commons.lang3.RandomStringUtils;
import org.apache.commons.lang3.time.DateUtils;
import org.junit.jupiter.api.Disabled;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.util.UriComponentsBuilder;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeResult;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;

class DisputeControllerTest extends BaseTestSuite {

	@Autowired
	@Qualifier("DisputeControllerV1_0")
	// TODO: remove me in favour of issuing appropriate GET, POST, or DELETE requests to the DispatcherServlet - this also tests spring wiring.
	// @see #findDispute()
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
		assertEquals(disputeId, allDisputes.get(0).getDisputeId());
		assertEquals(dispute.getDisputantSurname(), allDisputes.get(0).getDisputantSurname());

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
		assertEquals(disputeId, dispute.getDisputeId());
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
		assertEquals(disputeId, dispute.getDisputeId());
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
		assertEquals(disputeId, dispute.getDisputeId());
		assertEquals(DisputeStatus.NEW, dispute.getStatus());

		// Set the status to PROCESSING
		disputeController.submitDispute(disputeId, principal);

		// Assert status is set, rejected reason is NOT set.
		dispute = disputeController.getDispute(disputeId, principal).getBody();
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
		assertEquals(disputeId, dispute.getDisputeId());
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
		assertEquals(disputeId, dispute.getDisputeId());
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
		assertEquals(disputeId, dispute.getDisputeId());

		// Create a new dispute with different values and update the existing dispute
		Dispute updatedDispute = RandomUtil.createDispute();
		updatedDispute.setDisputantSurname("Doe");
		updatedDispute.setDisputantGivenName1("John");
		disputeController.updateDispute(disputeId, updatedDispute, principal);

		// Assert db contains only the updated dispute record.
		dispute = disputeController.getDispute(disputeId, principal).getBody();
		assertEquals("Doe", dispute.getDisputantSurname());
		assertEquals("John", dispute.getDisputantGivenName1());
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
		dispute.setModifiedTs(DateUtils.addDays(now, -1));
		disputeController.updateDispute(disputeId, dispute, principal);

		Dispute dispute2 = RandomUtil.createDispute();
		dispute2.setStatus(DisputeStatus.PROCESSING);
		disputeController.saveDispute(dispute2);

		Dispute dispute3 = RandomUtil.createDispute();
		dispute3.setStatus(DisputeStatus.CANCELLED);
		disputeController.saveDispute(dispute3);

		// Assert controller returns all the disputes that were saved if no parameters passed.
		Iterable<Dispute> allDisputes = disputeController.getAllDisputes(null, null);
		assertEquals(3, IterableUtils.size(allDisputes));

		// Assert controller returns all disputes which do not have the specified type.
		List<Dispute> allDisputesWithStatusAndOlderThan = disputeController.getAllDisputes(null, DisputeStatus.CANCELLED);
		assertEquals(2, IterableUtils.size(allDisputesWithStatusAndOlderThan));

		// Assert controller returns all disputes which do not have the specified type.
		Iterable<Dispute> allDisputesWithStatus = disputeController.getAllDisputes(null, DisputeStatus.PROCESSING);
		assertEquals(2, IterableUtils.size(allDisputesWithStatus));
	}

	@Test
	public void testFindByTicketNumberAndTime() throws Exception {
		// Happy path. Expect results on a valid match.

		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		dispute.setTicketNumber("AX12345678");
		dispute.setIssuedDate(DateUtils.parseDate("14:54", "HH:mm"));
		Long disputeId = saveDispute(dispute);

		// try searching for exact match. Expect to find the dispute
		ResponseEntity<List<DisputeResult>> responseEntity = findDispute("AX12345678", "14:54");
		assertEquals(HttpStatus.OK, responseEntity.getStatusCode());
		assertNotNull(responseEntity.getBody());
		assertEquals(1, responseEntity.getBody().size());
		assertEquals(disputeId, responseEntity.getBody().get(0).getDisputeId());
		assertEquals(DisputeStatus.NEW, responseEntity.getBody().get(0).getDisputeStatus());

		// try searching for a different ticketNumber. Expect no records.
		responseEntity = findDispute("AX00000000", "14:54");
		assertEquals(HttpStatus.OK, responseEntity.getStatusCode());
		assertNotNull(responseEntity.getBody());
		assertEquals(0, responseEntity.getBody().size());

		// try searching for a different time. Expect no records.
		responseEntity = findDispute("AX12345678", "14:55");
		assertEquals(HttpStatus.OK, responseEntity.getStatusCode());
		assertNotNull(responseEntity.getBody());
		assertEquals(0, responseEntity.getBody().size());
	}

	@Test
	@Disabled
	// FIXME: this should work, but getting a RestClientException due to the validation error. It's trying to parse a json result but getting a 400 error page (as expected) but the restTemplate doens't know how to parse that.
	public void testFindByTicketNumberAndTimeNull() throws Exception {
		ResponseEntity<List<DisputeResult>> responseEntity = findDispute(null, null);
		assertEquals(HttpStatus.BAD_REQUEST, responseEntity.getStatusCode());
	}

	/** Issue a POST request to /api/v1.0/dispute. The appropriate controller is automatically called by the DispatchServlet */
	private Long saveDispute(Dispute dispute) {
		return postForObject(fromUriString("/dispute"), dispute, Long.class);
	}

	/** Issues a GET request to /api/v1.0/dispute with the required ticketNumber and time to find a Dispute. */
	private ResponseEntity<List<DisputeResult>> findDispute(String ticketNumber, String issuedTime) {
		UriComponentsBuilder uriBuilder = fromUriString("/dispute")
				.queryParam("ticketNumber", ticketNumber)
				.queryParam("issuedTime", issuedTime);
		return getForEntity(uriBuilder, new ParameterizedTypeReference<List<DisputeResult>>() {});
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
