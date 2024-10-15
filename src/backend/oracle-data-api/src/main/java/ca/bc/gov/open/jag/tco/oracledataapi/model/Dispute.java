package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import jakarta.validation.constraints.Size;

import com.fasterxml.jackson.annotation.JsonBackReference;
import com.fasterxml.jackson.annotation.JsonManagedReference;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * @author 237563 Represents a violation ticket notice of dispute.
 *
 */
//mark class as an Entity
//defining class name as Table name
@Getter
@Setter
@NoArgsConstructor
public class Dispute extends Auditable<String> {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	private Long disputeId;

	/**
	 * A unique string (GUID) for this Dispute.
	 */
	@Schema(nullable = true) // FIXME: this field should not be nullable (temp nullable for now to get things working).
	private String noticeOfDisputeGuid;

	/**
	 * The violation ticket number.
	 */
	@Schema(nullable = false)
	private String ticketNumber;

	/**
	 * The date and time the violation ticket was issue. Time must only be hours and
	 * minutes. This should always be in UTC date-time (ISO 8601) format
	 */
	@Schema(nullable=true)
	private Date issuedTs;

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

	/**
	 * The disputant's birthdate.
	 */
	@Schema(nullable = true)
	private Date disputantBirthdate;

	/**
	 * The drivers licence number. Note not all jurisdictions will use numeric
	 * drivers licence numbers.
	 */
	@Size(max = 30)
	@Schema(maxLength = 30, nullable = true)
	private String driversLicenceNumber;

	/**
	 * Name of the organization of the disputant
	 */
	@Schema(nullable = true)
	private String disputantOrganization;

	/**
	 * Disputant client ID
	 */
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private String disputantClientId;

	/**
	 * The country code of the drivers licence was issued by.
	 */
	@Schema(nullable = true)
	private Integer driversLicenceIssuedCountryId;

	/**
	 * The province sequence number of the drivers licence was issued by.
	 */
	@Schema(nullable = true)
	private Integer driversLicenceIssuedProvinceSeqNo;

	/**
	 * The province or state the drivers licence was issued by.
	 */
	@Size(max = 30)
	@Schema(maxLength = 30, nullable = true)
	private String driversLicenceProvince;

	private DisputeStatus status;

	/**
	 * The mailing address line one of the disputant.
	 */
	@Schema(nullable = true)
	private String addressLine1;

	/**
	 * The mailing address line two of the disputant.
	 */
	@Schema(nullable = true)
	private String addressLine2;

	/**
	 * The mailing address line three of the disputant.
	 */
	@Schema(nullable = true)
	private String addressLine3;

	/**
	 * The mailing address city of the disputant.
	 */
	@Size(max = 30)
	@Schema(nullable = true)
	private String addressCity;

	/**
	 * The mailing address province of the disputant.
	 */
	@Size(max = 30)
	@Schema(maxLength = 30, nullable = true)
	private String addressProvince;

	/**
	 * The mailing address province's country code of the disputant.
	 */
	@Schema(nullable = true)
	private Integer addressProvinceCountryId;

	/**
	 * The mailing address province's sequence number of the disputant.
	 */
	@Schema(nullable = true)
	private Integer addressProvinceSeqNo;

	/**
	 * The mailing address country id of the disputant.
	 */
	@Schema(nullable = true)
	private Integer addressCountryId;

	/**
	 * The mailing address postal code or zip code of the disputant.
	 */
	@Size(max = 10)
	@Schema(maxLength = 10, nullable = true) // needs to be nullable to provide a patch update (which may or may not include a surname)
	private String postalCode;

	/**
	 * The disputant's home phone number.
	 */
	@Schema(nullable = true)
	private String homePhoneNumber;

	/**
	 * The disputant's work phone number.
	 */
	@Schema(nullable = true)
	private String workPhoneNumber;

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

	//Legal Representation Section

	/**
	 * The disputant intends to be represented by a lawyer at the hearing.
	 */
	@Schema(nullable = true)
	private YesNo representedByLawyer;

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


	/**
	 * Address of the lawyer who will represent the disputant at the hearing.
	 */
	@Size(max = 300)
	@Schema(maxLength = 300, nullable = true)
	private String lawyerAddress;

