package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import javax.persistence.CascadeType;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;
import javax.persistence.FetchType;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.OneToMany;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;
import javax.validation.constraints.Email;

import com.fasterxml.jackson.annotation.JsonManagedReference;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * @author 237563 Represents a violation ticket dispute for JJ Workbench
 *
 */
//mark class as an Entity
@Entity
//defining class name as Table name
@Table
@Getter
@Setter
@NoArgsConstructor
public class JJDispute extends Auditable<String> {

	@Id
	@GeneratedValue
	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	private Long id;

	/**
	 * The violation ticket number as unique identifier.
	 */
	@Column(length = 100, nullable=false)
	@Schema(nullable = false)
	private String ticketNumber;
	
	/**
	 * Indicates whether the dispute is associated with an accident or not
	 */
	@Column(nullable = true)
	@Schema(description = "Indicates whether the dispute is associated with an accident or not", nullable = true)
	@Enumerated(EnumType.STRING)
	private YesNo accidentYn;

	/**
	 * The mailing address line1 of the disputant from contact information submitted via TCO as free form text.
	 */
	@Column(length = 100, nullable=false)
	@Schema(nullable = false)
	private String addressLine1;

	/**
	 * The mailing address line2 of the disputant from contact information submitted via TCO as free form text.
	 */
	@Column(length = 100)
	@Schema(nullable = true)
	private String addressLine2;

	/**
	 * The mailing address line3 of the disputant from contact information submitted via TCO as free form text.
	 */
	@Column(length = 100)
	@Schema(nullable = true)
	private String addressLine3;

	/**
	 * The mailing address city of the disputant from contact information submitted via TCO as free form text.
	 */
	@Column(length = 100)
	@Schema(nullable = true)
	private String addressCity;

	/**
	 * The mailing address province of the disputant from reconciled ticket data as free form text.
	 */
	@Column(length = 100)
	@Schema(nullable = true)
	private String addressProvince;

	/**
	 * The mailing address country of the disputant from reconciled ticket data as free form text.
	 */
	@Column(length = 100)
	@Schema(nullable = true)
	private String addressCountry;

	/**
	 * The mailing address postal code of the disputant from reconciled ticket data as free form text.
	 */
	@Column(length = 10)
	@Schema(nullable = true)
	private String addressPostalCode;

	/**
	 * The disputant's birthdate from contact information submitted via TCO.
	 */
	@Column
	@Temporal(TemporalType.DATE)
	@Schema(nullable = true)
	private Date disputantBirthdate;
	
	/**
	 * Surname of the disputant on violation ticket. Data source for this field is justin_participant_profiles.prof_surname_nm
	 */
	@Schema(description = "Surname of the disputant on violation ticket.", nullable = true)
	private String disputantSurname;
	
	/**
	 * First given name of the disputant on violation ticket. Data source for this field is justin_participant_profiles.prof_given_1_nm
	 */
	@Schema(description = "First given name of the disputant on violation ticket.", nullable = true)
	private String disputantGivenName1;
	
	/**
	 * Second given name of the disputant on violation ticket. Data source for this field is justin_participant_profiles.prof_given_2_nm
	 */
	@Schema(description = "Second given name of the disputant on violation ticket.", nullable = true)
	private String disputantGivenName2;
	
	/**
	 * Third given name of the disputant on violation ticket. Data source for this field is justin_participant_profiles.prof_given_3_nm
	 */
	@Schema(description = "Third given name of the disputant on violation ticket.", nullable = true)
	private String disputantGivenName3;

	/**
	 * The drivers licence number from reconciled ticket data.
	 */
	@Column(length = 30)
	@Schema(nullable = true)
	private String driversLicenceNumber;

	/**
	 * The province lookup sequence number from the reconciled ticket data.
	 */
	@Column(length = 4)
	@Schema(nullable = true)
	private String drvLicIssuedProvSeqNo;

	/**
	 * The country lookup sequence number from the reconciled ticket data.
	 */
	@Column(length = 4)
	@Schema(nullable = true)
	private String drvLicIssuedCtryId;

	/**
	 * The disputant's email address from reconciled ticket data.
	 */
	@Column(length = 100)
	@Email(regexp = ".+@.+\\..+")
	@Schema(nullable = true)
	private String emailAddress;

	@Column(nullable = false)
	@Schema(nullable = false)
	private JJDisputeStatus status;

	@Column
	@Schema(nullable = true)
	private JJDisputeHearingType hearingType;
	
