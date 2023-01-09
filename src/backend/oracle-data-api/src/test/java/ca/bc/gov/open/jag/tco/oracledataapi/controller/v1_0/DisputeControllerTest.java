package ca.bc.gov.open.jag.tco.oracledataapi.controller.v1_0;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

import java.util.Date;
import java.util.List;

import javax.transaction.Transactional;

import org.apache.commons.lang3.RandomStringUtils;
import org.apache.commons.lang3.time.DateFormatUtils;
import org.apache.commons.lang3.time.DateUtils;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvSource;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.test.web.servlet.ResultActions;
import org.springframework.test.web.servlet.request.MockMvcRequestBuilders;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.util.UriComponentsBuilder;

import com.fasterxml.jackson.core.type.TypeReference;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputantUpdateRequest;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputantUpdateRequestStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeResult;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.util.DateUtil;
import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;
import io.swagger.v3.oas.annotations.Parameter;

class DisputeControllerTest extends BaseTestSuite {

	@Test
	public void testSaveDispute() throws Exception {
		// Assert db is empty and clean
		List<Dispute> allDisputes = getAllDisputes(null, null);
		assertEquals(0, allDisputes.size());

		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		Long disputeId = saveDispute(dispute);

		// Assert db contains the single created record
		allDisputes = getAllDisputes(null, null);
		assertEquals(1, allDisputes.size());
		assertEquals(disputeId, allDisputes.get(0).getDisputeId());
		assertEquals(dispute.getDisputantSurname(), allDisputes.get(0).getDisputantSurname());

		// Delete record
		deleteDispute(disputeId);

		// Assert db contains is empty again
		allDisputes = getAllDisputes(null, null);
		assertEquals(0, allDisputes.size());
	}

	@Test
	public void testSaveDispute_InvalidTicketNumber() throws Exception {
		// TCVP-1918 mismatched ticketNumber should yield a 400 BAD REQUEST

		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		dispute.setTicketNumber("AX00000000");
		dispute.getViolationTicket().setTicketNumber("AB11111111");

		mvc.perform(MockMvcRequestBuilders
				.post("/api/v1.0/dispute")
				.principal(getPrincipal())
				.content(asJsonString(dispute))
				.contentType(MediaType.APPLICATION_JSON)
				.accept(MediaType.APPLICATION_JSON))
				.andExpect(status().isBadRequest());
	}

	@Test
	public void testRejectDispute() throws Exception {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		Long disputeId = saveDispute(dispute);

		// Retrieve it from the controller's endpoint
		dispute = getDispute(disputeId);
		assertEquals(disputeId, dispute.getDisputeId());
		assertEquals(DisputeStatus.NEW, dispute.getStatus());

		// Set the status to REJECTED
		rejectDispute(disputeId, "Just because")
			.andExpect(status().isOk());

		// Assert status and reason are set.
		dispute = getDispute(disputeId);
		assertEquals(DisputeStatus.REJECTED, dispute.getStatus());
		assertEquals("Just because", dispute.getRejectedReason());
	}

	@Test
	public void testRejectDispute_409() throws Exception {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		Long disputeId = saveDispute(dispute);

		// Login
		setPrincipal("TestUser1");

		// Retrieve it from the controller's endpoint
		dispute = getDispute(disputeId);
		assertEquals(disputeId, dispute.getDisputeId());
		assertEquals(DisputeStatus.NEW, dispute.getStatus());
		assertEquals(dispute.getUserAssignedTo(), "TestUser1");

		// Login with a different user
		setPrincipal("TestUser2");

		// Attempt to set the status to REJECTED - should fail with a 409
		rejectDispute(disputeId, "Just because")
			.andExpect(status().isConflict());
	}

	@Test
	public void testRejectDispute_400() throws Exception {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		Long disputeId = saveDispute(dispute);

		// Retrieve it from the controller's endpoint
		dispute = getDispute(disputeId);
		assertEquals(disputeId, dispute.getDisputeId());
		assertEquals(DisputeStatus.NEW, dispute.getStatus());

		// try using an empty reason (should fail with 400 error)
		rejectDispute(disputeId, "")
			.andExpect(status().isBadRequest());

		// try using an long reason, > 256 (should fail with 400 error)
		rejectDispute(disputeId, RandomStringUtils.random(257))
			.andExpect(status().isBadRequest());

		String longString = RandomStringUtils.randomAlphabetic(256);

		// Set the status to REJECTED
		rejectDispute(disputeId, longString)
			.andExpect(status().isOk());

		// Assert status and reason are set.
		dispute = getDispute(disputeId);
		assertEquals(DisputeStatus.REJECTED, dispute.getStatus());
		assertEquals(longString, dispute.getRejectedReason());
	}

