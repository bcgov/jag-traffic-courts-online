package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.sql.Date;
import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

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
import javax.persistence.OneToMany;
import javax.persistence.OneToOne;
import javax.persistence.Table;
import javax.validation.constraints.Email;

import com.fasterxml.jackson.annotation.JsonManagedReference;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;


/**
 * @author 237563
 * Represents a violation ticket notice of dispute.
 *
 */
//mark class as an Entity
@Entity
//defining class name as Table name
@Table
@Getter
@Setter
@NoArgsConstructor
public class Dispute {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
    @Id
    @GeneratedValue
    private UUID id;

    @Enumerated(EnumType.STRING)
    private DisputeStatus status;

    /**
     * The violation ticket number.
     */
    @Column
    @Schema(nullable = true)
    private String ticketNumber;

    /**
     * The provincial court hearing location named on the violation ticket.
     */
    @Column
    @Schema(nullable = true)
    private String provincialCourtHearingLocation;

    /**
     * The date and time the violation ticket was issue. Time must only be hours and minutes.
     */
    @Column
    @Schema(nullable = true)
    private Date issuedDate;
    
    /**
     * The date and time the citizen was submitted the notice of dispute.
     */
    @Column
    @Schema(nullable = true)
    private Date citizenSubmittedDate;

    /**
     * The surname or corporate name.
     */
    @Column
    @Schema(nullable = true)
    private String surname;

    /**
     * The given names or corporate name continued.
     */
    @Column
    @Schema(nullable = true)
    private String givenNames;

    /**
     * The mailing address of the disputant.
     */
    @Column
    @Schema(nullable = true)
    private String address;
    
    /**
     * The mailing address city of the disputant.
     */
    @Column
    @Schema(nullable = true)
    private String city;

    /**
     * The mailing address province of the disputant.
     */
    @Column
    @Schema(nullable = true)
    private String province;

    /**
     * The mailing address postal code or zip code of the disputant.
     */
    @Column
    @Schema(nullable = true)
    private String postalCode;

    /**
     * The disputant's home phone number.
     */
    @Column
    @Schema(nullable = true)
    private String homePhoneNumber;
    
    /**
     * The disputant's work phone number.
     */
    @Column
    @Schema(nullable = true)
    private String workPhoneNumber;
    
    /**
     * The disputant's email address.
     */
    @Column
    @Email(regexp = ".+@.+\\..+")
    @Schema(nullable = true)
    private String emailAddress;
    
    @Column
    @Schema(nullable = true)
    private Date filingDate;
    
    @JsonManagedReference
    @OneToMany(targetEntity=DisputedCount.class, cascade = CascadeType.ALL, fetch = FetchType.LAZY, orphanRemoval = true)
    @JoinColumn(name="dispute_id")
    private List<DisputedCount> disputedCounts = new ArrayList<DisputedCount>();

    /**
     * The disputant intends to be represented by a lawyer at the hearing.
     */
    @Column
    private boolean representedByLawyer;
    
    @JsonManagedReference
    @OneToOne(fetch = FetchType.LAZY, optional = true, cascade = CascadeType.ALL, orphanRemoval = true, mappedBy = "dispute")
    @Schema(nullable = true)
    private LegalRepresentation legalRepresentation;

    /**
     * The disputant requires spoken language interpreter. The language name is indicated in this field.
     */
    @Column
    @Schema(nullable = true)
    private String interpreterLanguage;

    /**
     * Number of witness that the disputant intends to call.
     */
    @Column
    @Schema(nullable = true)
    private Integer numberOfWitness;
    
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

    /**
     * A note or reason indicating why this Dispute has a status of REJECTED. This field is blank for other status types.
     */
    @Column(length = 256)
    @Schema(nullable = true)
    private String rejectedReason;
    
    @Column
    private boolean citizenDetectedOcrIssues;
    
    @Column
    @Schema(nullable = true)
    private String citizenOcrIssuesDescription;
    
    @Column
    private boolean systemDetectedOcrIssues;
    
    @Column
    @Schema(nullable = true)
    private String jjAssigned;
    
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
    private ViolationTicket violationTicket;
    
    public void addDisputedCounts(List<DisputedCount> disputedCounts) { 		
    	for (DisputedCount disputedCount : disputedCounts) { 			
    		disputedCount.setDispute(this); 		
    	} 		
    	this.disputedCounts.addAll(disputedCounts); 	
    }
    
    public void setLegalRepresentation(LegalRepresentation legal) {
        if (legal == null) {
            if (this.legalRepresentation != null) {
                this.legalRepresentation.setDispute(null);
            }
        }
        else {
            legal.setDispute(this);
        }
        this.legalRepresentation = legal;
    }
    
    public void setViolationTicket(ViolationTicket ticket) {
        if (ticket == null) {
            if (this.violationTicket != null) {
                this.violationTicket.setDispute(null);
            }
        }
        else {
        	ticket.setDispute(this);
        }
        this.violationTicket = ticket;
    }

}
