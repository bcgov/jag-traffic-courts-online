package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.Date;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * @author 237563 Represents a violation ticket notice of dispute fields for workbench list.
 *
 */
//mark class as an Entity
@Getter
@Setter
@NoArgsConstructor
public class DisputeListItem {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	private Long disputeId;

	/**
	 * The violation ticket number.
	 */
	@Schema(nullable = false)
	private String ticketNumber;

	/**
	 * The date and time the citizen submitted the notice of dispute.
	 * This should always be in UTC date-time (ISO 8601) format
	 */
	@Schema(nullable = true)
	private Date submittedTs;

	/**
	 * The surname or corporate name.
	 */
	@Schema(nullable = true) // needs to be nullable to provide a patch update (which may or may not include a surname)
	private String disputantSurname;

	/**
	 * First given name of the disputant
	 */
	@Schema(nullable = true)
	private String disputantGivenName1;

	/**
	 * Second given name of the disputant
	 */
	@Schema(nullable = true)
	private String disputantGivenName2;

	/**
	 * Third given name of the disputant
	 */
	@Schema(nullable = true)
	private String disputantGivenName3;

	private DisputeStatus status;

	/**
	 * The disputant's email address.
	 */
	@Schema(nullable = true)
	private String emailAddress;

	/**
	 * Indicates whether the disputant's email address is verified or not.
	 */
	private Boolean emailAddressVerified = Boolean.FALSE;

	@Schema(nullable = true)
	private Date filingDate;

	@Schema(nullable = true)
	private YesNo requestCourtAppearanceYn;

	/**
	 * The IDIR of the Staff whom the dispute is assigned to be reviewed on Staff
	 * Portal.
	 */
	@Schema(nullable = true)
	private String userAssignedTo;

	/**
	 * The date and time a dispute was assigned to a Staff to be reviewed.
	 * This should always be in UTC date-time (ISO 8601) format
	 */
	@Schema(nullable = true)
	private Date userAssignedTs;

	/**
	 * Identifier for whether the citizen has detected any issues with the OCR
	 * ticket result or not.
	 */
	@Schema(nullable = true)
	private YesNo disputantDetectedOcrIssues;

	/**
	 * Identifier for whether the system has detected any issues with the OCR ticket
	 * result or not.
	 */
	@Schema(nullable= false)
	private YesNo systemDetectedOcrIssues;

	/**
	 * The date and time the violation ticket was issued.
	 */
	@Schema(nullable = true)
	private Date violationDate;

	@Schema(nullable = true)
	private JJDisputeStatus jjDisputeStatus;

	/**
	 * The ID of the Staff whom the dispute is assigned to be reviewed on JJ Workbench.
	 */
	@Schema(nullable = true)
	private String jjAssignedTo;

	/**
	 * The ID of the JJ who made a decision.
	 */
	@Schema(nullable = true)
	private String decisionMadeBy;

	/**
	 * The date that JJ made a decision
	 */
	@Schema(nullable = true)
	private Date jjDecisionDate;

	/**
	 * Lookup ID for courthouse data
	 */
	@Schema(nullable = true)
	private String courtAgenId;
}