	@Test
	public void testSubmitDispute() throws Exception {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		Long disputeId = saveDispute(dispute);

		// Retrieve it from the controller's endpoint
		dispute = getDispute(disputeId);
		assertEquals(disputeId, dispute.getDisputeId());
		assertEquals(DisputeStatus.NEW, dispute.getStatus());

		// Set the status to PROCESSING
		submitDispute(disputeId);

		// Assert status is set, rejected reason is NOT set.
		dispute = getDispute(disputeId);
		assertEquals(DisputeStatus.PROCESSING, dispute.getStatus());
		assertNull(dispute.getRejectedReason());
	}

	@Test
	public void testCancelDispute() throws Exception {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		Long disputeId = saveDispute(dispute);

		// Retrieve it from the controller's endpoint
		dispute = getDispute(disputeId);
		assertEquals(disputeId, dispute.getDisputeId());
		assertEquals(DisputeStatus.NEW, dispute.getStatus());

		// Set the status to PROCESSING
		submitDispute(disputeId);

		// Set the status to CANCELLED (can only be set to Cancelled after it's first been set to PROCESSING.
		cancelDispute(disputeId);

		// Assert status is set, rejected reason is NOT set.
		dispute = getDispute(disputeId);
		assertEquals(DisputeStatus.CANCELLED, dispute.getStatus());
		assertNull(dispute.getRejectedReason());
	}

	@Test
	public void testValidateDispute() throws Exception {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		Long disputeId = saveDispute(dispute);

		// Retrieve it from the controller's endpoint
		dispute = getDispute(disputeId);
		assertEquals(disputeId, dispute.getDisputeId());
		assertEquals(DisputeStatus.NEW, dispute.getStatus());

		// Set the status to VALIDATED
		validateDispute(disputeId);

		// Assert status is set, rejected reason is NOT set.
		dispute = getDispute(disputeId);
		assertEquals(DisputeStatus.VALIDATED, dispute.getStatus());
		assertNull(dispute.getRejectedReason());
	}

	@Test
	@Transactional
	public void testUpdateDispute() throws Exception {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		dispute.setDisputantSurname("Steven");
		dispute.setDisputantGivenName1("Strange");
		Long disputeId = saveDispute(dispute);

		// Retrieve it from the controller's endpoint
		dispute = getDispute(disputeId);
		assertEquals(disputeId, dispute.getDisputeId());

		// Modify dispute with different values
		dispute.setDisputantSurname("Bruce");
		dispute.setDisputantGivenName1("Banner");
		updateDispute(disputeId, dispute);

		// Assert db contains only the updated dispute record.
		dispute = getDispute(disputeId);
		assertEquals("Bruce", dispute.getDisputantSurname());
		assertEquals("Banner", dispute.getDisputantGivenName1());
		List<Dispute> disputes = getAllDisputes(null, null);
		assertEquals(1, disputes.size());
	}

	@Test
	@Transactional
	public void testGetAllDisputesByStatusAndCreatedTs() throws Exception {
		// Create disputes with different status and createdTs to test
		Date now = new Date();
		Dispute dispute = RandomUtil.createDispute();
		dispute.setStatus(DisputeStatus.NEW);
		Long disputeId = saveDispute(dispute);
		dispute.setModifiedTs(DateUtils.addDays(now, -1));
		updateDispute(disputeId, dispute);

		Dispute dispute2 = RandomUtil.createDispute();
		dispute2.setStatus(DisputeStatus.PROCESSING);
		saveDispute(dispute2);

		Dispute dispute3 = RandomUtil.createDispute();
		dispute3.setStatus(DisputeStatus.CANCELLED);
		saveDispute(dispute3);

		// Assert controller returns all the disputes that were saved if no parameters passed.
		List<Dispute> allDisputes = getAllDisputes(null, null);
		assertEquals(3, allDisputes.size());

		// Assert controller returns all disputes which do not have the specified type.
		List<Dispute> allDisputesWithStatusAndOlderThan = getAllDisputes(null, DisputeStatus.CANCELLED);
		assertEquals(2, allDisputesWithStatusAndOlderThan.size());

		// Assert controller returns all disputes which do not have the specified type.
		List<Dispute> allDisputesWithStatus = getAllDisputes(null, DisputeStatus.PROCESSING);
		assertEquals(2, allDisputesWithStatus.size());
	}

