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
import javax.validation.constraints.Size;

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
	 * The ID of the jj group whom the dispute is assigned to be listed on JJ Workbench.
	 */
	@Column
	@Schema(nullable = true)
	private String jjGroupAssignedTo;
	
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
	 * A note/free form text field that is for internal use of JJs.
	 */
	@Size(max = 500)
	@Column(length = 500)
	@Schema(nullable = true, maxLength = 500)
	private String remarks;
	
	@JsonManagedReference
	@OneToOne(fetch = FetchType.LAZY, optional = true, cascade = CascadeType.ALL, orphanRemoval = true, mappedBy = "jjDispute")
	@Schema(nullable = true)
	private DisputantContactInformation contactInformation;
	
	@JsonManagedReference
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

}
