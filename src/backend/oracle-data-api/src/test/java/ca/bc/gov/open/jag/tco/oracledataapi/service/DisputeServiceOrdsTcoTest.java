package ca.bc.gov.open.jag.tco.oracledataapi.service;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.condition.EnabledIfEnvironmentVariable;
import org.springframework.beans.factory.annotation.Autowired;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.HealthApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.JjDisputeApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.PingResult;

@EnabledIfEnvironmentVariable(named = "JJDISPUTE_REPOSITORY_SRC", matches = "ords")
class DisputeServiceOrdsTcoTest extends BaseTestSuite {

	@Autowired
	private HealthApi ordsTcoHealthApi;

	@Autowired
	private JjDisputeApi ordsTcoJjDisputeApi;

	@Test
	void testPingOrdsTco() throws Exception {
		PingResult pingResult = ordsTcoHealthApi.ping();
		assertNotNull(pingResult);
		assertEquals("success", pingResult.getStatus());
	}

	@Test
	void testJjDispute_GET() throws Exception {
		// TODO: replace this test with a POST/GET/PUT/DELETE to confirm CRUD
		// Hard-coded TicketNumber for now since there are no POST/DELETE endpoints yet.
		JJDispute jjDispute = ordsTcoJjDisputeApi.v1JjDisputeGet("EA90100004");

		assertNotNull(jjDispute);
		assertEquals("EA90100004", jjDispute.getTicketNumberTxt());
	}

}
