package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.sql.Date;
import java.util.ArrayList;
import java.util.List;
import javax.persistence.CascadeType;
import javax.persistence.Column;
import javax.persistence.Entity;
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
    private Long id;

    /**
     * The violation ticket number.
     */
    @Column
    @Schema(nullable = true)
    private String ticketNumber;

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
     * The person issued the ticket has been identified as a young person.
     */
    @Column
    @Schema(nullable = true)
    private Boolean isYoungPerson;
    
    /**
	 * The person issued the ticket has been identified as a young person.
	 */
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private String isYoungPersonYN;

    /**
     * The drivers licence number. Note not all jurisdictions will use numeric drivers licence numbers.
     */
    @Size(min = 7, max = 30)
    @Column(length = 30)
    @Schema(nullable = true, minLength = 7, maxLength = 30)
    private String driversLicenceNumber;

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
     * The country of the identity issued in.
     */
    @Column(length = 100)
    @Schema(nullable = true)
    private String identityIssuedCountry;
    
    /**
     * The province of the identity issued in.
     */
    @Column(length = 100)
    @Schema(nullable = true)
    private String identityIssuedProvince;

    /**
     * The birthdate of the individual the violation ticket was issued to.
     */
    @Column
    @Schema(nullable = true)
    private Date birthdate;

    /**
     * The address of the individual the violation ticket was issued to.
     */
    @Column
    @Schema(nullable = true)
    private String address;

    /**
     * The city of the individual the violation ticket was issued to.
     */
    @Column
    @Schema(nullable = true)
    private String city;

    /**
     * The province or state of the individual the violation ticket was issued to.
     */
    @Column
    @Schema(nullable = true)
    private String province;

    /**
     * The postal code or zip code.
     */
    @Column
    @Schema(nullable = true)
    private String postalCode;

    /**
     * The address represents a change of address. The address on the violation would be different from the address on the drivers licence or provided identification.
     */
    @Column
    @Schema(nullable = true)
    private Boolean isChangeOfAddress;
    
    /**
	 * The address represents a change of address. The address on the violation would be different from the address on the drivers licence or provided identification.
	 */
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private String isChangeOfAddressYN;

    /**
     * The enforcement officer says that he or she has reasonable grounds to believe, and does believe, that the above named as the vehicle driver.
     */
    @Column
    @Schema(nullable = true)
    private Boolean isDriver;
    
    /**
	 * The enforcement officer says that he or she has reasonable grounds to believe, and does believe, that the above named as the vehicle driver.
	 */
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private String isDriverYN;

    /**
     * The enforcement officer says that he or she has reasonable grounds to believe, and does believe, that the above named as the cyclist.
     */
    @Column
    @Schema(nullable = true)
    private Boolean isCyclist;

    /**
     * The enforcement officer says that he or she has reasonable grounds to believe, and does believe, that the above named as the vehicle owner.
     */
    @Column
    @Schema(nullable = true)
    private Boolean isOwner;
    
    /**
	 * The enforcement officer says that he or she has reasonable grounds to believe, and does believe, that the above named as the vehicle owner.
	 */
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private String isOwnerYN;

    /**
     * The enforcement officer says that he or she has reasonable grounds to believe, and does believe, that the above named as a pedestrain.
     */
    @Column
    @Schema(nullable = true)
    private Boolean isPedestrian;

    /**
     * The enforcement officer says that he or she has reasonable grounds to believe, and does believe, that the above named as a passenger.
     */
    @Column
    @Schema(nullable = true)
    private Boolean isPassenger;

    /**
     * The enforcement officer says that he or she has reasonable grounds to believe, and does believe, that the above named as a other designation.
     */
    @Column
    @Schema(nullable = true)
    private Boolean isOther;

    /**
     * If TrafficCourts.Models.ViolationTicket.IssuedToOther is true, the other designation description.
     */
    @Column
    @Schema(nullable = true)
    private String otherDescription;

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

    @Column
    @Schema(nullable = true)
    private String issuedAtOrNearCity;
    
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private String issuedAtOrNearCityYN;

    /**
     * The violation ticket was issued for offence under the Motor Vehicle Act (MVA).
     */
    @Column
    @Schema(nullable = true)
    private Boolean isMvaOffence;

    /**
     * The violation ticket was issued for offence under the Wildlife Act (WLA).
     */
    @Column
    @Schema(nullable = true)
    private Boolean isWlaOffence;

    /**
     * The violation ticket was issued for offence under the Liquor Control and Licencing Act (LCA).
     */
    @Column
    @Schema(nullable = true)
    private Boolean isLcaOffence;

    /**
     * The violation ticket was issued for offence under the Motor Carrier Act (MCA).
     */
    @Column
    @Schema(nullable = true)
    private Boolean isMcaOffence;

    /**
     * The violation ticket was issued for offence under the Firearm Act (FAA).
     */
    @Column
    @Schema(nullable = true)
    private Boolean isFaaOffence;

    /**
     * The violation ticket was issued for offence under the Transit Conduct and Safety Regs (TCR).
     */
    @Column
    @Schema(nullable = true)
    private Boolean isTcrOffence;

    /**
     * The violation ticket was issued for offence under the Commercial Transport Act (CTA).
     */
    @Column
    @Schema(nullable = true)
    private Boolean isCtaOffence;

    /**
     * The violation ticket was issued for other.
     */
    @Column
    @Schema(nullable = true)
    private Boolean isOtherOffence;

    /**
     * The violation ticket was issued for other.
     */
    @Column
    @Schema(nullable = true)
    private String otherOffenceDescription;

    /**
     * Issuing organization.
     */
    @Column
    @Schema(nullable = true)
    private String organizationLocation;
    
    /**
     * Court location.
     */
    @Column(length = 150)
    @Schema(nullable = true)
    private String courtLocation;
    
    /**
     * Detachment location.
     */
    @Column(length = 150)
    @Schema(nullable = true)
    private String detachmentLocation;
    
    /**
	 * Disputant client number
	 */
	@Column(length = 30)
	@Schema(nullable = true)
	private String disputantClientNumber;
	
	/**
	 * Name of the organization of the disputant
	 */
	@Column(length = 150)
	@Schema(nullable = true)
	private String disputantOrganization;
	
	/**
	 * Officer Pin
	 */
	@Column(length = 10)
	@Schema(nullable = true)
	private String officerPin;
    
    @JsonManagedReference
    @OneToMany(targetEntity=ViolationTicketCount.class, cascade = CascadeType.ALL, fetch = FetchType.LAZY, orphanRemoval = true)
    @JoinColumn(name="violation_ticket_id")
    private List<ViolationTicketCount> violationTicketCounts = new ArrayList<ViolationTicketCount>();

    @JsonBackReference
	@OneToOne
	@JoinColumn(name = "dispute_id", referencedColumnName = "id")
	@Schema(hidden = true)
	private Dispute dispute;

}
