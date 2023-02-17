package ca.bc.gov.open.jag.tco.oracledataapi.controller.v1_0;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;

import org.junit.jupiter.api.Test;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.Mockito;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.model.EmailHistory;
import ca.bc.gov.open.jag.tco.oracledataapi.service.EmailHistoryService;
import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;

class EmailHistoryControllerTest extends BaseTestSuite {

	@InjectMocks
	private EmailHistoryController emailHistoryController;

	@Mock
	private EmailHistoryService service;

	@Test
	public void testSaveEmailHistory() {
		// Create a single EmailHistory record
		EmailHistory emailHistory = RandomUtil.createEmailHistory();
		// Mock underlying saveDisputeUpdateRequest service
		Mockito.when(service.insertEmailHistory(emailHistory)).thenReturn(emailHistory.getEmailHistoryId());

		ResponseEntity<Long> controllerResponse = emailHistoryController.insertEmailHistory(emailHistory);
		Mockito.verify(service).insertEmailHistory(emailHistory);
		Long resultId = controllerResponse.getBody();
		assertNotNull(resultId);
		assertEquals(controllerResponse.getStatusCode().value(), HttpStatus.OK.value());
	}
}
