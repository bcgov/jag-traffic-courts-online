package ca.bc.gov.open.jag.tco.oracledataapi.service.impl.ords;

import static org.junit.jupiter.api.Assertions.assertDoesNotThrow;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.mockito.ArgumentMatchers.any;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.NoSuchElementException;

import javax.ws.rs.InternalServerErrorException;

import org.apache.commons.lang3.time.DateUtils;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.Mock;
import org.mockito.Mockito;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.api.ViolationTicketApi;
import ca.bc.gov.open.jag.tco.oracledataapi.api.handler.ApiException;
import ca.bc.gov.open.jag.tco.oracledataapi.api.model.ResponseResult;
import ca.bc.gov.open.jag.tco.oracledataapi.api.model.ViolationTicket;
import ca.bc.gov.open.jag.tco.oracledataapi.api.model.ViolationTicketListResponse;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeResult;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.ords.DisputeRepositoryImpl;


class DisputeServiceImplTest extends BaseTestSuite {

	@Mock
	private ViolationTicketApi violationTicketApi;
	private DisputeRepositoryImpl repository;

	@Override
	@BeforeEach
	public void beforeEach() {
		repository = new DisputeRepositoryImpl(violationTicketApi);
	}

	@Test
	public void testAssignExpect200() throws Exception {
		Long disputeId = Long.valueOf(1);
		String userName = "testUser";
		ResponseResult response = new ResponseResult();
		response.setStatus("1");

		Mockito.when(violationTicketApi.v1AssignViolationTicketPost(disputeId, userName)).thenReturn(response);

		assertDoesNotThrow(() -> {
			repository.assignDisputeToUser(disputeId, userName);
		});
	}

	@Test
	public void testAssignExpect500_ApiException() throws Exception {
		Long disputeId = Long.valueOf(1);
		String userName = "testUser";

		Mockito.when(violationTicketApi.v1AssignViolationTicketPost(disputeId, userName)).thenThrow(ApiException.class);

		assertThrows(ApiException.class, () -> {
			repository.assignDisputeToUser(disputeId, userName);
		});
	}

	@Test
	public void testAssignExpect500_noErrorMsg() throws Exception {
		Long disputeId = Long.valueOf(1);
		String userName = "testUser";
		ResponseResult response = new ResponseResult();
		response.setStatus("0"); // 0 status (meaning failure)

		Mockito.when(violationTicketApi.v1AssignViolationTicketPost(disputeId, userName)).thenReturn(response);

		assertThrows(InternalServerErrorException.class, () -> {
			repository.assignDisputeToUser(disputeId, userName);
		});
	}

	@Test
	public void testAssignExpect500_nullResponse() throws Exception {
		Long disputeId = Long.valueOf(1);
		String userName = "testUser";

		Mockito.when(violationTicketApi.v1AssignViolationTicketPost(disputeId, userName)).thenReturn(null);

		assertThrows(InternalServerErrorException.class, () -> {
			repository.assignDisputeToUser(disputeId, userName);
		});
	}

	@Test
	public void testAssignExpect500_withErrorMsg() throws Exception {
		Long disputeId = Long.valueOf(1);
		String userName = "testUser";
		ResponseResult response = new ResponseResult();
		response.setStatus("0"); // 0 status (meaning failure)
		response.setException("some failure cause");

		Mockito.when(violationTicketApi.v1AssignViolationTicketPost(disputeId, userName)).thenReturn(response);

		assertThrows(InternalServerErrorException.class, () -> {
			repository.assignDisputeToUser(disputeId, userName);
		});
	}

	@Test
	public void testDeleteExpect200() throws ApiException {
		Long disputeId = Long.valueOf(1);
		ResponseResult response = new ResponseResult();
		response.setStatus("1");

		Mockito.when(violationTicketApi.v1DeleteViolationTicketDelete(disputeId)).thenReturn(response);

		assertDoesNotThrow(() -> {
			repository.deleteById(disputeId);
		});
	}

