package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.OneToOne;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;
import javax.validation.constraints.Email;
import javax.validation.constraints.Size;

import com.fasterxml.jackson.annotation.JsonBackReference;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;


/**
 * @author 237563
 * 
 * Represents the contact information section of a JJ dispute.
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
public class DisputantContactInformation extends Auditable<String>{
	
	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
    @Id
    private Long id;

	/**
	 * The surname or corporate name from contact information submitted via TCO.
	 */
	@Column
	private String surname;

	/**
	 * The given names or corporate name from contact information submitted via TCO.
	 */
	@Column
	private String givenNames;

	/**
	 * The disputant's birthdate from contact information submitted via TCO.
	 */
	@Column
	@Temporal(TemporalType.DATE)
	private Date birthdate;

	/**
	 * The drivers licence number from reconciled ticket data.
	 */
	@Size(max = 20)
	@Column(length = 20)
	@Schema(maxLength = 20)
	private String driversLicenceNumber;

	/**
	 * The mailing address province of the disputant from reconciled ticket data.
	 */
	@Size(max = 30)
	@Column(length = 30)
	@Schema(maxLength = 30)
	private String province;

	/**
	 * The mailing address of the disputant from contact information submitted via TCO.
	 */
	@Column
	private String address;
	
	/**
	 * The disputant's email address from reconciled ticket data.
	 */
	@Column
	@Email(regexp = ".+@.+\\..+")
	private String emailAddress;
	
	@JsonBackReference
	@OneToOne
	@JoinColumn(name = "jjdispute_id", referencedColumnName = "ticketNumber")
	@Schema(hidden = true)
	private JJDispute jjDispute;
}
