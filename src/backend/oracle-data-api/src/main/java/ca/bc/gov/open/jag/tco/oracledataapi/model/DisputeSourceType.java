package ca.bc.gov.open.jag.tco.oracledataapi.model;

/**
 * Where a Dispute was submitted from. Values match TCO.TCO_DISPUTE_SOURCE_TYPES.DISPUTE_SOURCE_TYPE_CD.
 */
public enum DisputeSourceType {

	/** Unknown type (undefined). Must be index 0. Never saved to the database. */
	UNKNOWN,
	/** Citizen Portal */
	CITZ,
	/** Staff Portal */
	STAF,
	/** JUSTIN */
	JSTN
}
