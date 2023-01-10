package ca.bc.gov.open.jag.tco.oracledataapi.controller.v1_0;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.junit.jupiter.api.Assertions.assertTrue;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

import java.util.Arrays;
import java.util.Date;
import java.util.List;
import java.util.UUID;

import javax.transaction.Transactional;

import org.apache.commons.collections4.IterableUtils;
import org.apache.commons.lang3.RandomStringUtils;
import org.apache.commons.lang3.time.DateFormatUtils;
import org.apache.commons.lang3.time.DateUtils;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvSource;
import org.springframework.http.MediaType;
import org.springframework.test.web.servlet.MvcResult;
import org.springframework.test.web.servlet.ResultActions;
import org.springframework.test.web.servlet.request.MockMvcRequestBuilders;

import com.fasterxml.jackson.core.type.TypeReference;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputantUpdateRequest;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputantUpdateRequestStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputantUpdateRequestType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeResult;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeHearingType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.util.DateUtil;
import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;

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
	public void testGetDisputeById() throws Exception {
		assertEquals(0, IterableUtils.toList(disputeRepository.findAll()).size());

		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		dispute.setTicketNumber("AX12345678");
		dispute.setStatus(DisputeStatus.NEW);
		dispute.setIssuedTs(DateUtils.parseDate("14:54", DateUtil.TIME_FORMAT));
		dispute.setViolationTicket(null);
		disputeRepository.save(dispute);

		assertEquals(1, IterableUtils.toList(disputeRepository.findAll()).size());

		// Retrieve the Dispute via the Controller
		ResultActions resultActions = mvc.perform(MockMvcRequestBuilders
				.get("/api/v1.0/dispute/{id}", dispute.getDisputeId())
				.principal(getPrincipal()))
				.andExpect(status().isOk());
		Dispute result = mapResult(resultActions, new TypeReference<Dispute>() {});
		assertNotNull(result);
	}

	@Test
	public void testGetJJDisputeByTicketNumber() throws Exception {
		assertEquals(0, IterableUtils.toList(jjDisputeRepository.findAll()).size());

		// Create a single Dispute
		JJDispute jjDispute = RandomUtil.createJJDispute();
		jjDispute.setTicketNumber("AX12345678");
		jjDispute.setStatus(JJDisputeStatus.IN_PROGRESS);
		jjDispute.setHearingType(JJDisputeHearingType.WRITTEN_REASONS);
		jjDisputeRepository.save(jjDispute);

		assertEquals(1, IterableUtils.toList(jjDisputeRepository.findAll()).size());

		// Retrieve the Dispute via the Controller
		ResultActions resultActions = mvc.perform(MockMvcRequestBuilders
				.get("/api/v1.0/jj/disputes")
				.param("ticketNumber", "AX12345678")
				.principal(getPrincipal()))
				.andExpect(status().isOk());
		List<JJDispute> result = mapResult(resultActions, new TypeReference<List<JJDispute>>() {});
		assertNotNull(result);
		assertEquals(1, result.size());
	}

	@Test
	public void testFindDisputeStatuses() throws Exception {
		assertEquals(0, IterableUtils.toList(disputeRepository.findAll()).size());
		assertEquals(0, IterableUtils.toList(jjDisputeRepository.findAll()).size());

		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		dispute.setTicketNumber("AX12345678");
		dispute.setStatus(DisputeStatus.NEW);
		dispute.setIssuedTs(DateUtils.parseDate("14:54", DateUtil.TIME_FORMAT));
		dispute.setViolationTicket(null);
		disputeRepository.save(dispute);

		// Create a single JJDispute
		JJDispute jjDispute = RandomUtil.createJJDispute();
		jjDispute.setTicketNumber("AX12345678");
		jjDispute.setStatus(JJDisputeStatus.IN_PROGRESS);
		jjDispute.setHearingType(JJDisputeHearingType.WRITTEN_REASONS);
		jjDisputeRepository.save(jjDispute);

		// Assert records exist in repo
		assertEquals(1, IterableUtils.toList(disputeRepository.findAll()).size());
		assertEquals(1, IterableUtils.toList(jjDisputeRepository.findAll()).size());

		// Assert jjDisputeRepository.findByTicketNumber() works
		List<JJDispute> jjDisputes = jjDisputeRepository.findByTicketNumber("AX12345678");
		assertEquals(1, jjDisputes.size());

		// Test the same jjDisputeRepository.findByTicketNumber(), but via /api/v1.0/dispute/status endpoint.
		List<DisputeResult> findResults = findDisputeStatuses("AX12345678", "14:54");
		assertEquals(1, findResults.size());
		assertEquals(DisputeStatus.NEW, findResults.get(0).getDisputeStatus());
		assertEquals(JJDisputeStatus.IN_PROGRESS, findResults.get(0).getJjDisputeStatus());
		assertEquals(JJDisputeHearingType.WRITTEN_REASONS, findResults.get(0).getJjDisputeHearingType());

		// try searching for a different ticketNumber. Expect no records.
		findResults = findDisputeStatuses("AX00000000", "14:54");
		assertEquals(0, findResults.size());

		// try searching for a different time. Expect no records.
		findResults = findDisputeStatuses("AX12345678", "14:55");
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

	@Test
	public void testSaveDisputantUpdateRequest_200() throws Exception {
		// Setup - persist a new random Dispute to the database
		Long disputeId = saveDispute(RandomUtil.createDispute());
		DisputantUpdateRequest disputantUpdateRequest = RandomUtil.createDisputantUpdateRequest(disputeId);

		// issue a POST request, expect 200
		Long disputantUpdateRequestId = saveDisputantUpdateRequest(disputantUpdateRequest);
		assertNotNull(disputantUpdateRequestId);
	}

	@Test
	public void testSaveDisputantUpdateRequest_404() throws Exception {
		// Try to issue a POST request to create a DisputantUpdateRequest for a Dispute that doesn't exist
		mvc.perform(MockMvcRequestBuilders
				.post("/api/v1.0/dispute/{guid}/updateRequest", UUID.randomUUID().toString()) // random guid that doesn't exist
				.principal(getPrincipal())
				.content(asJsonString(RandomUtil.createDisputantUpdateRequest(Long.valueOf(1))))
				.contentType(MediaType.APPLICATION_JSON)
				.accept(MediaType.APPLICATION_JSON))
				.andExpect(status().isNotFound());
	}

	@Test
	@SuppressWarnings("unused")
	public void testGetDisputantUpdateRequests_200() throws Exception {
		// Setup - persist a couple new random Disputes to the database along with a few DisputantUpdateRequests
		Long disputeId1 = saveDispute(RandomUtil.createDispute());
		Long disputeId2 = saveDispute(RandomUtil.createDispute());

		Long disputantUpdateRequest1 = saveDisputantUpdateRequest(RandomUtil.createDisputantUpdateRequest(disputeId1, DisputantUpdateRequestStatus.PENDING, DisputantUpdateRequestType.DISPUTANT_NAME));
		Long disputantUpdateRequest2 = saveDisputantUpdateRequest(RandomUtil.createDisputantUpdateRequest(disputeId1, DisputantUpdateRequestStatus.ACCEPTED, DisputantUpdateRequestType.DISPUTANT_ADDRESS));
		Long disputantUpdateRequest3 = saveDisputantUpdateRequest(RandomUtil.createDisputantUpdateRequest(disputeId2, DisputantUpdateRequestStatus.PENDING, DisputantUpdateRequestType.DISPUTANT_NAME));
		Long disputantUpdateRequest4 = saveDisputantUpdateRequest(RandomUtil.createDisputantUpdateRequest(disputeId2, DisputantUpdateRequestStatus.REJECTED, DisputantUpdateRequestType.DISPUTANT_PHONE));

		List<DisputantUpdateRequest> results;

		// Fetching for dispute 1 (filtered by status) should return the single disputantUpdateRequest1 record
		results = getGetDisputantUpdateRequests(disputeId1, DisputantUpdateRequestStatus.PENDING);
		assertEquals(1, results.size());
		assertEquals(disputantUpdateRequest1, results.get(0).getDisputantUpdateRequestId());

		// Fetching for dispute 2 (filtered by status) should return the single disputantUpdateRequest3 record
		results = getGetDisputantUpdateRequests(disputeId2, DisputantUpdateRequestStatus.PENDING);
		assertEquals(1, results.size());
		assertEquals(disputantUpdateRequest3, results.get(0).getDisputantUpdateRequestId());

		// Fetching for any dispute (filtered by status) should return the both disputantUpdateRequests 1 and 3 records
		results = getGetDisputantUpdateRequests(null, DisputantUpdateRequestStatus.PENDING);
		assertEquals(2, results.size());
		assertTrue(Arrays.asList(disputantUpdateRequest1, disputantUpdateRequest3).contains(results.get(0).getDisputantUpdateRequestId()));

		// Fetching for a dispute that doesn't exist should just return an empty list.
		assertFalse(Arrays.asList(disputeId1, disputeId1).contains(Long.valueOf(-1))); // assert the new ids are not -1
		results = getGetDisputantUpdateRequests(Long.valueOf(-1), DisputantUpdateRequestStatus.PENDING);
		assertEquals(0, results.size());
	}

	@Test
	public void testUpdateDisputantUpdateRequest_200() throws Exception {
		// Setup - persist a new random Dispute to the database
		Long disputeId = saveDispute(RandomUtil.createDispute());
		DisputantUpdateRequest disputantUpdateRequest = RandomUtil.createDisputantUpdateRequest(disputeId, DisputantUpdateRequestStatus.PENDING, DisputantUpdateRequestType.DISPUTANT_NAME);
		Long disputantUpdateRequestId = saveDisputantUpdateRequest(disputantUpdateRequest);
		assertNotNull(disputantUpdateRequestId);

		// assert the DisputantUpdateRequest was property persisted
		List<DisputantUpdateRequest> disputantUpdateRequests = getGetDisputantUpdateRequests(disputeId, DisputantUpdateRequestStatus.PENDING);
		assertEquals(1, disputantUpdateRequests.size());
		assertEquals(disputantUpdateRequestId, disputantUpdateRequests.get(0).getDisputantUpdateRequestId());
		assertEquals(DisputantUpdateRequestStatus.PENDING, disputantUpdateRequests.get(0).getStatus());
		assertEquals(DisputantUpdateRequestType.DISPUTANT_NAME, disputantUpdateRequests.get(0).getUpdateType());

		// issue a PUT request to update the record to ACCEPTED, expect 200
		mvc.perform(MockMvcRequestBuilders
				.put("/api/v1.0/dispute/updateRequest/{id}", disputantUpdateRequestId)
				.param("disputantUpdateRequestStatus", DisputantUpdateRequestStatus.ACCEPTED.toString())
				.principal(getPrincipal()))
				.andExpect(status().isOk());

		// reissue request, the returned result should have been updated.
		disputantUpdateRequests = getGetDisputantUpdateRequests(disputeId, DisputantUpdateRequestStatus.ACCEPTED);
		assertEquals(1, disputantUpdateRequests.size());
		assertEquals(disputantUpdateRequestId, disputantUpdateRequests.get(0).getDisputantUpdateRequestId());
		assertEquals(DisputantUpdateRequestStatus.ACCEPTED, disputantUpdateRequests.get(0).getStatus());
		assertEquals(DisputantUpdateRequestType.DISPUTANT_NAME, disputantUpdateRequests.get(0).getUpdateType());
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

	/** Issues a GET request to /api/v1.0/dispute/status with the required ticketNumber and time to find a Dispute.
	 * @throws Exception */
	private List<DisputeResult> findDisputeStatuses(String ticketNumber, String issuedTime) throws Exception {

		ResultActions resultActions = mvc.perform(MockMvcRequestBuilders
				.get("/api/v1.0/dispute/status")
				.param("ticketNumber", ticketNumber)
				.param("issuedTime", issuedTime)
				.principal(getPrincipal()))
				.andExpect(status().isOk());
		List<DisputeResult> result = mapResult(resultActions, new TypeReference<List<DisputeResult>>() {});
		return result;
	}

	/** Issues a POST request to /api/v1.0/dispute/{guid}/updateRequest to persist a new DisputantUpdateRequest in the database */
	private Long saveDisputantUpdateRequest(DisputantUpdateRequest disputantUpdateRequest) throws Exception {
		// Get the guid of the corresponding Disputeid
		Dispute dispute = getDispute(disputantUpdateRequest.getDisputeId());

		// perform a POST to the controller to create an updateRequest object
		MvcResult mvcResult = mvc.perform(MockMvcRequestBuilders
				.post("/api/v1.0/dispute/{guid}/updateRequest", dispute.getNoticeOfDisputeGuid())
				.principal(getPrincipal())
				.content(asJsonString(disputantUpdateRequest))
				.contentType(MediaType.APPLICATION_JSON)
				.accept(MediaType.APPLICATION_JSON))
				.andExpect(status().isOk())
				.andReturn();
		return Long.valueOf(mvcResult.getResponse().getContentAsString());
	}

	/** Issues a GET request to /api/v1.0/dispute/updateRequests to retrieve all DisputantUpdateRequests per for a Dispute */
	private List<DisputantUpdateRequest> getGetDisputantUpdateRequests(Long disputeId, DisputantUpdateRequestStatus status) throws Exception {
		ResultActions resultActions = mvc.perform(MockMvcRequestBuilders
				.get("/api/v1.0/dispute/updateRequests")
				.param("id", disputeId == null ? null : disputeId.toString())
				.param("status", status.toString())
				.principal(getPrincipal()))
				.andExpect(status().isOk());

		List<DisputantUpdateRequest> disputantUpdateRequests = mapResult(resultActions, new TypeReference<List<DisputantUpdateRequest>>() {});
		return disputantUpdateRequests;
	}

}
