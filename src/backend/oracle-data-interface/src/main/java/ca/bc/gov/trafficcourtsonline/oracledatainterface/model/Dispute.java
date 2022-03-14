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
	@Getter @Setter private int id;
	@Column
	@Getter @Setter private String ticketNumber;
	@Column
	@Getter @Setter private String courtLocation;
	@Column
	@Getter @Setter private Date violationDate;
	@Column
	@Getter @Setter private String disputantSurname;
	@Column
	@Getter @Setter private String givenNames;
	@Column
	@Getter @Setter private String streetAddress;
	@Column
	@Getter @Setter private String province;
	@Column
	@Getter @Setter private String postalCode;
	@Column
	@Getter @Setter private String homePhone;
	@Column
	@Getter @Setter private String driversLicense;
	@Column
	@Getter @Setter private String driversLicenseProvince;
	@Column
	@Getter @Setter private String workPhone;
	@Column
	@Getter @Setter private Date dateOfBirth;
	@Column
	@Getter @Setter private String enforcementOrganization;
	@Column
	@Getter @Setter private Date serviceDate;
	@OneToMany(mappedBy = "dispute", cascade = CascadeType.ALL)
	@JsonManagedReference("dispute_ticket_counts")
	@Getter @Setter private List<TicketCount> ticketCounts;
    @Column
    @Getter @Setter private boolean lawyerRepresentation;
    @Column
    @Getter @Setter private String interpreterLanguage;
    @Column
    @Getter @Setter private boolean witnessIntent;
}