	@Test
	public void testDeleteExpect400() throws ApiException {
		Long disputeId = null;

		assertThrows(IllegalArgumentException.class, () -> {
			repository.deleteById(disputeId);
		});
	}

	@Test
	public void testDeleteExpect404() throws ApiException {
		Long disputeId = Long.valueOf(1);
		ResponseResult response = new ResponseResult();
		response.setStatus("0");
		response.setException("ORA-01403: no data found");

		Mockito.when(violationTicketApi.v1DeleteViolationTicketDelete(disputeId)).thenReturn(response);

		assertThrows(NoSuchElementException.class, () -> {
			repository.deleteById(disputeId);
		});
	}

	@Test
	public void testDeleteExpect500_ApiException() throws Exception {
		Long disputeId = Long.valueOf(1);

		Mockito.when(violationTicketApi.v1DeleteViolationTicketDelete(disputeId)).thenThrow(ApiException.class);

		assertThrows(ApiException.class, () -> {
			repository.deleteById(disputeId);
		});
	}

	@Test
	public void testDeleteExpect500_noErrorMessage() throws ApiException {
		Long disputeId = Long.valueOf(1);
		ResponseResult response = new ResponseResult();
		response.setStatus("0"); // 0 status (meaning failure) and no message

		Mockito.when(violationTicketApi.v1DeleteViolationTicketDelete(disputeId)).thenReturn(response);

		assertThrows(InternalServerErrorException.class, () -> {
			repository.deleteById(disputeId);
		});
	}

	@Test
	public void testDeleteExpect500_nullReponse() throws ApiException {
		Long disputeId = Long.valueOf(1);

		Mockito.when(violationTicketApi.v1DeleteViolationTicketDelete(disputeId)).thenReturn(null);

		assertThrows(InternalServerErrorException.class, () -> {
			repository.deleteById(disputeId);
		});
	}

	@Test
	public void testDeleteExpect500_withErrorMessage() throws ApiException {
		Long disputeId = Long.valueOf(1);
		ResponseResult response = new ResponseResult();
		response.setStatus("0"); // 0 status (meaning failure) and no message
		response.setException("some failure cause");

		Mockito.when(violationTicketApi.v1DeleteViolationTicketDelete(disputeId)).thenReturn(response);

		assertThrows(InternalServerErrorException.class, () -> {
			repository.deleteById(disputeId);
		});
	}

	@Test
	public void testSetStatusExpect200() throws Exception {
		Long disputeId = Long.valueOf(1);
		DisputeStatus disputeStatus = DisputeStatus.NEW;
		String rejectedReason = "just because";
		ResponseResult response = new ResponseResult();
		response.setStatus("1");

		Mockito.when(violationTicketApi.v1ViolationTicketStatusPost(disputeId, disputeStatus.toShortName(), rejectedReason)).thenReturn(response);

		assertDoesNotThrow(() -> {
			repository.setStatus(disputeId, disputeStatus, rejectedReason);
		});
	}

	@Test
	public void testSetStatusExpect500_ApiException() throws Exception {
		Long disputeId = Long.valueOf(1);
		DisputeStatus disputeStatus = DisputeStatus.NEW;
		String rejectedReason = "just because";

		Mockito.when(violationTicketApi.v1ViolationTicketStatusPost(disputeId, disputeStatus.toShortName(), rejectedReason)).thenThrow(ApiException.class);

		assertThrows(ApiException.class, () -> {
			repository.setStatus(disputeId, disputeStatus, rejectedReason);
		});
	}

	@Test
	public void testSetStatusExpect500_noErrorMsg() throws Exception {
		Long disputeId = Long.valueOf(1);
		DisputeStatus disputeStatus = DisputeStatus.NEW;
		String rejectedReason = "just because";
		ResponseResult response = new ResponseResult();
		response.setStatus("0"); // 0 status (meaning failure) and no message

		Mockito.when(violationTicketApi.v1ViolationTicketStatusPost(disputeId, disputeStatus.toShortName(), rejectedReason)).thenReturn(response);

		assertThrows(InternalServerErrorException.class, () -> {
			repository.setStatus(disputeId, disputeStatus, rejectedReason);
		});
	}

