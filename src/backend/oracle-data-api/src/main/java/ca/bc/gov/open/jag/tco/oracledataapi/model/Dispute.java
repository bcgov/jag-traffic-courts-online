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
import javax.persistence.OneToMany;
import javax.persistence.Table;
import javax.validation.constraints.Email;

import com.fasterxml.jackson.annotation.JsonManagedReference;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

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

    @Column
    private String ticketNumber;

    @Column
    private String courtLocation;

    @Column
    private Date violationDate;

    @Column
    private String disputantSurname;

    @Column
    private String givenNames;

    @Column
    private String streetAddress;

    @Column
    private String province;

    @Column
    private String postalCode;

    @Column
    private String homePhone;
    
    @Column
    @Email(regexp = ".+@.+\\..+")
    private String emailAddress;

    @Column
    private String driversLicense;

    @Column
    private String driversLicenseProvince;

    @Column
    private String workPhone;

    @Column
    private Date dateOfBirth;

    @Column
    private String enforcementOrganization;

    @Column
    private Date serviceDate;
    
    @JsonManagedReference
    @OneToMany(targetEntity=TicketCount.class, cascade = CascadeType.ALL, fetch = FetchType.LAZY, orphanRemoval = true)
    @JoinColumn(name="dispute_id")
    private List<TicketCount> ticketCounts = new ArrayList<TicketCount>();

    @Column
    private boolean lawyerRepresentation;

    @Column
    @Schema(nullable = true)
    private String interpreterLanguage;

    @Column
    private boolean witnessIntent;
    
    @Column
    @Schema(nullable = true)
    private String ocrViolationTicket;

    /**
     * A note or reason indicating why this Dispute has a status of REJECTED. This field is blank for other status types.
     */
    @Column(length = 256)
    @Schema(nullable = true)
    private String rejectedReason;
    
    public void addTicketCounts(List<TicketCount> ticketCounts) { 		
    	for (TicketCount ticketCount : ticketCounts) { 			
    		ticketCount.setDispute(this); 		
    	} 		
    	this.ticketCounts.addAll(ticketCounts); 	
    }

}