	/**
	 * Indicates whether the dispute is associated with multiple officers or not
	 */
	@Column
	@Schema(description = "Indicates whether the dispute is associated with multiple officers or not", nullable = false)
	@Enumerated(EnumType.STRING)
	private YesNo multipleOfficersYn;

	@Column(length = 36)
	@Schema(nullable = true)
	private String noticeOfDisputeGuid;

	@Column(nullable = true)
	@Schema(nullable = true)
	@Enumerated(EnumType.STRING)
	private YesNo noticeOfHearingYn;

	@Column(length = 30)
	@Schema(nullable = true)
	private String occamDisputantGiven1Nm;

	@Column(length = 30)
	@Schema(nullable = true)
	private String occamDisputantGiven2Nm;

	@Column(length = 30)
	@Schema(nullable = true)
	private String occamDisputantGiven3Nm;

	@Column(length = 30)
	@Schema(nullable = true)
	private String occamDisputantSurnameNm;
	
	/**
	 * Disputant's home or work phone number provided on submission.
	 */
	@Schema(description = "Disputant's home or work phone number provided on submission.", nullable = true)
	private String occamDisputantPhoneNumber;

	@Schema(nullable = false)
	private Long occamDisputeId;

	@Column(length = 15)
	@Schema(nullable = true)
	private String occamViolationTicketUpldId;

	/**
	 * The date submitted by disputant in TCO.
	 */
	@Column
	@Schema(nullable = true)
	@Temporal(TemporalType.TIMESTAMP)
	private Date submittedTs;

	@Column
	@Schema(nullable = true)
	@Temporal(TemporalType.TIMESTAMP)
	private Date issuedTs;

	/**
	 * The date and time the violation ticket was issued.
	 */
	@Column
	@Schema(nullable = true)
	@Temporal(TemporalType.TIMESTAMP)
	private Date violationDate;

	/**
	 * The date received in TRM from ICBC.
	 */
	@Column
	@Schema(nullable = true)
	@Temporal(TemporalType.TIMESTAMP)
	private Date icbcReceivedDate;

	/**
	 * The enforcement officer associated to the disputed violation ticket.
	 */
	@Column
	@Schema(nullable = true)
	private String enforcementOfficer;

	@Column(nullable = true)
	@Schema(nullable = true)
	@Enumerated(EnumType.STRING)
	private YesNo electronicTicketYn;

	/**
	 * The police detachment location.
	 */
	@Column
	@Schema(nullable = true)
	private String policeDetachment;

	/**
	 * The provincial court hearing location named on the violation ticket.
	 */
	@Column
	@Schema(nullable = true)
	private String courthouseLocation;

	/**
	 * The location where the offence took place.
	 */
	@Column
	@Schema(nullable = true)
	private String offenceLocation;

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
	 * VTC assigned to review JJ Decision
	 */
	@Column
	@Schema(nullable = true)
	private String vtcAssignedTo;

	/**
	 * Date and Time that VTC was assigned
	 */
	@Column
	@Schema(nullable = true)
	@Temporal(TemporalType.TIMESTAMP)
	private Date vtcAssignedTs;

	/**
	 * Reason indicating why the fine reduction requested for the ticket.
	 */
	@Column(length = 256)
	@Schema(nullable = true)
	private String fineReductionReason;

	/**
	 * Reason indicating why the disputant needs more time in order to make payment for the ticket.
	 */
	@Column(length = 256)
	@Schema(nullable = true)
	private String timeToPayReason;

	@Column(nullable = true)
	@Schema(nullable = true)
	private String contactLawFirmName;

	@Column(nullable = true)
	@Schema(nullable = true)
	private String contactGivenName1;

	@Column(nullable = true)
	@Schema(nullable = true)
	private String contactGivenName2;

	@Column(nullable = true)
	@Schema(nullable = true)
	private String contactGivenName3;

	@Column(nullable = true)
	@Schema(nullable = true)
	private String contactSurname;

	@Column(nullable = true)
	@Schema(nullable = true)
	private ContactType contactType;

	/**
	 * Does the want to appear in court?
	 */
	@Column(nullable = false)
	@Schema(nullable = false)
	@Enumerated(EnumType.STRING)
	private YesNo appearInCourt;

	@Column
	@Schema(nullable = true)
	private String courtAgenId;
	
