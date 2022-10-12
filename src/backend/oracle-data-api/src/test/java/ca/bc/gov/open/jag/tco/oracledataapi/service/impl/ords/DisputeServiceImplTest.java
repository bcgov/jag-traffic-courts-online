package ca.bc.gov.open.jag.tco.oracledataapi.service.impl.ords;

import static org.junit.jupiter.api.Assertions.assertDoesNotThrow;
import static org.junit.jupiter.api.Assertions.assertThrows;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.NoSuchElementException;

import javax.ws.rs.InternalServerErrorException;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.Mock;
import org.mockito.Mockito;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.api.ViolationTicketApi;
import ca.bc.gov.open.jag.tco.oracledataapi.api.handler.ApiException;
import ca.bc.gov.open.jag.tco.oracledataapi.api.model.ResponseResult;
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
		String assignedBeforeTs = dateToString(now);
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
		String assignedBeforeTs = dateToString(now);

		Mockito.when(violationTicketApi.v1UnassignViolationTicketPost(assignedBeforeTs)).thenThrow(ApiException.class);

		assertThrows(ApiException.class, () -> {
			repository.unassignDisputes(now);
		});
	}

	@Test
	public void testUnassignExpect500_noErrorMsg() throws Exception {
		Date now = new Date();
		String assignedBeforeTs = dateToString(now);
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
		String assignedBeforeTs = dateToString(now);

		Mockito.when(violationTicketApi.v1UnassignViolationTicketPost(assignedBeforeTs)).thenReturn(null);

		assertThrows(InternalServerErrorException.class, () -> {
			repository.unassignDisputes(now);
		});
	}

	@Test
	public void testUnassignExpect500_withErrorMsg() throws Exception {
		Date now = new Date();
		String assignedBeforeTs = dateToString(now);
		ResponseResult response = new ResponseResult();
		response.setStatus("0"); // 0 status (meaning failure)
		response.setException("some failure cause");

		Mockito.when(violationTicketApi.v1UnassignViolationTicketPost(assignedBeforeTs)).thenReturn(response);

		assertThrows(InternalServerErrorException.class, () -> {
			repository.unassignDisputes(now);
		});
	}

	private String dateToString(Date date) {
		SimpleDateFormat dateFormat = new SimpleDateFormat(DisputeRepositoryImpl.DATE_TIME_FORMAT);
		return dateFormat.format(date);
	}

}
