package ca.bc.gov.trafficcourtsonline.oracledatainterface.model;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.ManyToOne;
import javax.persistence.Table;

import com.fasterxml.jackson.annotation.JsonBackReference;

//mark class as an Entity   
@Entity  
//defining class name as Table name  
@Table  
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
	
	public int getId() {
		return id;
	}
	public void setId(int id) {
		this.id = id;
	}
	public String getOffenceDeclaration() {
		return offenceDeclaration;
	}
	public void setOffenceDeclaration(String offenceDeclaration) {
		this.offenceDeclaration = offenceDeclaration;
	}
	public boolean isTimeToPayRequest() {
		return timeToPayRequest;
	}
	public void setTimeToPayRequest(boolean timeToPayRequest) {
		this.timeToPayRequest = timeToPayRequest;
	}
	public boolean isFineReductionRequest() {
		return fineReductionRequest;
	}
	public void setFineReductionRequest(boolean fineReductionRequest) {
		this.fineReductionRequest = fineReductionRequest;
	}
	public Dispute getDispute() {
		return dispute;
	}
	public void setDispute(Dispute dispute) {
		this.dispute = dispute;
	}
}