	@Test
	public void testFindByTicketNumberAndTime() throws Exception {
		// Happy path. Expect results on a valid match.

		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		dispute.setTicketNumber("AX12345678");
		dispute.setIssuedTs(DateUtils.parseDate("14:54", DateUtil.TIME_FORMAT));
		dispute.setViolationTicket(null);
		Long disputeId = saveDispute(dispute);

		// try searching for exact match. Expect to find the dispute
		List<DisputeResult> findResults = findDispute("AX12345678", "14:54");
		assertEquals(1, findResults.size());
		assertEquals(disputeId, findResults.get(0).getDisputeId());
		assertEquals(DisputeStatus.NEW, findResults.get(0).getDisputeStatus());

		// try searching for a different ticketNumber. Expect no records.
		findResults = findDispute("AX00000000", "14:54");
		assertEquals(0, findResults.size());

		// try searching for a different time. Expect no records.
		findResults = findDispute("AX12345678", "14:55");
		assertEquals(0, findResults.size());
	}

	@Test
	public void testFindByTicketNumberAndTime_Null() throws Exception {
		mvc.perform(MockMvcRequestBuilders
				.get("/api/v1.0/dispute/status")) // missing parameters
				.andExpect(status().isBadRequest());
	}

	@ParameterizedTest
	@CsvSource({
		"AB12345678,123abc,", // should be of the format HH:mm
		"AB12345678,55:99,",  // invalid time
		"AB123456789,14:23,", // too long
		"ABC1234567,14:23,",  // invalid regex
		",,",                 // missing parameters
		"AB12345678,,",       // missing parameter
		",14:23,",            // missing parameter
		})
	public void testFindByTicketNumberAndTime_Invalid(String ticketNumber, String issuedTime, String noticeOfDisputeGuid) throws Exception {
		mvc.perform(MockMvcRequestBuilders
				.get("/api/v1.0/dispute/status")
				.param("ticketNumber", ticketNumber)
				.param("issuedTime", issuedTime)
				.param("noticeOfDisputeGuid", noticeOfDisputeGuid))
				.andExpect(status().isBadRequest());
	}

	@Test
	public void testResetEmail_200() throws Exception {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		dispute.setEmailAddress("oldaddress@somewhere.com");
		dispute.setEmailAddressVerified(Boolean.TRUE);
		Long disputeId = saveDispute(dispute);

		// Load the dispute to confirm the database values are correct
		dispute = getDispute(disputeId);
		assertEquals("oldaddress@somewhere.com", dispute.getEmailAddress());
		assertEquals(Boolean.TRUE, dispute.getEmailAddressVerified());

		// attempt to reset the email address
		mvc.perform(MockMvcRequestBuilders
				.put("/api/v1.0/dispute/{id}/email/reset", disputeId)
				.param("email", "newaddress@somewhere.com"))
				.andExpect(status().isOk());

		// reload the dispute to confirm the emailAddress has been updated.
		Dispute updatedDispute = getDispute(disputeId);
		assertEquals("newaddress@somewhere.com", updatedDispute.getEmailAddress());
		assertEquals(Boolean.FALSE, updatedDispute.getEmailAddressVerified());
	}

	@Test
	public void testResetEmail_400() throws Exception {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		dispute.setEmailAddress("oldaddress@somewhere.com");
		dispute.setEmailAddressVerified(Boolean.TRUE);
		Long disputeId = saveDispute(dispute);

		// Load the dispute to confirm the database values are correct
		dispute = getDispute(disputeId);
		assertEquals("oldaddress@somewhere.com", dispute.getEmailAddress());
		assertEquals(Boolean.TRUE, dispute.getEmailAddressVerified());

		// attempt to reset with an overly long email address
		mvc.perform(MockMvcRequestBuilders
				.put("/api/v1.0/dispute/{id}/email/reset", disputeId)
				.param("email", RandomUtil.randomAlphabetic(101))) // create a string 101 characters long - should fail.
				.andExpect(status().isBadRequest());

		// attempt to reset with a missing email address - should be permitted to set the email to blank.
		mvc.perform(MockMvcRequestBuilders
				.put("/api/v1.0/dispute/{id}/email/reset", disputeId))
				.andExpect(status().isOk());

		dispute = getDispute(disputeId);
		assertNull(dispute.getEmailAddress());
		assertEquals(Boolean.TRUE, dispute.getEmailAddressVerified());
	}

