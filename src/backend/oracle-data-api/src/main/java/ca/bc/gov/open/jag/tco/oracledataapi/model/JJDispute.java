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
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.OneToMany;
import javax.persistence.OneToOne;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;

import com.fasterxml.jackson.annotation.JsonManagedReference;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * @author 237563
 * Represents a violation ticket dispute for JJ Workbench
 *
 */
//mark class as an Entity
@Entity
//defining class name as Table name
@Table
@Getter
@Setter
@Builder(toBuilder = true)
@NoArgsConstructor
@AllArgsConstructor
public class JJDispute extends Auditable<String>{

    /**
     * The violation ticket number as unique identifier.
     */
    @Id
    private String ticketNumber;
    
    @Enumerated(EnumType.STRING)
	private JJDisputeStatus status;
    
    @Enumerated(EnumType.STRING)
    private JJDisputeHearingType hearingType;
    
    /**
     * The date submitted by disputant in TCO.
     */
    @Column
    @Schema(nullable = true)
    @Temporal(TemporalType.TIMESTAMP)
    private Date submittedDate;

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
	 * The surname or corporate name of the disputant.
	 */
	@Column
	@Schema(nullable = true)
	private String surname;

    /**
     * The given name of the disputant.
     */
    @Column
    @Schema(nullable = true)
    private String givenNames;

    /**
     * The enforcement officer associated to the disputed violation ticket.
     */
    @Column
    @Schema(nullable = true)
    private String enforcementOfficer;

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
	 * Reason indicating why the disputant needs more time in order to make payment
	 * for the ticket.
	 */
	@Column(length = 256)
	@Schema(nullable = true)
	private String timeToPayReason;
	
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
	private Integer witnessNo;
	
	/**
	 * All the remarks for this jj dispute that are for internal use of JJs.
	 */
	@JsonManagedReference
	@OneToMany(targetEntity = JJDisputeRemark.class, cascade = CascadeType.ALL, fetch = FetchType.LAZY, orphanRemoval = true, mappedBy = "jjDispute")
	@Builder.Default
	private List<JJDisputeRemark> remarks = new ArrayList<JJDisputeRemark>();
	
	@JsonManagedReference
	@OneToOne(fetch = FetchType.LAZY, optional = true, cascade = CascadeType.ALL, orphanRemoval = true, mappedBy = "jjDispute")
	@Schema(nullable = true)
	private DisputantContactInformation contactInformation;
	
	@JsonManagedReference(value="jj_dispute_count_reference")
	@OneToMany(targetEntity = JJDisputedCount.class, cascade = CascadeType.ALL, fetch = FetchType.LAZY, orphanRemoval = true)
	@JoinColumn(name = "jjdispute_id")
	@Builder.Default
	private List<JJDisputedCount> jjDisputedCounts = new ArrayList<JJDisputedCount>();
	
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

	@JsonManagedReference(value="jj_dispute_court_appearance_reference")
	@OneToMany(targetEntity = JJDisputeCourtAppearanceRoP.class, cascade = CascadeType.ALL, fetch = FetchType.LAZY, orphanRemoval = true)
	@JoinColumn(name = "jjdispute_id")
	@Builder.Default
	private List<JJDisputeCourtAppearanceRoP> jjDisputeCourtAppearanceRoPs = new ArrayList<JJDisputeCourtAppearanceRoP>();
	
	public void addJJDisputeCourtAppearances(List<JJDisputeCourtAppearanceRoP> disputeCourtAppearanceRoPs) {
		for (JJDisputeCourtAppearanceRoP disputeCourtAppearanceRoP : disputeCourtAppearanceRoPs) {
			disputeCourtAppearanceRoP.setJjDispute(this);
		}
		this.jjDisputeCourtAppearanceRoPs.addAll(disputeCourtAppearanceRoPs);
	}
	
}
