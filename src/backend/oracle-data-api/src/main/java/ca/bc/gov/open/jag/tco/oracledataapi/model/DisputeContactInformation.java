package ca.bc.gov.open.jag.tco.oracledataapi.model;

import javax.validation.constraints.Size;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
public class DisputeContactInformation {

	/**
	 * The surname or corporate name.
	 */
	@Size(max = 30)
	@Schema(maxLength = 30)
	private String disputantSurname;

	/**
	 * First given name of the disputant
	 */
	@Size(max = 30)
	@Schema(maxLength = 30)
	private String disputantGivenName1;

	/**
	 * Second given name of the disputant
	 */
	@Size(max = 30)
	@Schema(maxLength = 30)
	private String disputantGivenName2;

	/**
	 * Third given name of the disputant
	 */
	@Size(max = 30)
	@Schema(maxLength = 30)
	private String disputantGivenName3;

	/**
	 * The mailing address line one of the disputant.
	 */
	@Size(max = 100)
	@Schema(maxLength = 100)
	private String addressLine1;

	/**
	 * The mailing address line two of the disputant.
	 */
	@Size(max = 100)
	@Schema(maxLength = 100)
	private String addressLine2;

	/**
	 * The mailing address line three of the disputant.
	 */
	@Size(max = 100)
	@Schema(maxLength = 100)
	private String addressLine3;

	/**
	 * The mailing address city of the disputant.
	 */
	@Size(max = 30)
	@Schema(maxLength = 30)
	private String addressCity;

	/**
	 * The mailing address province of the disputant.
	 */
	@Size(max = 30)
	@Schema(maxLength = 30)
	private String addressProvince;

	/**
	 * The mailing address postal code or zip code of the disputant.
	 */
	@Size(max = 10)
	@Schema(maxLength = 10)
	private String postalCode;

	/**
	 * The disputant's email address.
	 */
	@Size(max = 100)
	@Schema(maxLength = 100)
	private String emailAddress;

	/**
	 * The disputant's home phone number.
	 */
	@Size(max = 20)
	@Schema(maxLength = 20)
	private String homePhoneNumber;

}