	@Test
	public void testSetStatusExpect500_nullReponse() throws Exception {
		Long disputeId = Long.valueOf(1);
		DisputeStatus disputeStatus = DisputeStatus.NEW;
		String rejectedReason = "just because";

		Mockito.when(violationTicketApi.v1ViolationTicketStatusPost(disputeId, disputeStatus.toShortName(), rejectedReason)).thenReturn(null);

		assertThrows(InternalServerErrorException.class, () -> {
			repository.setStatus(disputeId, disputeStatus, rejectedReason);
		});
	}

	@Test
	public void testSetStatusExpect500_withErrorMsg() throws Exception {
		Long disputeId = Long.valueOf(1);
		DisputeStatus disputeStatus = DisputeStatus.NEW;
		String rejectedReason = "just because";
		ResponseResult response = new ResponseResult();
		response.setStatus("0"); // 0 status (meaning failure)
		response.setException("some failure cause");

		Mockito.when(violationTicketApi.v1ViolationTicketStatusPost(disputeId, disputeStatus.toShortName(), rejectedReason)).thenReturn(response);

		assertThrows(InternalServerErrorException.class, () -> {
			repository.setStatus(disputeId, disputeStatus, rejectedReason);
		});
	}

	@Test
	public void testUnassignExpect200() throws Exception {
		Date now = new Date();
		String assignedBeforeTs = dateToString(now, DisputeRepositoryImpl.DATE_TIME_FORMAT);
		ResponseResult response = new ResponseResult();
		response.setStatus("1");

		Mockito.when(violationTicketApi.v1UnassignViolationTicketPost(assignedBeforeTs)).thenReturn(response);

		assertDoesNotThrow(() -> {
			repository.unassignDisputes(now);
		});
	}

	@Test
	public void testUnassignExpect500_ApiException() throws Exception {
		Date now = new Date();
		String assignedBeforeTs = dateToString(now, DisputeRepositoryImpl.DATE_TIME_FORMAT);

		Mockito.when(violationTicketApi.v1UnassignViolationTicketPost(assignedBeforeTs)).thenThrow(ApiException.class);

		assertThrows(ApiException.class, () -> {
			repository.unassignDisputes(now);
		});
	}

	@Test
	public void testUnassignExpect500_noErrorMsg() throws Exception {
		Date now = new Date();
		String assignedBeforeTs = dateToString(now, DisputeRepositoryImpl.DATE_TIME_FORMAT);
		ResponseResult response = new ResponseResult();
		response.setStatus("0"); // 0 status (meaning failure) and no message

		Mockito.when(violationTicketApi.v1UnassignViolationTicketPost(assignedBeforeTs)).thenReturn(response);

		assertThrows(InternalServerErrorException.class, () -> {
			repository.unassignDisputes(now);
		});
	}

	@Test
	public void testUnassignExpect500_nullReponse() throws Exception {
		Date now = new Date();
		String assignedBeforeTs = dateToString(now, DisputeRepositoryImpl.DATE_TIME_FORMAT);

		Mockito.when(violationTicketApi.v1UnassignViolationTicketPost(assignedBeforeTs)).thenReturn(null);

		assertThrows(InternalServerErrorException.class, () -> {
			repository.unassignDisputes(now);
		});
	}

	@Test
	public void testUnassignExpect500_withErrorMsg() throws Exception {
		Date now = new Date();
		String assignedBeforeTs = dateToString(now, DisputeRepositoryImpl.DATE_TIME_FORMAT);
		ResponseResult response = new ResponseResult();
		response.setStatus("0"); // 0 status (meaning failure)
		response.setException("some failure cause");

		Mockito.when(violationTicketApi.v1UnassignViolationTicketPost(assignedBeforeTs)).thenReturn(response);

		assertThrows(InternalServerErrorException.class, () -> {
			repository.unassignDisputes(now);
		});
	}

