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
	private Long id;

	@Enumerated(EnumType.STRING)
	private DisputeStatus status;

	/**
	 * The violation ticket number.
	 */
	@Column
	private String ticketNumber;

	/**
	 * The provincial court hearing location named on the violation ticket.
	 */
	@Column
	@Schema(nullable = true)
	private String provincialCourtHearingLocation;

	/**
	 * The date and time the violation ticket was issue. Time must only be hours and
	 * minutes.
	 */
	@Column
	@Temporal(TemporalType.TIMESTAMP)
	private Date issuedDate;

	/**
	 * The date and time the citizen was submitted the notice of dispute.
	 */
	@Column
	@Schema(nullable = true)
	@Temporal(TemporalType.TIMESTAMP)
	private Date submittedDate;

	/**
	 * The surname or corporate name.
	 */
	@Column(length = 30)
	private String surname;

	/**
	 * The given names or corporate name continued.
	 */
	@Column
	private String givenNames;

	/**
	 * The disputant's birthdate.
	 */
	@Column
	@Temporal(TemporalType.DATE)
	private Date birthdate;

	/**
	 * The drivers licence number. Note not all jurisdictions will use numeric
	 * drivers licence numbers.
	 */
	@Size(max = 20)
	@Column(length = 20)
	@Schema(maxLength = 20)
	private String driversLicenceNumber;

	/**
	 * The province or state the drivers licence was issued by.
	 */
	@Size(max = 30)
	@Column(length = 30)
	@Schema(maxLength = 30)
	private String driversLicenceProvince;

	/**
	 * The mailing address of the disputant.
	 */
	@Column(length = 500)
	private String address;

	/**
	 * The mailing address city of the disputant.
	 */
	@Column
	private String city;

	/**
	 * The mailing address province of the disputant.
	 */
	@Size(max = 30)
	@Column(length = 30)
	@Schema(maxLength = 30)
	private String province;

	/**
	 * The mailing address postal code or zip code of the disputant.
	 */
	@Size(max = 6)
	@Column(length = 10)
	@Schema(maxLength = 6)
	private String postalCode;
	
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private int addressCityCtryId;
	
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private int addressCitySeqNo;
	
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private int addressProvCtryId;
	
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private int addressProvSeqNo;

	/**
	 * The disputant's home phone number.
	 */
	@Column(length = 20)
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
	@Email(regexp = ".+@.+\\..+")
	private String emailAddress;

	@Column
	@Schema(nullable = true)
	@Temporal(TemporalType.TIMESTAMP)
	private Date filingDate;

	@JsonManagedReference
	@OneToMany(targetEntity = DisputedCount.class, cascade = CascadeType.ALL, fetch = FetchType.LAZY, orphanRemoval = true)
	@JoinColumn(name = "dispute_id")
	private List<DisputedCount> disputedCounts = new ArrayList<DisputedCount>();

	/**
	 * The disputant intends to be represented by a lawyer at the hearing.
	 */
	@Column
	private boolean representedByLawyer;

	/**
	 * The disputant requires spoken language interpreter. The language name is
	 * indicated in this field.
	 */
	@Column
	@Schema(nullable = true)
	private String interpreterLanguage;

	/**
	 * Number of witness that the disputant intends to call.
	 */
	@Column(length = 3)
	@Schema(nullable = true)
	private Integer numberOfWitness;

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
	 * A note or reason indicating why this Dispute has a status of REJECTED. This
	 * field is blank for other status types.
	 */
	@Column(length = 500)
	@Schema(nullable = true)
	private String rejectedReason;

	/**
	 * Identifier for whether the citizen has detected any issues with the OCR
	 * ticket result or not.
	 */
	@Column
	private boolean disputantDetectedOcrIssues;

	/**
	 * The description of the issue with OCR ticket if the citizen has detected any.
	 */
	@Column(length = 500)
	@Schema(nullable = true)
	private String disputantOcrIssuesDescription;

	/**
	 * Identifier for whether the system has detected any issues with the OCR ticket
	 * result or not.
	 */
	@Column
	private boolean systemDetectedOcrIssues;

	@Column(length  = 30)
	@Schema(nullable = true)
	private String jjAssigned;

	/**
	 * All OCR Violation ticket data serialized into a JSON string.
	 */
	@Column
	@Lob
	@Schema(nullable = true)
	private String ocrViolationTicket;

	/**
	 * The IDIR of the Staff whom the dispute is assigned to be reviewed on Staff
	 * Portal.
	 */
	@Column
	@Schema(nullable = true)
	private String assignedTo;

	/**
	 * The date and time a dispute was assigned to a Staff to be reviewed.
	 */
	@Column
	@Schema(nullable = true)
	@Temporal(TemporalType.TIMESTAMP)
	private Date assignedTs;
	
	/**
	 * Detachment location
	 */
	@Column(length = 150)
	@Schema(nullable = true)
	private String detachmentLocation;
	
	/**
	 * Disputant client ID
	 */
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private String disputantClientId;
	
	/**
	 * Disputant comment
	 */
	@Column(length = 4000)
	@Schema(nullable = true)
	private String disputantComment;
	
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
	 * Name of the organization of the disputant
	 */
	@Column(length = 150)
	@Schema(nullable = true)
	private String disputantOrganization;
	
	/**
	 * Country id of the issued identity
	 */
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private int identIssuedCtryId;
	
	/**
	 * Province seq no of the issued identity
	 */
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private int identIssuedProvSeqNo;
	
	/**
	 * Indicates that whether an interpreter is required by the disputant or not
	 */
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private String interpreterRequiredYN;
	
	/**
	 * Language code
	 */
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private String languageCd;
	
	//Legal Representation Section
	
	/**
	 * Indicates that whether the disputant is represented by a lawyer or not
	 */
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private String representedByLawyerYN;
	
	/**
	 * Name of the law firm that will represent the disputant at the hearing.
	 */
	@Column(length = 200)
	@Schema(nullable = true)
	private String lawFirmName;
	
	/**
	 * Full name of the lawyer who will represent the disputant at the hearing.
	 */
	@Column
	@Schema(nullable = true)
	private String lawyerFullName;
	
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
	 * Surname of the lawyer
	 */
	@Column(length = 30)
	@Schema(nullable = true)
	private String lawyerSurname;
	
	/**
	 * Email address of the lawyer who will represent the disputant at the hearing.
	 */
	@Column
	@Schema(nullable = true)
	@Email(regexp = ".+@.+\\..+")
	private String lawyerEmail;
	
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
	
	// End of Legal Representation Section
	
	/**
	 * Officer Pin
	 */
	@Column(length = 10)
	@Schema(nullable = true)
	private String officerPin;
	
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private Float courtAgenId;

	@JsonManagedReference
	@OneToOne(fetch = FetchType.LAZY, optional = true, cascade = CascadeType.ALL, orphanRemoval = true, mappedBy = "dispute")
	@Schema(nullable = true)
	private ViolationTicket violationTicket;
	
	@JsonBackReference
	@ManyToOne(targetEntity=DisputeStatusType.class, fetch = FetchType.LAZY)
	@Schema(hidden = true)
	private DisputeStatusType statusType;

	public void addDisputedCounts(List<DisputedCount> disputedCounts) {
		for (DisputedCount disputedCount : disputedCounts) {
			disputedCount.setDispute(this);
		}
		this.disputedCounts.addAll(disputedCounts);
	}

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
