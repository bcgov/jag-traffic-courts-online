package ca.bc.gov.open.jag.tco.oracledataapi.model;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.ManyToOne;
import javax.persistence.Table;

import com.fasterxml.jackson.annotation.JsonBackReference;

import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

//mark class as an Entity   
@Entity  
//defining class name as Table name  
@Table
@Getter @Setter @NoArgsConstructor
public class TicketCount {
	@Id
	@GeneratedValue
	private int id;
	@Column
	private String offenceDeclaration;
	@Column
	private boolean timeToPayRequest;
	@Column
	private boolean fineReductionRequest;
	@ManyToOne
	@JoinColumn(name="dispute_id", nullable = false)
	@JsonBackReference("dispute_ticket_counts")
	private Dispute dispute;
}