	@Test
	public void testFindByTicketNumberAndTime200() throws Exception {
		Long disputeId = Long.valueOf(1001L);
		DisputeStatus disputeStatus = DisputeStatus.PROCESSING;
		String ticketNumber = "AX00000000";
		String issuedDateStr = "13:54";
		Date issuedDate = DateUtils.parseDate(issuedDateStr, "HH:mm");

		ca.bc.gov.open.jag.tco.oracledataapi.api.model.Dispute dispute = new ca.bc.gov.open.jag.tco.oracledataapi.api.model.Dispute();
		dispute.setDisputeId(disputeId.toString());
		dispute.setDisputeStatusTypeCd(disputeStatus.toShortName());

		ViolationTicket violationTicket = new ViolationTicket();
		violationTicket.setDispute(dispute);
		violationTicket.setTicketNumberTxt(ticketNumber);
		violationTicket.setIssuedDt(new java.util.Date(issuedDate.getTime()));

		ViolationTicketListResponse validResponse = new ViolationTicketListResponse();
		validResponse.setViolationTickets(new ArrayList<ViolationTicket>());
		validResponse.getViolationTickets().add(violationTicket);

		ViolationTicketListResponse emptyResponse = new ViolationTicketListResponse();
		emptyResponse.setViolationTickets(new ArrayList<ViolationTicket>());

		Mockito.when(violationTicketApi.v1ViolationTicketListGet(null, null, ticketNumber, null, issuedDateStr)).thenReturn(validResponse);

		List<DisputeResult> disputeResults = repository.findByTicketNumberAndTime(ticketNumber, issuedDate);
		assertEquals(1, disputeResults.size());
		assertEquals(disputeId, disputeResults.get(0).getDisputeId());
		assertEquals(disputeStatus, disputeResults.get(0).getDisputeStatus());

		disputeResults = repository.findByTicketNumberAndTime("AX", issuedDate);
		assertEquals(0, disputeResults.size());

		disputeResults = repository.findByTicketNumberAndTime(ticketNumber, DateUtils.parseDate("13:55", "HH:mm"));
		assertEquals(0, disputeResults.size());
	}

	@Test
	public void testFindByTicketNumberAndTime500() throws Exception {
		Mockito.when(violationTicketApi.v1ViolationTicketListGet(any(), any(), any(), any(), any())).thenThrow(ApiException.class);

		assertThrows(ApiException.class, () -> {
			repository.findByTicketNumberAndTime("AX00000000", new Date());
		});
	}

	@Test
	public void testGetAllDisputesExpect200() throws Exception {
		Date now = new Date();
		String olderThanDate = dateToString(now, DisputeRepositoryImpl.DATE_FORMAT);
		DisputeStatus excludedStatus = DisputeStatus.CANCELLED;
		String noticeOfDisputeGuid = java.util.UUID.randomUUID().toString();
		ViolationTicketListResponse response = new ViolationTicketListResponse();
		List<ViolationTicket> violationTickets = new ArrayList<ViolationTicket>();
		response.setViolationTickets(violationTickets);

		Mockito.when(violationTicketApi.v1ViolationTicketListGet(olderThanDate, excludedStatus.toShortName(), null, noticeOfDisputeGuid, null)).thenReturn(response);

		assertDoesNotThrow(() -> {
			repository.findByStatusNotAndCreatedTsBeforeAndNoticeOfDisputeGuid(excludedStatus, now, noticeOfDisputeGuid);
		});
	}