	/**
	 * Indicates whether the dispute is re-opened by a JJ and set to review from its previous accepted or concluded status or not.
	 */
	@Schema(description = "Indicates whether the dispute is re-opened by a JJ and set to review from its previous accepted or concluded status or not.", nullable = true)
	private Boolean recalled;

	/**
	 * Name of the law firm that will represent the disputant at the hearing.
	 */
	@Column(length = 200)
	@Schema(nullable = true)
	private String lawFirmName;

	/**
	 * Surname of the lawyer
	 */
	@Column(length = 30)
	@Schema(nullable = true)
	private String lawyerSurname;

	/**
	 * First given name of the lawyer
	 */
	@Column(length = 30)
	@Schema(nullable = true)
	private String lawyerGivenName1;

	/**
	 * Second given name of the lawyer
	 */
	@Column(length = 30)
	@Schema(nullable = true)
	private String lawyerGivenName2;

	/**
	 * Third given name of the lawyer
	 */
	@Column(length = 30)
	@Schema(nullable = true)
	private String lawyerGivenName3;

	@Column
	@Schema(nullable = true)
	private String justinRccId;

	/**
	 * The disputant requires spoken language interpreter. The language name is indicated in this field.
	 */
	@Column
	@Schema(nullable = true)
	private String interpreterLanguageCd;

	/**
	 * Number of witness that the disputant intends to call.
	 */
	@Column(length = 3)
	@Schema(nullable = true)
	private Integer witnessNo;

	/**
	 * The disputants attendance type.
	 */
	@Column
	@Schema(nullable = true)
	@Enumerated(EnumType.STRING)
	private JJDisputeAttendanceType disputantAttendanceType;
	
	/**
	 * Signatory Type. Can be either 'D' for Disputant or 'A' for Agent.
	 */
	@Schema(description = "Signatory Type. Can be either 'D' for Disputant or 'A' for Agent.", nullable = true)
	private SignatoryType signatoryType;
	
	/**
	 * Name of the person who signed the dispute.
	 */
	@Schema(description = "Name of the person who signed the dispute.", maxLength = 100, nullable = true)
	private String signatoryName;

	/**
	 * All the remarks for this jj dispute that are for internal use of JJs.
	 */
	@JsonManagedReference
	@OneToMany(targetEntity = JJDisputeRemark.class, cascade = CascadeType.ALL, fetch = FetchType.LAZY, orphanRemoval = true, mappedBy = "jjDispute")
	public List<JJDisputeRemark> remarks = new ArrayList<JJDisputeRemark>();

	@JsonManagedReference(value = "jj_dispute_count_reference")
	@OneToMany(targetEntity = JJDisputedCount.class, cascade = CascadeType.ALL, fetch = FetchType.LAZY, orphanRemoval = true)
	@JoinColumn(name = "jjdispute_id")
	public List<JJDisputedCount> jjDisputedCounts = new ArrayList<JJDisputedCount>();

	@JsonManagedReference(value = "jj_dispute_court_appearance_rop_reference")
	@OneToMany(targetEntity = JJDisputeCourtAppearanceRoP.class, cascade = CascadeType.ALL, fetch = FetchType.LAZY, orphanRemoval = true)
	@JoinColumn(name = "jjdispute_id")
	public List<JJDisputeCourtAppearanceRoP> jjDisputeCourtAppearanceRoPs = new ArrayList<JJDisputeCourtAppearanceRoP>();

	/**
	 * Simple constructor to create a blank JJDispute with the id.
	 */
	public JJDispute(Long jjDisputeId) {
		this.id = jjDisputeId;
	}

	public void addJJDisputedCounts(List<JJDisputedCount> disputedCounts) {
		for (JJDisputedCount disputedCount : disputedCounts) {
			disputedCount.setJjDispute(this);
		}
		this.jjDisputedCounts.addAll(disputedCounts);
	}

	public void addRemarks(List<JJDisputeRemark> remarks) {
		for (JJDisputeRemark remark : remarks) {
			remark.setJjDispute(this);
		}
		this.remarks.addAll(remarks);
	}

	public void addJJDisputeCourtAppearances(List<JJDisputeCourtAppearanceRoP> disputeCourtAppearanceRoPs) {
		for (JJDisputeCourtAppearanceRoP disputeCourtAppearanceRoP : disputeCourtAppearanceRoPs) {
			disputeCourtAppearanceRoP.setJjDispute(this);
		}
		this.jjDisputeCourtAppearanceRoPs.addAll(disputeCourtAppearanceRoPs);
	}

}
