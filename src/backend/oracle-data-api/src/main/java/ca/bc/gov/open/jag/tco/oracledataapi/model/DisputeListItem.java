package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.Date;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * @author 237563 Represents a violation ticket notice of dispute fields for workbench list.
 *
 */
//mark class as an Entity
@Entity
//defining class name as Table name
@Table
@Getter
@Setter
@NoArgsConstructor
public class DisputeListItem {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	@Id
	@GeneratedValue
	private Long disputeId;

	/**
	 * The violation ticket number.
	 */
	@Column(length = 50)
	@Schema(nullable = false)
	private String ticketNumber;

	/**
	 * The date and time the citizen submitted the notice of dispute.
	 * This should always be in UTC date-time (ISO 8601) format
	 */
	@Column
	@Schema(nullable = true)
	@Temporal(TemporalType.TIMESTAMP)
	private Date submittedTs;

	/**
	 * The surname or corporate name.
	 */
	@Column(length = 30)
	@Schema(nullable = true) // needs to be nullable to provide a patch update (which may or may not include a surname)
	private String disputantSurname;

	/**
	 * First given name of the disputant
	 */
	@Column(length = 30)
	@Schema(nullable = true)
	private String disputantGivenName1;
	
	/**
	 * Second given name of the disputant
	 */
	@Column(length = 30)
	@Schema(nullable = true)
	private String disputantGivenName2;

	/**
	 * Third given name of the disputant
	 */
	@Column(length = 30)
	@Schema(nullable = true)
	private String disputantGivenName3;

	@Enumerated(EnumType.STRING)
	private DisputeStatus status;

	/**
	 * The disputant's email address.
	 */
	@Column(length = 100)
	@Schema(nullable = true)
	private String emailAddress;

	/**
	 * Indicates whether the disputant's email address is verified or not.
	 */
	@Column
	private Boolean emailAddressVerified = Boolean.FALSE;

	@Column
	@Schema(nullable = true)
	@Temporal(TemporalType.DATE)
	private Date filingDate;

	@Column(nullable = true)
	@Schema(nullable = true)
	@Enumerated(EnumType.STRING)
	private YesNo requestCourtAppearanceYn;

	/**
	 * The IDIR of the Staff whom the dispute is assigned to be reviewed on Staff
	 * Portal.
	 */
	@Column(length = 30)
	@Schema(nullable = true)
	private String userAssignedTo;

	/**
	 * The date and time a dispute was assigned to a Staff to be reviewed.
	 * This should always be in UTC date-time (ISO 8601) format
	 */
	@Column
	@Schema(nullable = true)
	@Temporal(TemporalType.TIMESTAMP)
	private Date userAssignedTs;

	/**
	 * Identifier for whether the citizen has detected any issues with the OCR
	 * ticket result or not.
	 */
	@Column
	@Schema(nullable = true)
	@Enumerated(EnumType.STRING)
	private YesNo disputantDetectedOcrIssues;

	/**
	 * Identifier for whether the system has detected any issues with the OCR ticket
	 * result or not.
	 */
	@Column
	@Schema(nullable= false)
	@Enumerated(EnumType.STRING)
	private YesNo systemDetectedOcrIssues;
	
	/**
	 * The date and time the violation ticket was issued.
	 */
	@Column
	@Schema(nullable = true)
	@Temporal(TemporalType.TIMESTAMP)
	private Date violationDate;
	
	@Column
	@Schema(nullable = true)
	@Enumerated(EnumType.STRING)
	private JJDisputeStatus jjDisputeStatus;
	
	/**
	 * The ID of the Staff whom the dispute is assigned to be reviewed on JJ Workbench.
	 */
	@Column
	@Schema(nullable = true)
	private String jjAssignedTo;
	
	/**
	 * The ID of the JJ who made a decision.
	 */
	@Column
	@Schema(nullable = true)
	private String decisionMadeBy;

	/**
	 * The date that JJ made a decision
	 */
	@Column
	@Schema(nullable = true)
	@Temporal(TemporalType.TIMESTAMP)
	private Date jjDecisionDate;
	
	/**
	 * Lookup ID for courthouse data
	 */
	@Column
	@Schema(nullable = true)
	private String courtAgenId;
}
