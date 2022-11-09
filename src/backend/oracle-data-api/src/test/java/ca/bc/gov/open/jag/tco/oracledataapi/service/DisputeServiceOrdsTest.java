package ca.bc.gov.open.jag.tco.oracledataapi.service;

import static org.junit.jupiter.api.Assertions.assertTrue;

import java.util.ArrayList;
import java.util.Date;
import java.util.Iterator;
import java.util.List;

import org.apache.commons.lang3.builder.Diff;
import org.apache.commons.lang3.builder.ReflectionDiffBuilder;
import org.apache.commons.lang3.builder.ToStringStyle;
import org.apache.commons.lang3.time.DateFormatUtils;
import org.assertj.core.util.Arrays;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;

@ConditionalOnProperty(name = "repository.dispute", havingValue = "ords", matchIfMissing = true)
class DisputeServiceOrdsTest extends BaseTestSuite {

	@Autowired
	private DisputeService disputeService;

	@Test
	public void testOrdsPersistance() {
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

	private void logDiffs(List<Diff<?>> diffs, String objectClass) {
		if (!diffs.isEmpty()) {
			System.out.println("\n" + objectClass + " Diffs:");
			for (Diff<?> diff : diffs) {
				logDiff(diff);
			}
		}

	}

	private <T> List<Diff<?>> getDifferences(T lhs, T rhs, String... ignoredFields) {
		List<Diff<?>> diffs = new ArrayList<Diff<?>>(new ReflectionDiffBuilder<T>(lhs, rhs, ToStringStyle.JSON_STYLE).build().getDiffs());
		List<Object> skippedFields = Arrays.asList(ignoredFields);

		for (Iterator<Diff<?>> iterator = diffs.iterator(); iterator.hasNext();) {
			Diff<?> diff = iterator.next();

			// skip known field differences and child objects
			if (skippedFields.contains(diff.getFieldName())) {
				iterator.remove();
			}
		}

		return diffs;
	}

	private void logDiff(Diff<?> diff) {
		if (diff.getKey() != null && diff.getKey() instanceof Date) {
			StringBuffer sb = new StringBuffer();
			sb.append("[");
			sb.append(diff.getFieldName());
			sb.append(": ");
			if (diff.getFieldName().endsWith("Ts")) {
				sb.append(diff.getLeft() == null ? "null" : DateFormatUtils.ISO_8601_EXTENDED_DATETIME_TIME_ZONE_FORMAT.format(diff.getLeft()));
				sb.append(", ");
				sb.append(diff.getRight() == null ? "null" : DateFormatUtils.ISO_8601_EXTENDED_DATETIME_TIME_ZONE_FORMAT.format(diff.getRight()));
			}
			else {
				sb.append(diff.getLeft() == null ? "null" : DateFormatUtils.ISO_8601_EXTENDED_DATE_FORMAT.format(diff.getLeft()));
				sb.append(", ");
				sb.append(diff.getRight() == null ? "null" : DateFormatUtils.ISO_8601_EXTENDED_DATE_FORMAT.format(diff.getRight()));
			}
			sb.append("]");
			System.out.println(sb);
		}
		else {
			System.out.println(diff.toString());
		}
	}

}
