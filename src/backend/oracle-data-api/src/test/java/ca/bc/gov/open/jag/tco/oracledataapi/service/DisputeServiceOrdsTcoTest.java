package ca.bc.gov.open.jag.tco.oracledataapi.service;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

import java.util.Date;
import java.util.List;

import org.apache.commons.lang3.builder.Diff;
import org.apache.commons.lang3.time.DateUtils;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.condition.EnabledIfEnvironmentVariable;
import org.springframework.beans.factory.annotation.Autowired;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.HealthApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.PingResult;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;

@EnabledIfEnvironmentVariable(named = "JJDISPUTE_REPOSITORY_SRC", matches = "ords")
class DisputeServiceOrdsTcoTest extends BaseTestSuite {

	@Autowired
	private JJDisputeService jjDisputeService;

	@Autowired
	private JJDisputeRepository jjDisputeRepository;

	@Autowired
	private HealthApi ordsTcoHealthApi;

	@Test
	public void testPingOrdsTco() throws Exception {
		PingResult pingResult = ordsTcoHealthApi.ping();
		assertNotNull(pingResult);
		assertEquals("success", pingResult.getStatus());
	}

	@Test
	public void testJjDispute_GET() throws Exception {
		// TODO: replace this test with a POST/GET/PUT/DELETE to confirm CRUD
		// Hard-coded TicketNumber for now since there are no POST/DELETE endpoints yet.
		String ticketNumber = "EA90100004";

		JJDispute jjDispute = jjDisputeService.getJJDisputeByTicketNumber(ticketNumber);
		assertNotNull(jjDispute);
		assertEquals(ticketNumber, jjDispute.getTicketNumber());
	}

	@Test
	public void testSetStatus() throws Exception {
		String ticketNumber = "EA90100004";

		// Try setting the status to IN_PROGRESS
		JJDispute jjDisputeDb = jjDisputeService.setStatus(
				ticketNumber,
				JJDisputeStatus.IN_PROGRESS,
				getPrincipal(),
				"A custom remark 3. Setting status to IN_PROGRESS ...",
				"1",
				Long.valueOf(1l));
		assertNotNull(jjDisputeDb);
		assertEquals(ticketNumber, jjDisputeDb.getTicketNumber());
		assertEquals(JJDisputeStatus.IN_PROGRESS, jjDisputeDb.getStatus());

	}

	@Test
	public void testJjDisputeList_GET() throws Exception {
		List<JJDispute> jjDisputes = jjDisputeService.getJJDisputes(null, null);
		assertNotNull(jjDisputes);
		assertTrue(jjDisputes.size() > 0);
	}

	@Test
	public void testJjDispute_PUT() throws Exception {
		// Hard-coded TicketNumber for now since there are no POST endpoints yet to create our own JJDispute
		String ticketNumber = "EA90100004";

		JJDispute jjDispute = jjDisputeService.getJJDisputeByTicketNumber(ticketNumber);
		assertNotNull(jjDispute);
		assertEquals(ticketNumber, jjDispute.getTicketNumber());

		// Update the dispute with the same record
		JJDispute savedJJDispute = jjDisputeService.updateJJDispute(ticketNumber, jjDispute, getPrincipal());

		// compare jjDispute and savedJJDispute to ensure persistence works.
		List<Diff<?>> disputeDiffs = getDifferences(jjDispute, savedJJDispute,
				"remarks",
				"jjDisputedCounts",
				"jjDisputeCourtAppearanceRoPs");
		logDiffs(disputeDiffs, "JJDispute");

		List<Diff<?>> remarkDiffs = getDifferences(jjDispute.getRemarks().get(0), savedJJDispute.getRemarks().get(0),
				"jjDispute");
		logDiffs(remarkDiffs, "remarks");

		List<Diff<?>> disputeCountDiffs = getDifferences(jjDispute.getJjDisputedCounts().get(0), savedJJDispute.getJjDisputedCounts().get(0),
				"jjDispute",
				"jjDisputedCountRoP",
				"modifiedTs");
		logDiffs(disputeCountDiffs, "disputeCounts");

		List<Diff<?>> disputeCountRoPDiffs = getDifferences(jjDispute.getJjDisputedCounts().get(0).getJjDisputedCountRoP(), savedJJDispute.getJjDisputedCounts().get(0).getJjDisputedCountRoP(),
				"modifiedTs");
		logDiffs(disputeCountRoPDiffs, "disputeCountRoP");

		List<Diff<?>> courtAppearancesDiffs = getDifferences(jjDispute.getJjDisputeCourtAppearanceRoPs().get(0), savedJJDispute.getJjDisputeCourtAppearanceRoPs().get(0),
				"jjDispute");
		logDiffs(courtAppearancesDiffs, "courtAppearances");
	}

