package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.sql.Date;
import java.util.ArrayList;
import java.util.List;

import javax.persistence.CascadeType;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.OneToMany;
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
public class Dispute {

    @Id
    @GeneratedValue (strategy = GenerationType.IDENTITY)
    private Integer id;

    @Enumerated(EnumType.STRING)
    private DisputeStatus status;

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

    @OneToMany(cascade = CascadeType.ALL)
    private List<TicketCount> ticketCounts = new ArrayList<TicketCount>();

    @Column
    private boolean lawyerRepresentation;

    @Column
    private String interpreterLanguage;

    @Column
    private boolean witnessIntent;

    /**
     * A note or reason indicating why this Dispute has a status of REJECTED. This field is blank for other status types.
     */
    @Column
    private String rejectedReason;

}
