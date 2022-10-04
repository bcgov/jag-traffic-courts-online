package ca.bc.gov.open.jag.tco.oracledataapi.service.impl.ords;

import static org.junit.jupiter.api.Assertions.assertDoesNotThrow;
import static org.junit.jupiter.api.Assertions.assertThrows;

import java.util.NoSuchElementException;

import javax.ws.rs.InternalServerErrorException;

import org.junit.jupiter.api.Test;
import org.mockito.Mock;
import org.mockito.Mockito;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.api.ViolationTicketApi;
import ca.bc.gov.open.jag.tco.oracledataapi.api.handler.ApiException;
import ca.bc.gov.open.jag.tco.oracledataapi.api.model.DeleteResult;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.ords.DisputeRepositoryImpl;


class DisputeServiceImplTest extends BaseTestSuite {

	@Mock
	private ViolationTicketApi violationTicketApi;

	@Test
	public void testExpect200() throws ApiException {
		Long disputeId = Long.valueOf(1);
		DeleteResult _200 = new DeleteResult();
		_200.setStatus("1");

		Mockito.when(violationTicketApi.v1DeleteViolationTicketDelete(disputeId)).thenReturn(_200);
		DisputeRepositoryImpl repo = new DisputeRepositoryImpl(violationTicketApi);
		assertDoesNotThrow(() -> {
			repo.deleteById(disputeId);
		});
	}

	@Test
	public void testExpect400() throws ApiException {
		Long disputeId = null;

		DisputeRepositoryImpl repo = new DisputeRepositoryImpl(violationTicketApi);
		assertThrows(IllegalArgumentException.class, () -> {
			repo.deleteById(disputeId);
		});
	}

	@Test
	public void testExpect404() throws ApiException {
		Long disputeId = Long.valueOf(1);
		DeleteResult _404 = new DeleteResult();
		_404.setStatus("0");
		_404.setException("ORA-01403: no data found");

		Mockito.when(violationTicketApi.v1DeleteViolationTicketDelete(disputeId)).thenReturn(_404);
		DisputeRepositoryImpl repo = new DisputeRepositoryImpl(violationTicketApi);
		assertThrows(NoSuchElementException.class, () -> {
			repo.deleteById(disputeId);
		});
	}

	@Test
	public void testExpect500_noErrorMessage() throws ApiException {
		Long disputeId = Long.valueOf(1);
		DeleteResult _500 = new DeleteResult();
		_500.setStatus("0"); // 0 status (meaning failure) and no message

		Mockito.when(violationTicketApi.v1DeleteViolationTicketDelete(disputeId)).thenReturn(_500);
		DisputeRepositoryImpl repo = new DisputeRepositoryImpl(violationTicketApi);
		assertThrows(InternalServerErrorException.class, () -> {
			repo.deleteById(disputeId);
		});
	}

	@Test
	public void testExpect500_withErrorMessage() throws ApiException {
		Long disputeId = Long.valueOf(1);
		DeleteResult _500 = new DeleteResult();
		_500.setStatus("0"); // 0 status (meaning failure) and no message
		_500.setException("some failure cause");

		Mockito.when(violationTicketApi.v1DeleteViolationTicketDelete(disputeId)).thenReturn(_500);
		DisputeRepositoryImpl repo = new DisputeRepositoryImpl(violationTicketApi);
		assertThrows(InternalServerErrorException.class, () -> {
			repo.deleteById(disputeId);
		});
	}

}
