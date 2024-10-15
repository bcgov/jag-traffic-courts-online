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
 * @author 237563
 * Represents the data contained on a BC violation ticket
 *
 */
//mark class as an Entity
@Getter
@Setter
@NoArgsConstructor
public class ViolationTicket extends Auditable<String> {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	private Long violationTicketId;

	/**
	 * The violation ticket number.
	 */
	@Schema(nullable = true)
	private String ticketNumber;

	/**
	 * Name of the organization of the disputant
	 */
	@Schema(nullable = true)
	private String disputantOrganizationName;

	/**
	 * The surname or corporate name.
	 */
	@Schema(nullable = true)
	private String disputantSurname;

	/**
	 * The given names or corporate name continued.
	 */
	@Schema(nullable = true)
	private String disputantGivenNames;

	/**
	 * The person issued the ticket has been identified as a young person.
	 */
	@Schema(nullable = true)
	private YesNo isYoungPerson;

	/**
	 * The drivers licence number. Note not all jurisdictions will use numeric drivers licence numbers.
	 */
	@Size(min = 7, max = 30)
	@Schema(nullable = true, minLength = 7, maxLength = 30)
	private String disputantDriversLicenceNumber;

	/**
	 * Disputant client number
	 */
	@Schema(nullable = true)
	private String disputantClientNumber;

	/**
	 * The province or state the drivers licence was issued by.
	 */
	@Size(max = 100)
	@Schema(nullable = true)
	private String driversLicenceProvince;

	@Size(max = 100)
	@Schema(nullable = true)
	private String driversLicenceCountry;

	/**
	 * The year the drivers licence was issued.
	 */
	@Schema(nullable = true)
	private Integer driversLicenceIssuedYear;

	/**
	 * The year the drivers licence expires.
	 */
	@Schema(nullable = true)
	private Integer driversLicenceExpiryYear;

	/**
	 * The birthdate of the individual the violation ticket was issued to.
	 */
	@Schema(nullable = true)
	private Date disputantBirthdate;

	/**
	 * The address of the individual the violation ticket was issued to.
	 */
	@Size(max = 100)
	@Schema(nullable = true)
	private String address;

	/**
	 * The city of the individual the violation ticket was issued to.
	 */
	@Schema(nullable = true)
	private String addressCity;

	/**
	 * The province or state of the individual the violation ticket was issued to.
	 */
	@Schema(nullable = true)
	private String addressProvince;

	/**
	 * The postal code or zip code.
	 */
	@Schema(nullable = true)
	private String addressPostalCode;

	/**
	 * The address country.
	 */
	@Schema(nullable = true)
	private String addressCountry;

	/**
	 * Officer Pin
	 */
	@Schema(nullable = true)
	private String officerPin;

	/**
	 * Detachment location.
	 */
	@Schema(nullable = true)
	private String detachmentLocation;

	/**
	 * The date and time the violation ticket was issue. Time must only be hours and minutes.
	 * This should always be in UTC date-time (ISO 8601) format
	 */
	@Schema(nullable = true)
	private Date issuedTs;

	/**
	 * The violation ticket was issued on this road or highway.
	 */
	@Schema(nullable = true)
	private String issuedOnRoadOrHighway;

	@Schema(nullable = true)
	private String issuedAtOrNearCity;

	/**
	 * The address represents a change of address. The address on the violation would be different from the address on the drivers licence or provided identification.
	 */
	@Schema(nullable = true)
	private YesNo isChangeOfAddress;

	/**
	 * The enforcement officer says that he or she has reasonable grounds to believe, and does believe, that the above named as the vehicle driver.
	 */
	@Schema(nullable = true)
	private YesNo isDriver;

	/**
	 * The enforcement officer says that he or she has reasonable grounds to believe, and does believe, that the above named as the vehicle owner.
	 */
	@Schema(nullable = true)
	private YesNo isOwner;

	/**
	 * Court location.
	 */
	@Schema(nullable = true)
	private String courtLocation;

	@JsonManagedReference
	private List<ViolationTicketCount> violationTicketCounts = new ArrayList<ViolationTicketCount>();

	@JsonBackReference
	@Schema(hidden = true)
	private Dispute dispute;
}
