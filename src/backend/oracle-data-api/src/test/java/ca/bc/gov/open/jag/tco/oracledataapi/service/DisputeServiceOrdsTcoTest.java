package ca.bc.gov.open.jag.tco.oracledataapi.service;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.condition.EnabledIfEnvironmentVariable;
import org.springframework.beans.factory.annotation.Autowired;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.HealthApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.PingResult;

@EnabledIfEnvironmentVariable(named = "JJDISPUTE_REPOSITORY_SRC", matches = "ords")
class DisputeServiceOrdsTcoTest extends BaseTestSuite {

	@Autowired
	private HealthApi ordsTcoHealthApi;

	@Test
	void testPingOrdsTco() throws Exception {
		PingResult pingResult = ordsTcoHealthApi.ping();
		assertNotNull(pingResult);
		assertEquals("success", pingResult.getStatus());
	}

}
