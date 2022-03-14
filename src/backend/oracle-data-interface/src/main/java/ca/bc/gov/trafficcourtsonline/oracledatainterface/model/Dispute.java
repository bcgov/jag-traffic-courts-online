package ca.bc.gov.trafficcourtsonline.oracledatainterface.model;

import java.sql.Date;
import java.util.List;

import javax.persistence.CascadeType;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.OneToMany;
import javax.persistence.Table;

import com.fasterxml.jackson.annotation.JsonManagedReference;

//mark class as an Entity   
@Entity  
//defining class name as Table name  
@Table  
public class Dispute {
	@Id
	@GeneratedValue
	@Column
	private int id;
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
	@Column
	private String streetAddress;
	@Column
    private String province;
	@Column
    private String postalCode;
	@Column
    private String homePhone;
	@Column
    private String driversLicense;
	@Column
    private String driversLicenseProvince;
	@Column
    private String workPhone;
	@Column
    private Date dateOfBirth;
	@Column
    private String enforcementOrganization;
	@Column
    private Date serviceDate;
	@OneToMany(mappedBy = "dispute", cascade = CascadeType.ALL)
	@JsonManagedReference("dispute_ticket_counts")
    private List<TicketCount> ticketCounts;
    @Column
    private boolean lawyerRepresentation;
    @Column
    private String interpreterLanguage;
    @Column
    private boolean witnessIntent;
	
	public int getId() {
		return id;
	}
	public void setId(int id) {
		this.id = id;
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
	public String getStreetAddress() {
		return streetAddress;
	}
	public void setStreetAddress(String streetAddress) {
		this.streetAddress = streetAddress;
	}
	public String getProvince() {
		return province;
	}
	public void setProvince(String province) {
		this.province = province;
	}
	public String getPostalCode() {
		return postalCode;
	}
	public void setPostalCode(String postalCode) {
		this.postalCode = postalCode;
	}
	public String getHomePhone() {
		return homePhone;
	}
	public void setHomePhone(String homePhone) {
		this.homePhone = homePhone;
	}
	public String getDriversLicense() {
		return driversLicense;
	}
	public void setDriversLicense(String driversLicense) {
		this.driversLicense = driversLicense;
	}
	public String getDriversLicenseProvince() {
		return driversLicenseProvince;
	}
	public void setDriversLicenseProvince(String driversLicenseProvince) {
		this.driversLicenseProvince = driversLicenseProvince;
	}
	public String getWorkPhone() {
		return workPhone;
	}
	public void setWorkPhone(String workPhone) {
		this.workPhone = workPhone;
	}
	public Date getDateOfBirth() {
		return dateOfBirth;
	}
	public void setDateOfBirth(Date dateOfBirth) {
		this.dateOfBirth = dateOfBirth;
	}
	public String getEnforcementOrganization() {
		return enforcementOrganization;
	}
	public void setEnforcementOrganization(String enforcementOrganization) {
		this.enforcementOrganization = enforcementOrganization;
	}
	public Date getServiceDate() {
		return serviceDate;
	}
	public void setServiceDate(Date serviceDate) {
		this.serviceDate = serviceDate;
	}
	public List<TicketCount> getTicketCounts() {
		return ticketCounts;
	}
	public void setTicketCounts(List<TicketCount> ticketCounts) {
		this.ticketCounts = ticketCounts;
	}
	public boolean isLawyerRepresentation() {
		return lawyerRepresentation;
	}
	public void setLawyerRepresentation(boolean lawyerRepresentation) {
		this.lawyerRepresentation = lawyerRepresentation;
	}
	public String getInterpreterLanguage() {
		return interpreterLanguage;
	}
	public void setInterpreterLanguage(String interpreterLanguage) {
		this.interpreterLanguage = interpreterLanguage;
	}
	public boolean isWitnessIntent() {
		return witnessIntent;
	}
	public void setWitnessIntent(boolean witnessIntent) {
		this.witnessIntent = witnessIntent;
	}
}