	@Test
	public void testJjDisputeAssignJj_POST() throws Exception {
		String ticketNumber = "EA90100004";
		JJDispute jjDispute = jjDisputeService.getJJDisputeByTicketNumber(ticketNumber);
		String jjAssignedTo = jjDispute.getJjAssignedTo();

		String username = RandomUtil.randomGivenName().charAt(0) + RandomUtil.randomSurname();
		assertNotEquals(jjAssignedTo, username); // confirm jjAssignedTo is not already set to the random name.
		jjDisputeRepository.assignJJDisputeJj(ticketNumber, username);

		jjDispute = jjDisputeService.getJJDisputeByTicketNumber(ticketNumber);
		assertEquals(username, jjDispute.getJjAssignedTo());
	}

	@Test
	public void testJjDisputeUnassignJj_POST() throws Exception {
		String ticketNumber = "EA90100004";
		String username = RandomUtil.randomGivenName().charAt(0) + RandomUtil.randomSurname();

		jjDisputeRepository.assignJJDisputeJj(ticketNumber, username);
		JJDispute jjDispute = jjDisputeService.getJJDisputeByTicketNumber(ticketNumber);
		assertNotNull(jjDispute.getJjAssignedTo());

		// Setting the username to null should "unassign" JJDispute
		jjDisputeRepository.assignJJDisputeJj(ticketNumber, null);
		jjDispute = jjDisputeService.getJJDisputeByTicketNumber(ticketNumber);
		assertNull(jjDispute.getJjAssignedTo());
	}

	@Test
	public void testJjDisputeAssignVtc_POST() throws Exception {
		String ticketNumber = "EA90100004";
		JJDispute jjDispute = jjDisputeService.getJJDisputeByTicketNumber(ticketNumber);
		String vtcAssignedTo = jjDispute.getVtcAssignedTo();

		String username = RandomUtil.randomGivenName().charAt(0) + RandomUtil.randomSurname();
		assertNotEquals(vtcAssignedTo, username); // confirm vtcAssignedTo is not already set to the random name.
		jjDisputeRepository.assignJJDisputeVtc(ticketNumber, username);

		jjDispute = jjDisputeService.getJJDisputeByTicketNumber(ticketNumber);
		assertEquals(username, jjDispute.getVtcAssignedTo());
		// vtcAssignedTs should have been set by the stored procedure so we don't know the exact time, but it should have happened in the last 5 minutes.
		assertTrue(jjDispute.getVtcAssignedTs().after(DateUtils.addMinutes(new Date(), -5)));
	}

	@Test
	public void testJjDisputeUnAssignVtc_POST() throws Exception {
		String ticketNumber = "EA90100004";
		String username = RandomUtil.randomGivenName().charAt(0) + RandomUtil.randomSurname();

		// Ensure JJDispute is assigned.
		jjDisputeRepository.assignJJDisputeVtc(ticketNumber, username);
		JJDispute jjDispute = jjDisputeService.getJJDisputeByTicketNumber(ticketNumber);
		assertNotNull(jjDispute.getVtcAssignedTo());
		assertNotNull(jjDispute.getVtcAssignedTs());

		Thread.sleep(1000);

		// Attempt to unassign JJDispute, assert
		jjDisputeRepository.unassignJJDisputeVtc(ticketNumber, new Date());
		jjDispute = jjDisputeService.getJJDisputeByTicketNumber(ticketNumber);
		assertNull(jjDispute.getVtcAssignedTo());
		assertNull(jjDispute.getVtcAssignedTs());
	}

}