	@Test
	public void testResetEmail_404() throws Exception {
		// attempt to reset the email address on an invalid Dispute
		mvc.perform(MockMvcRequestBuilders
				.put("/api/v1.0/dispute/{id}/email/reset", Long.valueOf(1l))
				.param("email", "newaddress@somewhere.com"))
				.andExpect(status().isNotFound());
	}

	@Test
	public void testVerifyEmail_200() throws Exception {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		dispute.setEmailAddress("myaddress@somewhere.com");
		dispute.setEmailAddressVerified(Boolean.FALSE);
		Long disputeId = saveDispute(dispute);

		// Load the dispute to confirm the database values are correct
		dispute = getDispute(disputeId);
		assertEquals("myaddress@somewhere.com", dispute.getEmailAddress());
		assertEquals(Boolean.FALSE, dispute.getEmailAddressVerified());

		// attempt to verify the email address
		mvc.perform(MockMvcRequestBuilders
				.put("/api/v1.0/dispute/{id}/email/verify", disputeId))
				.andExpect(status().isOk());

		// reload the dispute to confirm the emailAddress has been updated.
		Dispute updatedDispute = getDispute(disputeId);
		assertEquals("myaddress@somewhere.com", updatedDispute.getEmailAddress());
		assertEquals(Boolean.TRUE, updatedDispute.getEmailAddressVerified());
	}

	@Test
	public void testVerifyEmail_404() throws Exception {
		// attempt to verify the email address with an invalid dispute id
		mvc.perform(MockMvcRequestBuilders
				.put("/api/v1.0/dispute/{id}/email/verify", Long.valueOf(-1L)))
				.andExpect(status().isNotFound());
	}

	/** Issue a POST request to /api/v1.0/dispute. The appropriate controller is automatically called by the DispatchServlet */
	private Long saveDispute(Dispute dispute) {
		return postForObject(fromUriString("/dispute"), dispute, Long.class);
	}

	/**
	 * Issue a PUT request to /api/v1.0/dispute/{id}. The appropriate controller is automatically called by the DispatchServlet
	 * @throws Exception
	 */
	private Dispute updateDispute(Long disputeId, Dispute dispute) throws Exception {
		ResultActions resultActions = mvc.perform(MockMvcRequestBuilders
				.put("/api/v1.0/dispute/{id}", disputeId)
				.principal(getPrincipal())
				.content(asJsonString(dispute))
				.contentType(MediaType.APPLICATION_JSON)
				.accept(MediaType.APPLICATION_JSON))
				.andExpect(status().isOk());
		Dispute result = mapResult(resultActions, new TypeReference<Dispute>() {});
		return result;
	}

	/**
	 * Issues a GET request to /api/v1.0/dispute/{id}. The appropriate controller is automatically called by the DispatchServlet
	 * @throws Exception
	 */
	private Dispute getDispute(Long disputeId) throws Exception {
		ResultActions resultActions = mvc.perform(MockMvcRequestBuilders
				.get("/api/v1.0/dispute/{id}", disputeId)
				.principal(getPrincipal()))
				.andExpect(status().isOk());
		Dispute result = mapResult(resultActions, new TypeReference<Dispute>() {});
		return result;
	}

	/**
	 * Issues a GET request to /api/v1.0/disputes. The appropriate controller is automatically called by the DispatchServlet
	 * @throws Exception
	 */
	private List<Dispute> getAllDisputes(Date olderThan, DisputeStatus excludeStatus) throws Exception {
		ResultActions resultActions = mvc.perform(MockMvcRequestBuilders
				.get("/api/v1.0/disputes")
				.param("olderThan", olderThan == null ? null : DateFormatUtils.format(olderThan, "yyyy-MM-dd"))
				.param("excludeStatus", excludeStatus == null ? null : excludeStatus.name())
				.principal(getPrincipal()))
				.andExpect(status().isOk());
		List<Dispute> result = mapResult(resultActions, new TypeReference<List<Dispute>>() {});
		return result;
	}

