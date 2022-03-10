package ca.bc.gov.trafficcourtsonline.oracledatainterface.model;

import java.sql.Date;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import javax.persistence.Table;

//mark class as an Entity   
@Entity  
//defining class name as Table name  
@Table  
public class Dispute {
	@Id  
	@Column
	private int disputeId;
	@Column
	private String ticketNumber;
	@Column
	private String courtLocation;
	@Column
	private Date violationDate;
	@Column
	private String disputantSurname;
	@Column
	private String givenNames;
	
	public int getDisputeId() {
		return disputeId;
	}
	public void setDisputeId(int disputeId) {
		this.disputeId = disputeId;
	}
	public String getTicketNumber() {
		return ticketNumber;
	}
	public void setTicketNumber(String ticketNumber) {
		this.ticketNumber = ticketNumber;
	}
	public String getCourtLocation() {
		return courtLocation;
	}
	public void setCourtLocation(String courtLocation) {
		this.courtLocation = courtLocation;
	}
	public Date getViolationDate() {
		return violationDate;
	}
	public void setViolationDate(Date violationDate) {
		this.violationDate = violationDate;
	}
	public String getDisputantSurname() {
		return disputantSurname;
	}
	public void setDisputantSurname(String disputantSurname) {
		this.disputantSurname = disputantSurname;
	}
	public String getGivenNames() {
		return givenNames;
	}
	public void setGivenNames(String givenNames) {
		this.givenNames = givenNames;
	}
}