	/**
	 * Phone number of the lawyer who will represent the disputant at the hearing.
	 */
	@Schema(nullable = true)
	private String lawyerPhoneNumber;

	/**
	 * Email address of the lawyer who will represent the disputant at the hearing.
	 */
	@Size(max = 100)
	@Schema(nullable = true)
	private String lawyerEmail;

	// End of Legal Representation Section

	/**
	 * Officer Pin
	 */
	@Schema(nullable = true)
	private String officerPin;

	@Schema(nullable = false)
	private ContactType contactTypeCd;

	@Schema(nullable = true)
	private YesNo requestCourtAppearanceYn;

	/** Can only be specified if ContantType is L */
	@Schema(nullable = true)
	private String contactLawFirmNm;

	/** Can only be specified if ContantType is L or O */
	@Schema(nullable = true)
	private String contactGiven1Nm;

	/** Can only be specified if ContantType is L or O */
	@Schema(nullable = true)
	private String contactGiven2Nm;

	/** Can only be specified if ContantType is L or O */
	@Schema(nullable = true)
	private String contactGiven3Nm;

	/** Can only be specified if ContantType is L or O */
	@Schema(nullable = true)
	private String contactSurnameNm;

	@Schema(nullable = true)
	private Date appearanceDtm;

	@Schema(nullable = true)
	private YesNo appearanceLessThan14DaysYn;

	/**
	 * Detachment location
	 */
	@Schema(nullable = true)
	private String detachmentLocation;

	/**
	 * Court Agency Id
	 */
	@Schema(nullable = true)
	private String courtAgenId;

	/**
	 * The disputant requires spoken language interpreter. The language name is
	 * indicated in this field.
	 */
	@Size(max = 3)
	@Schema(maxLength = 3, nullable = true)
	private String interpreterLanguageCd;

	/**
	 * Indicates that whether an interpreter is required by the disputant or not
	 */
	@Schema(nullable = true)
	private YesNo interpreterRequired;

	/**
	 * Number of witness that the disputant intends to call.
	 */
	@Schema(nullable = true)
	private Integer witnessNo;

	/**
	 * Reason indicating why the fine reduction requested for the ticket.
	 */
	@Schema(nullable = true)
	private String fineReductionReason;

	/**
	 * Reason indicating why the disputant needs more time in order to make payment
	 * for the ticket.
	 */
	@Schema(nullable = true)
	private String timeToPayReason;

	/**
	 * Disputant comment
	 */
	@Schema(nullable = true)
	private String disputantComment;

	/**
	 * A note or reason indicating why this Dispute has a status of REJECTED. This
	 * field is blank for other status types.
	 */
	@Schema(nullable = true)
	private String rejectedReason;

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
	 * The description of the issue with OCR ticket if the citizen has detected any.
	 */
	@Schema(nullable = true)
	private String disputantOcrIssues;

	/**
	 * Identifier for whether the system has detected any issues with the OCR ticket
	 * result or not.
	 */
	@Schema(nullable= false)
	private YesNo systemDetectedOcrIssues;

	/**
	 * Filename of JSON serialized OCR data that is saved in object storage.
	 */
	@Size(max = 100)
	@Schema(maxLength = 100, nullable = true)
	private String ocrTicketFilename;

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

	@JsonManagedReference
	@Schema(nullable = true)
	private ViolationTicket violationTicket;

	@JsonBackReference
	@Schema(hidden = true)
	private DisputeStatusType disputeStatusType;

	@JsonManagedReference(value="dispute_count_reference")
	private List<DisputeCount> disputeCounts = new ArrayList<DisputeCount>();

	public void setViolationTicket(ViolationTicket ticket) {
		if (ticket == null) {
			if (this.violationTicket != null) {
				this.violationTicket.setDispute(null);
			}
		} else {
			ticket.setDispute(this);
		}
		this.violationTicket = ticket;
	}

	public void addDisputeCounts(List<DisputeCount> disputeCounts) {
		for (DisputeCount disputeCount : disputeCounts) {
			disputeCount.setDispute(this);
		}
		this.disputeCounts.addAll(disputeCounts);
	}
}
