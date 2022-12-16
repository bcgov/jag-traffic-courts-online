package ca.bc.gov.open.jag.tco.oracledataapi.model;

/**
 * An enumeration of available Statuses on a JJ Dispute record.
 */
public enum JJDisputeStatus {
	/** Unknown type (undefined). Must be index 0. */
	UNKNOWN,
	NEW,
	IN_PROGRESS,
	DATA_UPDATE,
	CONFIRMED,
	REQUIRE_COURT_HEARING,
	REQUIRE_MORE_INFO,
	ACCEPTED,
	REVIEW,
	HEARING_SCHEDULED
}
