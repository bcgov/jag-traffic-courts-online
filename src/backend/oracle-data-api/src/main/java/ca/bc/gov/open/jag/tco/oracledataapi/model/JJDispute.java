package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import jakarta.validation.constraints.Email;

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
@Getter
@Setter
@NoArgsConstructor
public class JJDispute extends Auditable<String> {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	private Long id;

	/**
	 * The violation ticket number as unique identifier.
	 */
	@Schema(nullable = false)
	private String ticketNumber;

	/**
	 * Indicates whether the dispute is associated with an accident or not
	 */
	@Schema(description = "Indicates whether the dispute is associated with an accident or not", nullable = true)
	private YesNo accidentYn;

	/**
	 * The mailing address line1 of the disputant from contact information submitted via TCO as free form text.
	 */
	@Schema(nullable = false)
	private String addressLine1;

	/**
	 * The mailing address line2 of the disputant from contact information submitted via TCO as free form text.
	 */
	@Schema(nullable = true)
	private String addressLine2;

	/**
	 * The mailing address line3 of the disputant from contact information submitted via TCO as free form text.
	 */
	@Schema(nullable = true)
	private String addressLine3;

	/**
	 * The mailing address city of the disputant from contact information submitted via TCO as free form text.
	 */
	@Schema(nullable = true)
	private String addressCity;

	/**
	 * The mailing address province of the disputant from reconciled ticket data as free form text.
	 */
	@Schema(nullable = true)
	private String addressProvince;

	/**
	 * The mailing address country of the disputant from reconciled ticket data as free form text.
	 */
	@Schema(nullable = true)
	private String addressCountry;

	/**
	 * The mailing address postal code of the disputant from reconciled ticket data as free form text.
	 */
	@Schema(nullable = true)
	private String addressPostalCode;

	/**
	 * The disputant's birthdate from contact information submitted via TCO.
	 */
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
	@Schema(nullable = true)
	private String driversLicenceNumber;

	/**
	 * The province lookup sequence number from the reconciled ticket data.
	 */
	@Schema(nullable = true)
	private String drvLicIssuedProvSeqNo;

	/**
	 * The country lookup sequence number from the reconciled ticket data.
	 */
	@Schema(nullable = true)
	private String drvLicIssuedCtryId;

	/**
	 * The disputant's email address from reconciled ticket data.
	 */
	@Email(regexp = ".+@.+\\..+")
	@Schema(nullable = true)
	private String emailAddress;

	@Schema(nullable = false)
	private JJDisputeStatus status;

	@Schema(nullable = true)
	private JJDisputeHearingType hearingType;

	/**
	 * Indicates whether the dispute is associated with multiple officers or not
	 */
	@Schema(description = "Indicates whether the dispute is associated with multiple officers or not", nullable = false)
	private YesNo multipleOfficersYn;

	@Schema(nullable = true)
	private String noticeOfDisputeGuid;

	@Schema(nullable = true)
	private YesNo noticeOfHearingYn;

	@Schema(nullable = true)
	private String occamDisputantGiven1Nm;

	@Schema(nullable = true)
	private String occamDisputantGiven2Nm;

	@Schema(nullable = true)
	private String occamDisputantGiven3Nm;

	@Schema(nullable = true)
	private String occamDisputantSurnameNm;

	/**
	 * Disputant's home or work phone number provided on submission.
	 */
	@Schema(description = "Disputant's home or work phone number provided on submission.", nullable = true)
	private String occamDisputantPhoneNumber;

	@Schema(nullable = false)
	private Long occamDisputeId;

	@Schema(nullable = true)
	private String occamViolationTicketUpldId;

	/**
	 * The date submitted by disputant in TCO.
	 */
	@Schema(nullable = true)
	private Date submittedTs;

	@Schema(nullable = true)
	private Date issuedTs;

	/**
	 * The date and time the violation ticket was issued.
	 */
	@Schema(nullable = true)
	private Date violationDate;

	/**
	 * The date received in TRM from ICBC.
	 */
	@Schema(nullable = true)
	private Date icbcReceivedDate;

	/**
	 * The enforcement officer associated to the disputed violation ticket.
	 */
	@Schema(nullable = true)
	private String enforcementOfficer;

	@Schema(nullable = true)
	private YesNo electronicTicketYn;

	/**
	 * The police detachment location.
	 */
	@Schema(nullable = true)
	private String policeDetachment;

	/**
	 * The provincial court hearing location named on the violation ticket.
	 */
	@Schema(nullable = true)
	private String courthouseLocation;

	/**
	 * The location where the offence took place.
	 */
	@Schema(nullable = true)
	private String offenceLocation;

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
	 * VTC assigned to review JJ Decision
	 */
	@Schema(nullable = true)
	private String vtcAssignedTo;

	/**
	 * Date and Time that VTC was assigned
	 */
	@Schema(nullable = true)
	private Date vtcAssignedTs;

	/**
	 * Reason indicating why the fine reduction requested for the ticket.
	 */
	@Schema(nullable = true)
	private String fineReductionReason;

	/**
	 * Reason indicating why the disputant needs more time in order to make payment for the ticket.
	 */
	@Schema(nullable = true)
	private String timeToPayReason;

	@Schema(nullable = true)
	private String contactLawFirmName;

	@Schema(nullable = true)
	private String contactGivenName1;

	@Schema(nullable = true)
	private String contactGivenName2;

	@Schema(nullable = true)
	private String contactGivenName3;

	@Schema(nullable = true)
	private String contactSurname;

	@Schema(nullable = true)
	private ContactType contactType;

	/**
	 * Does the want to appear in court?
	 */
	@Schema(nullable = false)
	private YesNo appearInCourt;

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
	@Schema(nullable = true)
	private String lawFirmName;

	/**
	 * Surname of the lawyer
	 */
	@Schema(nullable = true)
	private String lawyerSurname;

	/**
	 * First given name of the lawyer
	 */
	@Schema(nullable = true)
	private String lawyerGivenName1;

	/**
	 * Second given name of the lawyer
	 */
	@Schema(nullable = true)
	private String lawyerGivenName2;

	/**
	 * Third given name of the lawyer
	 */
	@Schema(nullable = true)
	private String lawyerGivenName3;

	@Schema(nullable = true)
	private String justinRccId;

	/**
	 * The disputant requires spoken language interpreter. The language name is indicated in this field.
	 */
	@Schema(nullable = true)
	private String interpreterLanguageCd;

	/**
	 * Number of witness that the disputant intends to call.
	 */
	@Schema(nullable = true)
	private Integer witnessNo;

	/**
	 * The disputants attendance type.
	 */
	@Schema(nullable = true)
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
	public List<JJDisputeRemark> remarks = new ArrayList<JJDisputeRemark>();

	@JsonManagedReference(value = "jj_dispute_count_reference")
	public List<JJDisputedCount> jjDisputedCounts = new ArrayList<JJDisputedCount>();

	@JsonManagedReference(value = "jj_dispute_court_appearance_rop_reference")
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
