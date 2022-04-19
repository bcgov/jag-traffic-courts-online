package ca.bc.gov.open.jag.tco.oracledataapi.model;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Table;

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
public class TicketCount {

	@Id
	@GeneratedValue (strategy = GenerationType.IDENTITY)
	private Integer id;

	@Column
	private String offenceDeclaration;

	@Column
	private boolean timeToPayRequest;

	@Column
	private boolean fineReductionRequest;

}