	/**
	 * Issues a GET request to /api/v1.0/dispute/updateRequests. The appropriate controller is automatically called by the DispatchServlet
	 * @throws Exception
	 */
	public List<DisputantUpdateRequest> getDisputantUpdateRequests(Long id, DisputantUpdateRequestStatus status) {
		ResultActions resultActions;
		try {
			resultActions = mvc.perform(MockMvcRequestBuilders
					.get("/api/v1.0/dispute/updateRequests")
					.param("id", Long.toString(id))
					.param("status", status == null ? null : status.name())
					.principal(getPrincipal()))
					.andExpect(status().isOk());
			List<DisputantUpdateRequest> result = mapResult(resultActions, new TypeReference<List<DisputantUpdateRequest>>() {});
			return result;

		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			return null;
		}
	}
	
	/**
	 * Issues a DELETE request to /api/v1.0/dispute/{id}. The appropriate controller is automatically called by the DispatchServlet
	 * @throws Exception
	 */
	private void deleteDispute(Long disputeId) throws Exception {
		mvc.perform(MockMvcRequestBuilders
				.delete("/api/v1.0/dispute/{id}", disputeId)
				.principal(getPrincipal()))
				.andExpect(status().isOk());
	}

	/**
	 * Issues a PUT request to /api/v1.0/dispute/{id}/reject. The appropriate controller is automatically called by the DispatchServlet
	 * @throws Exception
	 */
	private ResultActions rejectDispute(Long disputeId, String rejectedReason) throws Exception {
		ResultActions resultActions = mvc.perform(MockMvcRequestBuilders
				.put("/api/v1.0/dispute/{id}/reject", disputeId)
				.content(rejectedReason)
				.principal(getPrincipal()));
		return resultActions;
	}

	/**
	 * Issues a PUT request to /api/v1.0/dispute/{id}/submit. The appropriate controller is automatically called by the DispatchServlet
	 * @throws Exception
	 */
	private Dispute submitDispute(Long disputeId) throws Exception {
		ResultActions resultActions = mvc.perform(MockMvcRequestBuilders
				.put("/api/v1.0/dispute/{id}/submit", disputeId)
				.principal(getPrincipal()))
				.andExpect(status().isOk());
		Dispute result = mapResult(resultActions, new TypeReference<Dispute>() {});
		return result;
	}

	/**
	 * Issues a PUT request to /api/v1.0/dispute/{id}/cancel. The appropriate controller is automatically called by the DispatchServlet
	 * @throws Exception
	 */
	private Dispute cancelDispute(Long disputeId) throws Exception {
		ResultActions resultActions = mvc.perform(MockMvcRequestBuilders
				.put("/api/v1.0/dispute/{id}/cancel", disputeId)
				.principal(getPrincipal()))
				.andExpect(status().isOk());
		Dispute result = mapResult(resultActions, new TypeReference<Dispute>() {});
		return result;
	}

	/**
	 * Issues a PUT request to /api/v1.0/dispute/{id}/validate. The appropriate controller is automatically called by the DispatchServlet
	 * @throws Exception
	 */
	private Dispute validateDispute(Long disputeId) throws Exception {
		ResultActions resultActions = mvc.perform(MockMvcRequestBuilders
				.put("/api/v1.0/dispute/{id}/validate", disputeId)
				.principal(getPrincipal()))
				.andExpect(status().isOk());
		Dispute result = mapResult(resultActions, new TypeReference<Dispute>() {});
		return result;
	}

	/** Issues a GET request to /api/v1.0/dispute with the required ticketNumber and time to find a Dispute. */
	private List<DisputeResult> findDispute(String ticketNumber, String issuedTime) {
		UriComponentsBuilder uriBuilder = fromUriString("/dispute/status")
				.queryParam("ticketNumber", ticketNumber)
				.queryParam("issuedTime", issuedTime);
		ResponseEntity<List<DisputeResult>> results = getForEntity(uriBuilder, new ParameterizedTypeReference<List<DisputeResult>>() {});
		return results.getBody();
	}

}
