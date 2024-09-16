package ca.bc.gov.open.jag.tco.oracledataapi.controller.v1_0;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.mockito.ArgumentMatchers.anyLong;
import static org.mockito.Mockito.doNothing;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.NoSuchElementException;

import javax.transaction.Transactional;

import org.apache.commons.collections4.IterableUtils;
import org.apache.commons.lang3.RandomStringUtils;
import org.apache.commons.lang3.time.DateFormatUtils;
import org.apache.commons.lang3.time.DateUtils;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvSource;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.Mockito;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.test.web.servlet.ResultActions;
import org.springframework.test.web.servlet.request.MockMvcRequestBuilders;

import com.fasterxml.jackson.core.type.TypeReference;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeListItem;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeResult;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequest;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequestStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequestType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeHearingType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.service.DisputeService;
import ca.bc.gov.open.jag.tco.oracledataapi.util.DateUtil;
import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;

class DisputeControllerTest extends BaseTestSuite {

	@InjectMocks
	private DisputeController disputeController;

	@Mock
	private DisputeService service;

	@Test
	public void testSaveDispute() throws Exception {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		Long disputeId = saveDispute(dispute);
		
		Dispute savedDispute = getDispute(disputeId, true);

		// Assert db contains the single created record
		assertNotNull(savedDispute);
		assertEquals(disputeId, savedDispute.getDisputeId());
		assertEquals(dispute.getDisputantSurname(), savedDispute.getDisputantSurname());

		// Delete record
		deleteDispute(disputeId);

		// Assert db does not contain the dispute anymore
		mvc.perform(MockMvcRequestBuilders
				.get("/api/v1.0/dispute/{id}/{isAssign}", disputeId, false)
				.principal(getPrincipal()))
				.andExpect(status().isNotFound());
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
		dispute = getDispute(disputeId, true);
		assertEquals(disputeId, dispute.getDisputeId());
		assertEquals(DisputeStatus.NEW, dispute.getStatus());

		// Set the status to REJECTED
		rejectDispute(disputeId, "Just because")
		.andExpect(status().isOk());

		// Assert status and reason are set.
		dispute = getDispute(disputeId, true);
		assertEquals(DisputeStatus.REJECTED, dispute.getStatus());
		assertEquals("Just because", dispute.getRejectedReason());
	}

	@Test
	public void testRejectDispute_409() throws Exception {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		Long disputeId = saveDispute(dispute);

		// Login
		setPrincipal("TestUser1", true);

		// Retrieve it from the controller's endpoint
		dispute = getDispute(disputeId, true);
		assertEquals(disputeId, dispute.getDisputeId());
		assertEquals(DisputeStatus.NEW, dispute.getStatus());
		assertEquals(dispute.getUserAssignedTo(), "TestUser1");

		// Login with a different user
		setPrincipal("TestUser2", true);

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
		dispute = getDispute(disputeId, true);
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
		dispute = getDispute(disputeId, true);
		assertEquals(DisputeStatus.REJECTED, dispute.getStatus());
		assertEquals(longString, dispute.getRejectedReason());
	}

	@Test
	public void testSubmitDispute() throws Exception {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		Long disputeId = saveDispute(dispute);

		// Retrieve it from the controller's endpoint
		dispute = getDispute(disputeId, true);
		assertEquals(disputeId, dispute.getDisputeId());
		assertEquals(DisputeStatus.NEW, dispute.getStatus());

		// Set the status to PROCESSING
		submitDispute(disputeId);

		// Assert status is set, rejected reason is NOT set.
		dispute = getDispute(disputeId, true);
		assertEquals(DisputeStatus.PROCESSING, dispute.getStatus());
		assertNull(dispute.getRejectedReason());
	}

	@Test
	public void testCancelDispute() throws Exception {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		Long disputeId = saveDispute(dispute);

		// Retrieve it from the controller's endpoint
		dispute = getDispute(disputeId, true);
		assertEquals(disputeId, dispute.getDisputeId());
		assertEquals(DisputeStatus.NEW, dispute.getStatus());

		// Set the status to PROCESSING
		submitDispute(disputeId);

		// Set the status to CANCELLED (can only be set to Cancelled after it's first been set to PROCESSING.
		cancelDispute(disputeId, "Just because");

		// Assert status is set, rejected reason is NOT set.
		dispute = getDispute(disputeId, true);
		assertEquals(DisputeStatus.CANCELLED, dispute.getStatus());
		assertEquals("Just because", dispute.getRejectedReason());
	}

