package ca.bc.gov.open.jag.tco.oracledataapi.model;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.OneToOne;
import javax.persistence.Table;
import javax.validation.constraints.Email;

import com.fasterxml.jackson.annotation.JsonBackReference;

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
public class LegalRepresentation extends Auditable<String> {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
    @Id
    @GeneratedValue
    private Long id;
	
	/**
	 * Name of the law firm that will represent the disputant at the hearing.
	 */
	@Column
	private String lawFirmName;
	
	/**
	 * Full name of the lawyer who will represent the disputant at the hearing.
	 */
	@Column
	private String lawyerFullName;
	
	/**
	 * Email address of the lawyer who will represent the disputant at the hearing.
	 */
	@Column
	@Email(regexp = ".+@.+\\..+")
	private String lawyerEmail;
	
	/**
	 * Address of the lawyer who will represent the disputant at the hearing.
	 */
	@Column
	private String lawyerAddress;
	
	/**
	 * Phone number of the lawyer who will represent the disputant at the hearing.
	 */
	@Column
	private String lawyerPhoneNumber;
	
	@JsonBackReference
	@OneToOne
	@JoinColumn(name = "dispute_id", referencedColumnName = "id")
	@Schema(hidden = true)
	private Dispute dispute;

}
