package ca.bc.gov.open.jag.tco.oracledataapi.controller.v1_0;

import static org.junit.jupiter.api.Assertions.assertEquals;

import java.util.List;

import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Qualifier;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.model.FileHistory;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.FileHistoryRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;

class FileHistoryControllerTest extends BaseTestSuite {

	@Autowired
	@Qualifier("FileHistoryControllerV1_0")
	private FileHistoryController fileHistoryController;

	@Autowired
	private FileHistoryRepository fileHistoryRepository;

	@Test
	public void testSaveFileHistory() {
		// Assert db is empty and clean
		List<FileHistory> allFileHistory = fileHistoryRepository.findAll();
		assertEquals(0, allFileHistory.size());

		// Create a single FileHistory record
		FileHistory fileHistory = RandomUtil.createFileHistory();
		Long fileHistoryId = fileHistoryController.insertFileHistory(fileHistory).getBody();

		// Assert db contains the single created record
		allFileHistory = fileHistoryRepository.findAll();
		assertEquals(1, allFileHistory.size());
		assertEquals(fileHistoryId, allFileHistory.get(0).getFileHistoryId());
		assertEquals(fileHistory.getDescription(), allFileHistory.get(0).getDescription());

		// Delete record
		fileHistoryRepository.deleteById(fileHistoryId);

		// Assert db is empty again
		allFileHistory = fileHistoryRepository.findAll();
		assertEquals(0, allFileHistory.size());
	}
}
