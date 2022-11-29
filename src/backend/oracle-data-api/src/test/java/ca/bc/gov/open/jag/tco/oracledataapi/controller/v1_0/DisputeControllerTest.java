package ca.bc.gov.open.jag.tco.oracledataapi.controller.v1_0;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

import java.util.Date;
import java.util.List;
import java.util.UUID;

import javax.transaction.Transactional;

import org.apache.commons.lang3.RandomStringUtils;
import org.apache.commons.lang3.time.DateFormatUtils;
import org.apache.commons.lang3.time.DateUtils;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvSource;
import org.junit.jupiter.params.provider.EnumSource;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.test.web.servlet.ResultActions;
import org.springframework.test.web.servlet.request.MockMvcRequestBuilders;
import org.springframework.web.util.UriComponentsBuilder;

import com.fasterxml.jackson.core.type.TypeReference;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeContactInformation;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeResult;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
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

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "NEW", "VALIDATED", "PROCESSING" })
	public void testUpdateDisputeContact(DisputeStatus disputeStatus) throws Exception {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		dispute.setStatus(disputeStatus);
		dispute.setDisputantSurname("Parker");
		dispute.setDisputantGivenName1("Peter");
		dispute.setDisputantGivenName2("Pete");
		dispute.setDisputantGivenName3("P");
		dispute.setAddressLine1("123 Main St.");
		dispute.setAddressLine2("Appt 3");
		dispute.setAddressLine3("Room 102");
		dispute.setAddressCity("Star City");
		dispute.setAddressProvince("ALB");
		dispute.setPostalCode("V9A8W3");
		dispute.setEmailAddress("someone1@somewhere.com");
		dispute.setHomePhoneNumber("2501112222");
		dispute.setNoticeOfDisputeGuid(UUID.randomUUID().toString());
		saveDispute(dispute);

		DisputeContactInformation patch = new DisputeContactInformation();
		patch.setDisputantSurname("Spiderman");
		patch.setDisputantGivenName1("Mr");
		patch.setDisputantGivenName2("Spidy");
		patch.setDisputantGivenName3("Creapy Crawly");
		patch.setAddressLine1("124 Main St.");
		patch.setAddressLine2("Appt 5");
		patch.setAddressLine3("Room 203");
		patch.setAddressCity("Starling City");
		patch.setAddressProvince("BC");
		patch.setPostalCode("V9A7W4");
		patch.setEmailAddress("someone2@somewhere.com");
		patch.setHomePhoneNumber("2505556666");

		ResultActions resultActions = mvc.perform(MockMvcRequestBuilders
				.put("/api/v1.0/dispute/noticeOfDispute/{guid}/contact", dispute.getNoticeOfDisputeGuid())
				.content(asJsonString(patch))
				.contentType(MediaType.APPLICATION_JSON)
				.accept(MediaType.APPLICATION_JSON))
				.andExpect(status().isOk());
		Dispute result = mapResult(resultActions, new TypeReference<Dispute>() {});

		assertEquals("Spiderman", result.getDisputantSurname());
		assertEquals("Mr", result.getDisputantGivenName1());
		assertEquals("Spidy", result.getDisputantGivenName2());
		assertEquals("Creapy Crawly", result.getDisputantGivenName3());
		assertEquals("124 Main St.", result.getAddressLine1());
		assertEquals("Appt 5", result.getAddressLine2());
		assertEquals("Room 203", result.getAddressLine3());
		assertEquals("Starling City", result.getAddressCity());
		assertEquals("BC", result.getAddressProvince());
		assertEquals("V9A7W4", result.getPostalCode());
		assertEquals("someone2@somewhere.com", result.getEmailAddress());
		assertEquals("2505556666", result.getHomePhoneNumber());
	}

	@Test
	public void testUpdateDisputeContact_400() throws Exception {
		mvc.perform(MockMvcRequestBuilders
				.put("/api/v1.0/dispute/noticeOfDispute/{guid}/contact", "an-unknown-guid-id")
				.content(asJsonString(new DisputeContactInformation()))
				.contentType(MediaType.APPLICATION_JSON)
				.accept(MediaType.APPLICATION_JSON))
				.andExpect(status().isNotFound());
	}

	@ParameterizedTest
	@EnumSource(value = DisputeStatus.class, names = { "CANCELLED", "REJECTED" })
	public void testUpdateDisputeContact_405(DisputeStatus disputeStatus) throws Exception {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		dispute.setStatus(disputeStatus);
		dispute.setDisputantSurname("Parker");
		dispute.setDisputantGivenName1("Peter");
		dispute.setDisputantGivenName2("Pete");
		dispute.setDisputantGivenName3("P");
		dispute.setAddressLine1("123 Main St.");
		dispute.setAddressLine2("Appt 3");
		dispute.setAddressLine3("Room 102");
		dispute.setAddressCity("Star City");
		dispute.setAddressProvince("ALB");
		dispute.setPostalCode("V9A8W3");
		dispute.setEmailAddress("someone1@somewhere.com");
		dispute.setHomePhoneNumber("2501112222");
		dispute.setNoticeOfDisputeGuid(UUID.randomUUID().toString());
		saveDispute(dispute);

		DisputeContactInformation patch = new DisputeContactInformation();
		patch.setDisputantSurname("Spiderman");
		patch.setDisputantGivenName1("Mr");
		patch.setDisputantGivenName2("Spidy");
		patch.setDisputantGivenName3("Creapy Crawly");
		patch.setAddressLine1("124 Main St.");
		patch.setAddressLine2("Appt 5");
		patch.setAddressLine3("Room 203");
		patch.setAddressCity("Starling City");
		patch.setAddressProvince("BC");
		patch.setPostalCode("V9A7W4");
		patch.setEmailAddress("someone2@somewhere.com");
		patch.setHomePhoneNumber("2505556666");

		mvc.perform(MockMvcRequestBuilders
				.put("/api/v1.0/dispute/noticeOfDispute/{guid}/contact", dispute.getNoticeOfDisputeGuid())
				.content(asJsonString(patch))
				.contentType(MediaType.APPLICATION_JSON)
				.accept(MediaType.APPLICATION_JSON))
				.andExpect(status().isMethodNotAllowed());
	}

	@Test
	public void testUpdateDisputeContact_500() throws Exception {
		// Create a single Dispute
		Dispute dispute = RandomUtil.createDispute();
		dispute.setStatus(null);
		dispute.setDisputantSurname("Parker");
		dispute.setDisputantGivenName1("Peter");
		dispute.setDisputantGivenName2("Pete");
		dispute.setDisputantGivenName3("P");
		dispute.setAddressLine1("123 Main St.");
		dispute.setAddressLine2("Appt 3");
		dispute.setAddressLine3("Room 102");
		dispute.setAddressCity("Star City");
		dispute.setAddressProvince("ALB");
		dispute.setPostalCode("V9A8W3");
		dispute.setEmailAddress("someone1@somewhere.com");
		dispute.setHomePhoneNumber("2501112222");
		dispute.setNoticeOfDisputeGuid(UUID.randomUUID().toString());
		saveDispute(dispute);

		mvc.perform(MockMvcRequestBuilders
				.put("/api/v1.0/dispute/noticeOfDispute/{guid}/contact", dispute.getNoticeOfDisputeGuid())
				.content(asJsonString(new DisputeContactInformation()))
				.contentType(MediaType.APPLICATION_JSON)
				.accept(MediaType.APPLICATION_JSON))
				.andExpect(status().isInternalServerError());
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
		dispute.setIssuedTs(DateUtils.parseDate("14:54", "HH:mm"));
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
		"AB12345678,123abc", // should be of the format HH:mm
		"AB12345678,55:99",  // invalid time
		"AB123456789,14:23", // too long
		"ABC1234567,14:23",  // invalid regex
		",14:23",            // invalid ticketNumber
		"ABC1234567,",       // invalid time
		})
	public void testFindByTicketNumberAndTime_Invalid(String ticketNumber, String issuedTime) throws Exception {
		mvc.perform(MockMvcRequestBuilders
				.get("/api/v1.0/dispute/status")
				.param("ticketNumber", ticketNumber)
				.param("issuedTime", issuedTime))
				.andExpect(status().isBadRequest());
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

	/** Issues a GET request to /api/v1.0/dispute with the required ticketNumber and time to find a Dispute. */
	private List<DisputeResult> findDispute(String ticketNumber, String issuedTime) {
		UriComponentsBuilder uriBuilder = fromUriString("/dispute/status")
				.queryParam("ticketNumber", ticketNumber)
				.queryParam("issuedTime", issuedTime);
		ResponseEntity<List<DisputeResult>> results = getForEntity(uriBuilder, new ParameterizedTypeReference<List<DisputeResult>>() {});
		return results.getBody();
	}

}
