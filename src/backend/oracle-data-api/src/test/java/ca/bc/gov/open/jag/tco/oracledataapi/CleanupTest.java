package ca.bc.gov.open.jag.tco.oracledataapi;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;

import java.util.Collections;
import java.util.Date;
import java.util.List;

import org.apache.commons.collections4.CollectionUtils;
import org.apache.commons.lang3.time.DateUtils;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.condition.EnabledIfEnvironmentVariable;

import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;

//@EnabledIfEnvironmentVariable(named = "DISPUTE_REPOSITORY_SRC", matches = "ords")
@EnabledIfEnvironmentVariable(named = "JJDISPUTE_REPOSITORY_SRC", matches = "ords")
class CleanupTest extends BaseTestSuite {

	@Test
	void test() {
		List<JJDispute> findByTicketNumber = jjDisputeRepository.findByTicketNumber("EA03148599");
		assertNotNull(findByTicketNumber);
		assertEquals(1, findByTicketNumber.size());

		List<Dispute> disputes = disputeRepository.findAll();

		Date purgeDate = DateUtils.addMonths(new Date(), -1);

		CollectionUtils.filter(disputes, d -> d.getCreatedTs() != null && purgeDate.after(d.getCreatedTs()));

		// Sort by createdTs DESC
		Collections.sort(disputes, (Dispute o1, Dispute o2) -> o1.getCreatedTs().compareTo(o2.getCreatedTs()));

		disputes.forEach(d -> {
			System.out.print(d.getDisputeId() + "\t" + d.getCreatedTs());
			List<JJDispute> jjDisputes = jjDisputeRepository.findByTicketNumber(d.getTicketNumber());
			if (jjDisputes.isEmpty()) {
				System.out.print(" removing...");
				disputeRepository.deleteById(d.getDisputeId());
			}
			System.out.println();
		});
	}

}
