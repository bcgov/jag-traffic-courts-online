package ca.bc.gov.open.jag.tco.oracledataapi.controller.v1_0;

import static org.junit.jupiter.api.Assertions.assertEquals;

import java.util.List;

import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Qualifier;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.model.EmailHistory;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.EmailHistoryRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;

class EmailHistoryControllerTest extends BaseTestSuite {

	@Autowired
	@Qualifier("EmailHistoryControllerV1_0")
	private EmailHistoryController emailHistoryController;

	@Autowired
	private EmailHistoryRepository emailHistoryRepository;

	@Test
	public void testSaveEmailHistory() {
		// Assert db is empty and clean
		List<EmailHistory> allEmailHistory = emailHistoryRepository.findAll();
		assertEquals(0, allEmailHistory.size());

		// Create a single EmailHistory record
		EmailHistory emailHistory = RandomUtil.createEmailHistory();
		Long emailHistoryId = emailHistoryController.insertEmailHistory(emailHistory).getBody();

		// Assert db contains the single created record
		allEmailHistory = emailHistoryRepository.findAll();
		assertEquals(1, allEmailHistory.size());
		assertEquals(emailHistoryId, allEmailHistory.get(0).getEmailHistoryId());
		assertEquals(emailHistory.getToEmailAddress(), allEmailHistory.get(0).getToEmailAddress());

		// Delete record
		emailHistoryRepository.deleteById(emailHistoryId);

		// Assert db is empty again
		allEmailHistory = emailHistoryRepository.findAll();
		assertEquals(0, allEmailHistory.size());
	}
}
