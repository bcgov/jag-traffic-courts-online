package ca.bc.gov.open.jag.tco.oracledataapi.service;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

import java.text.ParseException;
import java.util.List;

import org.apache.commons.lang3.builder.Diff;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.condition.EnabledIfEnvironmentVariable;
import org.springframework.beans.factory.annotation.Autowired;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequest;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.HealthApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.PingResult;
import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;

@EnabledIfEnvironmentVariable(named = "DISPUTE_REPOSITORY_SRC", matches = "ords")
class DisputeServiceOrdsOccamTest extends BaseTestSuite {

	@Autowired
	private DisputeService disputeService;

	@Autowired
	private HealthApi ordsOccamHealthApi;

	@Test
	public void testPingOrdsOccam() throws Exception {
		PingResult pingResult = ordsOccamHealthApi.ping();
		assertNotNull(pingResult);
		assertEquals("success", pingResult.getStatus());
	}

	@Test
	public void testOrdsPersistance() throws ParseException {
		// This test creates a Dispute where all fields are populated, saves it to ORDs, loads it from ORDs,
		// and then compares the values of all fields to the original copy - they should be identical (except for the noted ignoredFields).

		Dispute dispute = RandomUtil.createFullyPopulatedDispute();
		Dispute savedDispute = disputeService.save(dispute);

		List<Diff<?>> disputeDiffs = getDifferences(dispute, savedDispute,
				//"disputeId",
				"violationTicket",
				"disputeCounts",
				"createdBy",
				"createdTs");
		logDiffs(disputeDiffs, "Dispute");

		List<Diff<?>> disputeCntDiffs = getDifferences(dispute.getDisputeCounts().get(0), savedDispute.getDisputeCounts().get(0),
				"dispute",
				"createdBy",
				"createdTs");
		logDiffs(disputeCntDiffs, "DisputeCount");

		List<Diff<?>> vioTicketDiffs = getDifferences(dispute.getViolationTicket(), savedDispute.getViolationTicket(),
				"violationTicketId",
				"dispute",
				"violationTicketCounts",
				"createdBy",
				"createdTs");
		logDiffs(vioTicketDiffs, "ViolationTicket");

		List<Diff<?>> vioTicketCntDiffs = getDifferences(dispute.getViolationTicket().getViolationTicketCounts().get(0), savedDispute.getViolationTicket().getViolationTicketCounts().get(0),
				"violationTicketCountId",
				"violationTicket",
				"createdBy",
				"createdTs");
		logDiffs(vioTicketCntDiffs, "ViolationTicketCount");

		// cleanup ORDs
		disputeService.delete(savedDispute.getDisputeId());

		assertTrue(disputeDiffs.isEmpty());
		assertTrue(disputeCntDiffs.isEmpty());
		assertTrue(vioTicketDiffs.isEmpty());
		assertTrue(vioTicketCntDiffs.isEmpty());
	}

	@Test
	public void testDisputeUpdateRequestOrdsPersistance() throws ParseException {
		// This test creates a Dispute and a DisputeUpdateRequest where all fields are populated, saves it to ORDs, loads it from ORDs,
		// and then compares the values of all fields to the original copy - they should be identical (except for the noted ignoredFields).

		Dispute dispute = RandomUtil.createFullyPopulatedDispute();
		Dispute savedDispute = disputeService.save(dispute);

		DisputeUpdateRequest updateRequest = RandomUtil.createDisputeUpdateRequest(savedDispute.getDisputeId());
		DisputeUpdateRequest savedUpdateRequest = disputeService.saveDisputeUpdateRequest(savedDispute.getNoticeOfDisputeGuid(), updateRequest);

		List<Diff<?>> disputeUpdateRequestDiffs = getDifferences(updateRequest, savedUpdateRequest,
				"disputeUpdateRequestId",
				"createdBy",
				"createdTs");
		logDiffs(disputeUpdateRequestDiffs, "DisputeUpdateRequest");

		// cleanup ORDs
		disputeService.deleteDisputeUpdateRequest(savedUpdateRequest.getDisputeUpdateRequestId());
		disputeService.delete(savedDispute.getDisputeId());

		assertTrue(disputeUpdateRequestDiffs.isEmpty());
	}

}
