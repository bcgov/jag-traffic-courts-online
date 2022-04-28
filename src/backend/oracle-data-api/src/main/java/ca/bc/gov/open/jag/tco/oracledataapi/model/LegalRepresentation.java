package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.UUID;

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
public class LegalRepresentation {
	
	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
    @Id
    @GeneratedValue
    private UUID id;
	
	@Column
	private String lawFirmName;
	
	@Column
	private String lawyerName;
	
	@Column
	private String lawyerSurname;
	
	@Column
	@Email(regexp = ".+@.+\\..+")
	private String lawyerEmail;
	
	@Column
	private String lawyerAddress;
	
	@JsonBackReference
	@OneToOne
	@JoinColumn(name = "dispute_id", referencedColumnName = "id")
	@Schema(hidden = true)
	private Dispute dispute;
	
}