	@Test
	public void testCancelDispute_400() throws Exception {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		dispute.setStatus(DisputeStatus.PROCESSING);		
		Long disputeId = saveDispute(dispute);

		// Retrieve it from the controller's endpoint
		dispute = getDispute(disputeId, true);
		assertEquals(disputeId, dispute.getDisputeId());
		assertEquals(DisputeStatus.PROCESSING, dispute.getStatus());

		// try using an empty reason (should fail with 400 error)
		cancelDispute(disputeId, "")
		.andExpect(status().isBadRequest());

		// try using an long reason, > 256 (should fail with 400 error)
		cancelDispute(disputeId, RandomStringUtils.random(257))
		.andExpect(status().isBadRequest());

		String longString = RandomStringUtils.randomAlphabetic(256);

		// Set the status to CANCELLED
		cancelDispute(disputeId, longString)
		.andExpect(status().isOk());

		// Assert status and reason are set.
		dispute = getDispute(disputeId, true);
		assertEquals(DisputeStatus.CANCELLED, dispute.getStatus());
		assertEquals(longString, dispute.getRejectedReason());
	}

	@Test
	public void testValidateDispute() throws Exception {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		Long disputeId = saveDispute(dispute);

		// Retrieve it from the controller's endpoint
		dispute = getDispute(disputeId, true);
		assertEquals(disputeId, dispute.getDisputeId());
		assertEquals(DisputeStatus.NEW, dispute.getStatus());

		// Set the status to VALIDATED
		validateDispute(disputeId);

		// Assert status is set, rejected reason is NOT set.
		dispute = getDispute(disputeId, true);
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
		dispute = getDispute(disputeId, true);
		assertEquals(disputeId, dispute.getDisputeId());

		// Modify dispute with different values
		dispute.setDisputantSurname("Bruce");
		dispute.setDisputantGivenName1("Banner");
		updateDispute(disputeId, dispute);

		// Assert db contains only the updated dispute record.
		dispute = getDispute(disputeId, true);
		assertEquals("Bruce", dispute.getDisputantSurname());
		assertEquals("Banner", dispute.getDisputantGivenName1());
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
				.get("/api/v1.0/dispute/{id}/{isAssign}", dispute.getDisputeId(), true)
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
		jjDisputeRepository.saveAndFlush(jjDispute);

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
		jjDisputeRepository.saveAndFlush(jjDispute);

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
		dispute = getDispute(disputeId, false);
		assertEquals("oldaddress@somewhere.com", dispute.getEmailAddress());
		assertEquals(Boolean.TRUE, dispute.getEmailAddressVerified());

		// attempt to reset the email address
		mvc.perform(MockMvcRequestBuilders
				.put("/api/v1.0/dispute/{id}/email/reset", disputeId)
				.param("email", "newaddress@somewhere.com"))
		.andExpect(status().isOk());

		// reload the dispute to confirm the emailAddress has been updated.
		Dispute updatedDispute = getDispute(disputeId, false);
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
		dispute = getDispute(disputeId, false);
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

		dispute = getDispute(disputeId, false);
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
		dispute = getDispute(disputeId, false);
		assertEquals("myaddress@somewhere.com", dispute.getEmailAddress());
		assertEquals(Boolean.FALSE, dispute.getEmailAddressVerified());

		// attempt to verify the email address
		mvc.perform(MockMvcRequestBuilders
				.put("/api/v1.0/dispute/{id}/email/verify", disputeId))
		.andExpect(status().isOk());

		// reload the dispute to confirm the emailAddress has been updated.
		Dispute updatedDispute = getDispute(disputeId, false);
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
	public void testSaveDisputeUpdateRequest_200() throws Exception {
		// Setup - create a new random Dispute and DisutantUpdateRequest to return
		Dispute dispute = RandomUtil.createDispute();
		DisputeUpdateRequest disputeUpdateRequest = RandomUtil.createDisputeUpdateRequest(dispute.getDisputeId());

		// Mock underlying saveDisputeUpdateRequest service
		Mockito.when(service.saveDisputeUpdateRequest(dispute.getNoticeOfDisputeGuid(), disputeUpdateRequest)).thenReturn(disputeUpdateRequest);

		// issue a POST request, expect 200
		ResponseEntity<Long> controllerResponse = disputeController.saveDisputeUpdateRequest(dispute.getNoticeOfDisputeGuid(), disputeUpdateRequest);
		Mockito.verify(service).saveDisputeUpdateRequest(dispute.getNoticeOfDisputeGuid(), disputeUpdateRequest);
		Long resultId = controllerResponse.getBody();
		assertNotNull(resultId);
		assertEquals(controllerResponse.getStatusCode().value(), HttpStatus.OK.value());
	}

	@Test
	public void testSaveDisputeUpdateRequest_404() throws Exception {
		// Setup - create a new random Dispute and DisutantUpdateRequest to return
		Dispute dispute = RandomUtil.createDispute();
		Long disputeId = 2L;
		DisputeUpdateRequest disputeUpdateRequest = RandomUtil.createDisputeUpdateRequest(disputeId);

		// Mock underlying saveDisputeUpdateRequest service
		Mockito.when(service.saveDisputeUpdateRequest(dispute.getNoticeOfDisputeGuid(), disputeUpdateRequest)).thenThrow(NoSuchElementException.class);

		// issue a POST request, expect 404
		Assertions.assertThrows(NoSuchElementException.class, () -> {
			disputeController.saveDisputeUpdateRequest(dispute.getNoticeOfDisputeGuid(), disputeUpdateRequest);
		});
	}

	@Test
	@SuppressWarnings("unused")
	public void testGetDisputeUpdateRequests_200() throws Exception {
		// Setup - create a couple new random Disputes along with a few DisputeUpdateRequests
		Dispute dispute1 = RandomUtil.createDispute();
		saveDispute(dispute1);
		Dispute dispute2 = RandomUtil.createDispute();
		saveDispute(dispute2);
		assertEquals(2, IterableUtils.toList(disputeRepository.findAll()).size());

		DisputeUpdateRequest disputeUpdateRequest1 = RandomUtil.createDisputeUpdateRequest(dispute1.getDisputeId(), DisputeUpdateRequestStatus.PENDING, DisputeUpdateRequestType.DISPUTANT_NAME);
		DisputeUpdateRequest disputeUpdateRequest2 = RandomUtil.createDisputeUpdateRequest(dispute1.getDisputeId(), DisputeUpdateRequestStatus.ACCEPTED, DisputeUpdateRequestType.DISPUTANT_ADDRESS);
		DisputeUpdateRequest disputeUpdateRequest3 = RandomUtil.createDisputeUpdateRequest(dispute2.getDisputeId(), DisputeUpdateRequestStatus.PENDING, DisputeUpdateRequestType.DISPUTANT_NAME);
		DisputeUpdateRequest disputeUpdateRequest4 = RandomUtil.createDisputeUpdateRequest(dispute2.getDisputeId(), DisputeUpdateRequestStatus.REJECTED, DisputeUpdateRequestType.DISPUTANT_PHONE);

		List<DisputeUpdateRequest> results = new ArrayList<DisputeUpdateRequest>();
		results.add(disputeUpdateRequest2);

		// issue a GET request, expect 200 and correct ID and status
		Mockito.when(service.findDisputeUpdateRequestByDisputeIdAndStatus(dispute1.getDisputeId(), DisputeUpdateRequestStatus.ACCEPTED)).thenReturn(results);
		ResponseEntity<List<DisputeUpdateRequest>> controllerResponse = disputeController.getDisputeUpdateRequests(dispute1.getDisputeId(), DisputeUpdateRequestStatus.ACCEPTED);
		List<DisputeUpdateRequest> resultList = controllerResponse.getBody();
		assertNotNull(resultList);
		assertEquals(disputeUpdateRequest2.getDisputeUpdateRequestId(), results.get(0).getDisputeUpdateRequestId());
		assertEquals(disputeUpdateRequest2.getStatus(), results.get(0).getStatus());
		assertEquals(controllerResponse.getStatusCode().value(), HttpStatus.OK.value());

		// Get all update requests with null params
		results.add(disputeUpdateRequest1);
		results.add(disputeUpdateRequest3);
		results.add(disputeUpdateRequest4);
		Mockito.when(service.findDisputeUpdateRequestByDisputeIdAndStatus(null, null)).thenReturn(results);
		controllerResponse = disputeController.getDisputeUpdateRequests(null, null);
		resultList = controllerResponse.getBody();
		assertNotNull(resultList);
		assertEquals(resultList.size(),4);

		// Get all update requests with pending status
		results.clear();
		results.add(disputeUpdateRequest1);
		results.add(disputeUpdateRequest3);
		Mockito.when(service.findDisputeUpdateRequestByDisputeIdAndStatus(null, DisputeUpdateRequestStatus.PENDING)).thenReturn(results);
		controllerResponse = disputeController.getDisputeUpdateRequests(null, DisputeUpdateRequestStatus.PENDING);
		resultList = controllerResponse.getBody();
		assertNotNull(resultList);
		assertEquals(resultList.size(),2);
		assertEquals(resultList.get(0).getStatus(), DisputeUpdateRequestStatus.PENDING);

		// Get all update requests for given dispute id
		results.clear();
		results.add(disputeUpdateRequest1);
		results.add(disputeUpdateRequest2);
		Mockito.when(service.findDisputeUpdateRequestByDisputeIdAndStatus(dispute1.getDisputeId(), null)).thenReturn(results);
		controllerResponse = disputeController.getDisputeUpdateRequests(dispute1.getDisputeId(), null);
		resultList = controllerResponse.getBody();
		assertNotNull(resultList);
		assertEquals(resultList.size(),2);
		assertEquals(resultList.get(0).getDisputeId(), dispute1.getDisputeId());
	}

	@Test
	public void testUpdateDisputeUpdateRequest_200() throws Exception {
		// Setup - create a new random Dispute and DisutantUpdateRequest to update
		Dispute dispute = RandomUtil.createDispute();
		DisputeUpdateRequest disputeUpdateRequest = RandomUtil.createDisputeUpdateRequest(dispute.getDisputeId(), DisputeUpdateRequestStatus.PENDING, DisputeUpdateRequestType.DISPUTANT_NAME);
		DisputeUpdateRequest updatedDisputeUpdateRequest = disputeUpdateRequest;
		updatedDisputeUpdateRequest.setStatus(DisputeUpdateRequestStatus.ACCEPTED);

		// Mock underlying updateDisputeUpdateRequest service
		Mockito.when(service.updateDisputeUpdateRequest(disputeUpdateRequest.getDisputeUpdateRequestId(), DisputeUpdateRequestStatus.ACCEPTED)).thenReturn(updatedDisputeUpdateRequest);

		// issue a PUT request to update the record to ACCEPTED, expect 200
		ResponseEntity<DisputeUpdateRequest> controllerResponse = disputeController.updateDisputeUpdateRequestStatus(disputeUpdateRequest.getDisputeUpdateRequestId(), DisputeUpdateRequestStatus.ACCEPTED);
		DisputeUpdateRequest result = controllerResponse.getBody();
		assertNotNull(result);
		assertEquals(disputeUpdateRequest.getDisputeUpdateRequestId(), result.getDisputeUpdateRequestId());
		assertEquals(DisputeUpdateRequestStatus.ACCEPTED, result.getStatus());
		assertEquals(controllerResponse.getStatusCode().value(), HttpStatus.OK.value());
	}
	
	@Test 
	public void testDeleteViolationTicketCount_200() throws Exception { 
	    Long id = 1L;
	    // Assume that the service will successfully delete the ViolationTicketCount
	    doNothing().when(service).deleteViolationTicketCount(anyLong());
	  
	    // Perform the DELETE request
	    mvc.perform(MockMvcRequestBuilders
	            .delete("/api/v1.0/violationTicketCount/{id}", id)
	            .principal(getPrincipal()))
	            .andExpect(status().isOk());
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
	private Dispute getDispute(Long disputeId, boolean isAssign) throws Exception {
		ResultActions resultActions = mvc.perform(MockMvcRequestBuilders
				.get("/api/v1.0/dispute/{id}/{isAssign}", disputeId, isAssign)
				.principal(getPrincipal()))
				.andExpect(status().isOk());
		Dispute result = mapResult(resultActions, new TypeReference<Dispute>() {});
		return result;
	}

	/**
	 * Issues a GET request to /api/v1.0/disputes. The appropriate controller is automatically called by the DispatchServlet
	 * @throws Exception
	 */
	private List<DisputeListItem> getAllDisputes(Date newerThan, DisputeStatus excludeStatus) throws Exception {
		ResultActions resultActions = mvc.perform(MockMvcRequestBuilders
				.get("/api/v1.0/disputes")
				.param("newerThan", newerThan == null ? null : DateFormatUtils.format(newerThan, "yyyy-MM-dd"))
				.param("excludeStatus", excludeStatus == null ? null : excludeStatus.name())
				.principal(getPrincipal()))
				.andExpect(status().isOk());
		List<DisputeListItem> result = mapResult(resultActions, new TypeReference<List<DisputeListItem>>() {});
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
	private ResultActions cancelDispute(Long disputeId, String cancelledReason) throws Exception {
		ResultActions resultActions = mvc.perform(MockMvcRequestBuilders
				.put("/api/v1.0/dispute/{id}/cancel", disputeId)
				.content(cancelledReason)
				.principal(getPrincipal()));
		return resultActions;
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

}
