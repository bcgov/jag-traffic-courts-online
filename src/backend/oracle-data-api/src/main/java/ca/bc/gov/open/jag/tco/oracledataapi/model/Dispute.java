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
import javax.persistence.Lob;
import javax.persistence.ManyToOne;
import javax.persistence.OneToMany;
import javax.persistence.OneToOne;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;
import javax.validation.constraints.Email;
import javax.validation.constraints.Size;

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
@Entity
//defining class name as Table name
@Table
@Getter
@Setter
@NoArgsConstructor
public class Dispute extends Auditable<String> {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	@Id
	@GeneratedValue
	private Long disputeId;
	
	/**
     * Court location.
     */
    @Column(length = 150)
    @Schema(nullable = true)
    private String courtLocation;
    
    /**
	 * The date and time the violation ticket was issue. Time must only be hours and
	 * minutes.
	 */
	@Column
	@Temporal(TemporalType.TIMESTAMP)
	@Schema(nullable=true)
	private Date issuedDate;

	/**
	 * The date and time the citizen submitted the notice of dispute.
	 */
	@Column
	@Schema(nullable = true)
	@Temporal(TemporalType.TIMESTAMP)
	private Date submittedDate;

	/**
	 * The surname or corporate name.
	 */
	@Column(length = 30)
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
	
	/**
	 * The disputant's birthdate.
	 */
	@Column
	@Schema(nullable = true)
	@Temporal(TemporalType.DATE)
	private Date disputantBirthdate;
	
	/**
	 * The drivers licence number. Note not all jurisdictions will use numeric
	 * drivers licence numbers.
	 */
	@Size(max = 30)
	@Column(length = 30)
	@Schema(maxLength = 30, nullable = true)
	private String driversLicenceNumber;
	
	/**
	 * Name of the organization of the disputant
	 */
	@Column(length = 150)
	@Schema(nullable = true)
	private String disputantOrganization;
	
	/**
	 * Disputant client ID
	 */
	@Column(length = 30)
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private String disputantClientId;
	
	/**
	 * The province or state the drivers licence was issued by.
	 */
	@Size(max = 30)
	@Column(length = 30)
	@Schema(maxLength = 30, nullable = true)
	private String driversLicenceProvince;
	
	@Enumerated(EnumType.STRING)
	private DisputeStatus status;

	/**
	 * The mailing address of the disputant.
	 */
	@Column(length = 500)
	@Schema(nullable = true)
	private String address;

	/**
	 * The mailing address city of the disputant.
	 */
	@Column
	@Schema(nullable = true)
	private String addressCity;

	/**
	 * The mailing address province of the disputant.
	 */
	@Size(max = 30)
	@Column(length = 30)
	@Schema(maxLength = 30, nullable = true)
	private String addressProvince;

	/**
	 * The mailing address postal code or zip code of the disputant.
	 */
	@Size(max = 10)
	@Column(length = 10)
	@Schema(maxLength = 10)
	private String postalCode;
	
	/**
	 * The disputant's home phone number.
	 */
	@Column(length = 20)
	@Schema(nullable = true)
	private String homePhoneNumber;

	/**
	 * The disputant's work phone number.
	 */
	@Column(length = 20)
	@Schema(nullable = true)
	private String workPhoneNumber;

	/**
	 * The disputant's email address.
	 */
	@Column(length = 100)
	@Schema(nullable = true)
	@Email(regexp = "^(.+)@(\\S+)$." ) /* https://xperti.io/blogs/java-email-address-validate-method/#:~:text=The%20most%20basic%20regular%20expression%20to%20validate%20an,of%20the%20%E2%80%98%40%E2%80%99%20symbol%20in%20the%20email%20address */
	private String emailAddress;

	@Column
	@Schema(nullable = true)
	@Temporal(TemporalType.TIMESTAMP)
	private Date filingDate;

	//Legal Representation Section

	/**
	 * The disputant intends to be represented by a lawyer at the hearing.
	 */
	@Column
	@Enumerated(EnumType.STRING)
	@Schema(nullable = true)
    private YesNo representedByLawyer;
	
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
	 * Address of the lawyer who will represent the disputant at the hearing.
	 */
	@Column(length = 200)
	@Schema(nullable = true)
	private String lawyerAddress;
	
