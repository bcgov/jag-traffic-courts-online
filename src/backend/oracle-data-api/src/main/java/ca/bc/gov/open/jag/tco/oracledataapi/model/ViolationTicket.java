package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.sql.Date;
import java.util.ArrayList;
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
import javax.persistence.OneToOne;
import javax.persistence.Table;
import javax.validation.constraints.Size;

import com.fasterxml.jackson.annotation.JsonBackReference;
import com.fasterxml.jackson.annotation.JsonManagedReference;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * @author 237563
 * Represents the data contained on a BC violation ticket
 *
 */
//mark class as an Entity
@Entity
//defining class name as Table name
@Table
@Getter
@Setter
@NoArgsConstructor
public class ViolationTicket extends Auditable<String> {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
    @Id
    @GeneratedValue
    private Long violationTicketId;

    /**
     * The violation ticket number.
     */
    @Column(length = 50)
    @Schema(nullable = true)
    private String ticketNumber;
    
    /**
	 * Name of the organization of the disputant
	 */
	@Column(length = 150)
	@Schema(nullable = true)
	private String disputantOrganizationName;

    /**
     * The surname or corporate name.
     */
    @Column(length = 100)
    @Schema(nullable = true)
    private String disputantSurname;

    /**
     * The given names or corporate name continued.
     */
    @Column(length = 200)
    @Schema(nullable = true)
    private String disputantGivenNames;

   /**
	 * The person issued the ticket has been identified as a young person.
	 */
	@Schema(nullable = true)
	@Enumerated(EnumType.STRING)
    private YesNo isYoungPerson;

    /**
     * The drivers licence number. Note not all jurisdictions will use numeric drivers licence numbers.
     */
    @Size(min = 7, max = 30)
    @Column(length = 30)
    @Schema(nullable = true, minLength = 7, maxLength = 30)
    private String disputantDriversLicenceNumber;
    
    /**
	 * Disputant client number
	 */
	@Column(length = 30)
	@Schema(nullable = true)
	private String disputantClientNumber;
	
	 /**
     * The province or state the drivers licence was issued by.
     */
    @Column
    @Schema(nullable = true)
    private String driversLicenceProvince;
    
    /**
     * The year the drivers licence was issued.
     */
    @Column(length = 4)
    @Schema(nullable = true)
    private Integer driversLicenceIssuedYear;

    /**
     * The year the drivers licence expires.
     */
    @Column(length = 4)
    @Schema(nullable = true)
    private Integer driversLicenceExpiryYear;

     /**
     * The birthdate of the individual the violation ticket was issued to.
     */
    @Column
    @Schema(nullable = true)
    private Date disputantBirthdate;

    /**
     * The address of the individual the violation ticket was issued to.
     */
    @Column
    @Schema(nullable = true)
    private String address;

    /**
     * The city of the individual the violation ticket was issued to.
     */
    @Column(length = 100)
    @Schema(nullable = true)
    private String addressCity;

    /**
     * The province or state of the individual the violation ticket was issued to.
     */
    @Column(length = 100)
    @Schema(nullable = true)
    private String addressProvince;

    /**
     * The postal code or zip code.
     */
    @Column(length = 10)
    @Schema(nullable = true)
    private String addressPostalCode;
    
    /**
     * The address country.
     */
    @Column(length = 100)
    @Schema(nullable = true)
    private String addressCountry;
    
    /**
	 * Officer Pin
	 */
	@Column(length = 10)
	@Schema(nullable = true)
	private String officerPin;
   
	/**
     * Detachment location.
     */
    @Column(length = 150)
    @Schema(nullable = true)
    private String detachmentLocation;
    
    /**
     * The date and time the violation ticket was issue. Time must only be hours and minutes.
     */
    @Column
    @Schema(nullable = true)
    private Date issuedDate;

    /**
     * The violation ticket was issued on this road or highway.
     */
    @Column(length = 100)
    @Schema(nullable = true)
    private String issuedOnRoadOrHighway;

    @Column(length = 100)
    @Schema(nullable = true)
    private String issuedAtOrNearCity;
     
    /**
	 * The address represents a change of address. The address on the violation would be different from the address on the drivers licence or provided identification.
	 */
	@Schema(nullable = true)
	@Enumerated(EnumType.STRING)
	private YesNo isChangeOfAddress;

    /**
	 * The enforcement officer says that he or she has reasonable grounds to believe, and does believe, that the above named as the vehicle driver.
	 */
	@Schema(nullable = true)
	@Enumerated(EnumType.STRING)
	private YesNo isDriver;

     /**
	 * The enforcement officer says that he or she has reasonable grounds to believe, and does believe, that the above named as the vehicle owner.
	 */
	@Schema(nullable = true)
	@Enumerated(EnumType.STRING)
	private YesNo isOwner;

    /**
     * Court location.
     */
    @Column(length = 150)
    @Schema(nullable = true)
    private String courtLocation;
    
    @JsonManagedReference
    @OneToMany(targetEntity=ViolationTicketCount.class, cascade = CascadeType.ALL, fetch = FetchType.LAZY, orphanRemoval = true)
    @JoinColumn(name="violation_ticket_id", referencedColumnName="violationTicketId")
    private List<ViolationTicketCount> violationTicketCounts = new ArrayList<ViolationTicketCount>();

    @JsonBackReference
	@OneToOne
	@JoinColumn(name = "dispute_id", referencedColumnName = "disputeId")
	@Schema(hidden = true)
	private Dispute dispute;
}