	@Test
	public void testGetAllDisputesExpect500_InternalServerError() throws Exception {
		Date now = new Date();
		String olderThanDate = dateToString(now, DisputeRepositoryImpl.DATE_FORMAT);
		DisputeStatus excludedStatus = DisputeStatus.CANCELLED;
		String noticeOfDisputeGuid = java.util.UUID.randomUUID().toString();
		ViolationTicketListResponse response = new ViolationTicketListResponse();
		List<ViolationTicket> violationTickets = new ArrayList<ViolationTicket>();
		response.setViolationTickets(violationTickets);

		Mockito.when(violationTicketApi.v1ViolationTicketListGet(olderThanDate, excludedStatus.toShortName(), null, noticeOfDisputeGuid, null)).thenThrow(ApiException.class);

		assertThrows(InternalServerErrorException.class, () -> {
			repository.findByStatusNotAndCreatedTsBeforeAndNoticeOfDisputeGuid(excludedStatus, now, noticeOfDisputeGuid);
		});
	}

	@Test
	public void testUpdateDisputeExpect200() throws Exception {
		Long disputeId = Long.valueOf(1);
		ResponseResult response = new ResponseResult();
		response.setStatus("1");

		Dispute disputeToUpdate = new Dispute();
		disputeToUpdate.setDisputeId(disputeId);
		disputeToUpdate.setStatus(DisputeStatus.PROCESSING);

		Mockito.when(violationTicketApi.v1UpdateViolationTicketPut(any())).thenReturn(response);

		assertDoesNotThrow(() -> {
			repository.update(disputeToUpdate);
		});
	}

	@Test
	public void testUpdateDisputeExpect500_InternalServerError() throws Exception {
		Long disputeId = Long.valueOf(1);

		Dispute disputeToUpdate = new Dispute();
		disputeToUpdate.setDisputeId(disputeId);
		disputeToUpdate.setStatus(DisputeStatus.PROCESSING);

		Mockito.when(violationTicketApi.v1UpdateViolationTicketPut(any())).thenThrow(ApiException.class);

		assertThrows(InternalServerErrorException.class, () -> {
			repository.update(disputeToUpdate);
		});
	}

	@Test
	public void testUpdateDisputeExpect500_noErrorMsg() throws Exception {
		Long disputeId = Long.valueOf(1);
		ResponseResult response = new ResponseResult();
		response.setStatus("0"); // 0 status (meaning failure) and no message

		Dispute disputeToUpdate = new Dispute();
		disputeToUpdate.setDisputeId(disputeId);
		disputeToUpdate.setStatus(DisputeStatus.PROCESSING);

		Mockito.when(violationTicketApi.v1UpdateViolationTicketPut(any())).thenReturn(response);

		assertThrows(InternalServerErrorException.class, () -> {
			repository.update(disputeToUpdate);
		});
	}

	@Test
	public void testUpdateDisputeExpect500_nullResponse() throws Exception {
		Long disputeId = Long.valueOf(1);
		Dispute disputeToUpdate = new Dispute();
		disputeToUpdate.setDisputeId(disputeId);
		disputeToUpdate.setStatus(DisputeStatus.PROCESSING);

		Mockito.when(violationTicketApi.v1UpdateViolationTicketPut(any())).thenReturn(null);

		assertThrows(InternalServerErrorException.class, () -> {
			repository.update(disputeToUpdate);
		});
	}

	@Test
	public void testUpdateDisputeExpect500_withErrorMsg() throws Exception {
		Long disputeId = Long.valueOf(1);
		ResponseResult response = new ResponseResult();
		response.setStatus("0"); // 0 status (meaning failure)
		response.setException("some failure cause");

		Dispute disputeToUpdate = new Dispute();
		disputeToUpdate.setDisputeId(disputeId);
		disputeToUpdate.setStatus(DisputeStatus.PROCESSING);

		Mockito.when(violationTicketApi.v1UpdateViolationTicketPut(any())).thenReturn(response);

		assertThrows(InternalServerErrorException.class, () -> {
			repository.update(disputeToUpdate);
		});
	}

	private String dateToString(Date date, String format) {
		SimpleDateFormat dateFormat = new SimpleDateFormat(format);
		return dateFormat.format(date);
	}

}