	/**
	 * Phone number of the lawyer who will represent the disputant at the hearing.
	 */
	@Column(length = 20)
	@Schema(nullable = true)
	private String lawyerPhoneNumber;
		
	/**
	 * Email address of the lawyer who will represent the disputant at the hearing.
	 */
	@Column
	@Schema(nullable = true)
	@Email(regexp = "^(.+)@(\\S+)$." ) /* https://xperti.io/blogs/java-email-address-validate-method/#:~:text=The%20most%20basic%20regular%20expression%20to%20validate%20an,of%20the%20%E2%80%98%40%E2%80%99%20symbol%20in%20the%20email%20address */
	private String lawyerEmail;
	
	// End of Legal Representation Section
	
	/**
	 * Officer Pin
	 */
	@Column(length = 10)
	@Schema(nullable = true)
	private String officerPin;
	
	/**
	 * Detachment location
	 */
	@Column(length = 150)
	@Schema(nullable = true)
	private String detachmentLocation;
	
	
	/**
	 * The disputant requires spoken language interpreter. The language name is
	 * indicated in this field.
	 */
	@Column
	@Schema(nullable = true)
	private String interpreterLanguage;
	
	/**
	 * Indicates that whether an interpreter is required by the disputant or not
	 */
	@Schema(nullable = true)
	@Enumerated(EnumType.STRING)
    private YesNo interpreterRequired;
	
	/**
	 * Number of witness that the disputant intends to call.
	 */
	@Column(length = 3)
	@Schema(nullable = true)
	private Integer witnessNo;

	/**
	 * Reason indicating why the fine reduction requested for the ticket.
	 */
	@Column(length = 500)
	@Schema(nullable = true)
	private String fineReductionReason;

	/**
	 * Reason indicating why the disputant needs more time in order to make payment
	 * for the ticket.
	 */
	@Column(length = 500)
	@Schema(nullable = true)
	private String timeToPayReason;
	
	/**
	 * Disputant comment
	 */
	@Column(length = 4000)
	@Schema(nullable = true)
	private String disputantComment;
	
	/**
	 * A note or reason indicating why this Dispute has a status of REJECTED. This
	 * field is blank for other status types.
	 */
	@Column(length = 500)
	@Schema(nullable = true)
	private String rejectedReason;
	
	/**
	 * The IDIR of the Staff whom the dispute is assigned to be reviewed on Staff
	 * Portal.
	 */
	@Column(length = 30)
	@Schema(nullable = true)
	private String userAssignedTo;

	/**
	 * The date and time a dispute was assigned to a Staff to be reviewed.
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
	@Schema(nullable = false)
	@Enumerated(EnumType.STRING)
    private YesNo disputantDetectedOcrIssues;

	/**
	 * The description of the issue with OCR ticket if the citizen has detected any.
	 */
	@Column(length = 500)
	@Schema(nullable = true)
	private String disputantOcrIssues;

	/**
	 * Identifier for whether the system has detected any issues with the OCR ticket
	 * result or not.
	 */
	@Column
	@Schema(nullable= false)
	@Enumerated(EnumType.STRING)
    private YesNo systemDetectedOcrIssues;

	/**
	 * All OCR Violation ticket data serialized into a JSON string.
	 */
	@Column
	@Lob
	@Schema(nullable = true)
	private String ocrViolationTicket;

	@JsonManagedReference
	@OneToOne(fetch = FetchType.LAZY, optional = true, cascade = CascadeType.ALL, orphanRemoval = true, mappedBy = "dispute")
	@Schema(nullable = true)
	@JoinColumn(name = "dispute_id", referencedColumnName="disputeId")
	private ViolationTicket violationTicket;
	
	@JsonBackReference
	@ManyToOne(targetEntity=DisputeStatusType.class, fetch = FetchType.LAZY)
	@Schema(hidden = true)
	@JoinColumn(name = "dispute_status_type_cd", referencedColumnName="disputeStatusTypeCode")
	private DisputeStatusType disputeStatusType;

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

}
