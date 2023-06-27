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
import ca.bc.gov.open.jag.tco.oracledataapi.model.FileHistory;
import ca.bc.gov.open.jag.tco.oracledataapi.service.FileHistoryService;
import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;

class FileHistoryControllerTest extends BaseTestSuite {

	@InjectMocks
	private FileHistoryController fileHistoryController;

	@Mock
	private FileHistoryService service;

	@Test
	public void testSaveFileHistory_200() {
		// Create a single FileHistory record
		FileHistory fileHistory = RandomUtil.createFileHistory();
		// Mock underlying saveDisputeUpdateRequest service
		Mockito.when(service.insertFileHistory(fileHistory)).thenReturn(fileHistory.getFileHistoryId());

		// issue a POST request, expect 200
		ResponseEntity<Long> controllerResponse = fileHistoryController.insertFileHistory(fileHistory);
		Mockito.verify(service).insertFileHistory(fileHistory);
		Long resultId = controllerResponse.getBody();
		assertNotNull(resultId);
		assertEquals(controllerResponse.getStatusCode().value(), HttpStatus.OK.value());
	}
}
