package ca.bc.gov.open.jag.tco.oracledataapi.EmailHistory;

import ca.bc.gov.open.jag.tco.oracledataapi.controller.v1_0.EmailHistoryController;
import ca.bc.gov.open.jag.tco.oracledataapi.model.EmailHistory;
import ca.bc.gov.open.jag.tco.oracledataapi.service.EmailHistoryService;
import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.TestInstance;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.Mockito;
import org.mockito.MockitoAnnotations;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;

import java.util.ArrayList;
import java.util.List;
import java.util.NoSuchElementException;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;

@TestInstance(TestInstance.Lifecycle.PER_CLASS)
class EmailHistoryControllerTest {

	private EmailHistoryController sut;
	@Mock
	private EmailHistoryService serviceMock;

	@BeforeEach
	public void setUp() {
		MockitoAnnotations.openMocks(this);
		sut = new EmailHistoryController(serviceMock);
	}

	@Test
	public void getEmailHistoryByTicketNumber() {
		// Create a single EmailHistory record
		String ticketNumber = RandomUtil.randomTicketNumber();
		// Create a single EmailHistory record
		EmailHistory emailHistory = RandomUtil.createEmailHistory();
		// Create a single EmailHistory record
		List<EmailHistory> emailHistorys = new ArrayList<>();
		emailHistorys.add(emailHistory);
		// Mock underlying saveDisputeUpdateRequest service
		Mockito.when(serviceMock.getEmailHistoryByTicketNumber(ticketNumber)).thenReturn(emailHistorys);

		List<EmailHistory>  controllerResponse = sut.getEmailHistoryByTicketNumber(ticketNumber);
		Mockito.verify(serviceMock).getEmailHistoryByTicketNumber(ticketNumber);
		assertNotNull(controllerResponse);
		assertEquals(emailHistorys, controllerResponse);
	}

	@Test
	public void getEmailHistoryByTicketNumberReturn500() throws Exception {
		String ticketNumber = RandomUtil.randomTicketNumber();
		Mockito.when(serviceMock.getEmailHistoryByTicketNumber(ticketNumber)).thenThrow(RuntimeException.class);

		Assertions.assertThrows(RuntimeException.class, () -> {
			sut.getEmailHistoryByTicketNumber(ticketNumber);
		});
	}

	@Test
	public void getEmailHistoryByTicketNumberReturn404() throws Exception {
		String ticketNumber = RandomUtil.randomTicketNumber();
		Mockito.when(serviceMock.getEmailHistoryByTicketNumber(ticketNumber)).thenThrow(NoSuchElementException.class);

		Assertions.assertThrows(NoSuchElementException.class, () -> {
			sut.getEmailHistoryByTicketNumber(ticketNumber);
		});
	}

	@Test
	public void insertEmailHistoryTest() {
		// Create a single EmailHistory record
		EmailHistory emailHistory = RandomUtil.createEmailHistory();
		// Mock underlying saveDisputeUpdateRequest service
		Mockito.when(serviceMock.insertEmailHistory(emailHistory)).thenReturn(emailHistory.getEmailHistoryId());

		ResponseEntity<Long> controllerResponse = sut.insertEmailHistory(emailHistory);
		Mockito.verify(serviceMock).insertEmailHistory(emailHistory);
		Long resultId = controllerResponse.getBody();
		assertNotNull(resultId);
		assertEquals(controllerResponse.getStatusCode().value(), HttpStatus.OK.value());
	}

	@Test
	public void insertEmailHistoryRuturn400() throws Exception {
		// Create a single EmailHistory record
		EmailHistory emailHistory = new EmailHistory();
		// Mock underlying saveDisputeUpdateRequest service
		Mockito.when(serviceMock.insertEmailHistory(emailHistory)).thenThrow(NoSuchElementException.class);

		Assertions.assertThrows(IllegalArgumentException.class, () -> {
			sut.insertEmailHistory(emailHistory);
		});
	}

	@Test
	public void insertEmailHistoryReturn500() throws Exception {
		// Create a single EmailHistory record
		EmailHistory emailHistory = RandomUtil.createEmailHistory();
		// Mock underlying saveDisputeUpdateRequest service
		Mockito.when(serviceMock.insertEmailHistory(emailHistory)).thenThrow(RuntimeException.class);

		Assertions.assertThrows(RuntimeException.class, () -> {
			sut.insertEmailHistory(emailHistory);
		});
	}

	@Test
	public void insertEmailHistoryReturn404() throws Exception {
		// Create a single EmailHistory record
		EmailHistory emailHistory = RandomUtil.createEmailHistory();
		// Mock underlying saveDisputeUpdateRequest service
		Mockito.when(serviceMock.insertEmailHistory(emailHistory)).thenThrow(NoSuchElementException.class);

		Assertions.assertThrows(NoSuchElementException.class, () -> {
			sut.insertEmailHistory(emailHistory);
		});
	}
}
