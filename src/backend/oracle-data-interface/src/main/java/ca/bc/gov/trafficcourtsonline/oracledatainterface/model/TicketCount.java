package ca.bc.gov.trafficcourtsonline.oracledatainterface.model;

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
	@Getter @Setter private int id;
	@Column
	@Getter @Setter private String offenceDeclaration;
	@Column
	@Getter @Setter private boolean timeToPayRequest;
	@Column
	@Getter @Setter private boolean fineReductionRequest;
	@ManyToOne
	@JoinColumn(name="dispute_id", nullable = false)
	@JsonBackReference("dispute_ticket_counts")
	@Getter @Setter private Dispute dispute;
}
