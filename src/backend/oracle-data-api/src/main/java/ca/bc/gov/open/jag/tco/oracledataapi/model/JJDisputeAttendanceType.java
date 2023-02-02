package ca.bc.gov.open.jag.tco.oracledataapi.model;

/**
 * An enumeration of available Statuses on a JJ Dispute record.
 */
public enum JJDisputeAttendanceType {
	/** Unknown type (undefined). Must be index 0. */
	UNKNOWN,
    WRITTEN_REASONS,
	VIDEO_CONFERENCE,
	TELEPHONE_CONFERENCE,
	MSTEAMS_AUDIO,
	MSTEAMS_VIDEO,
	IN_PERSON
}
