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

import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

//mark class as an Entity   
@Entity  
//defining class name as Table name  
@Table
@Getter @Setter @NoArgsConstructor
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
}
