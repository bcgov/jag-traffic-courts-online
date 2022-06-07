package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.Date;
import java.util.UUID;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * @author 237563
 * Represents a violation ticket dispute for JJ Workbench
 *
 */
//mark class as an Entity
@Entity
//defining class name as Table name
@Table
@Getter
@Setter
@NoArgsConstructor
public class JJDispute extends Auditable<String>{
	
	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
    @Id
    @GeneratedValue
    private UUID id;

    /**
     * The violation ticket number.
     */
    @Column
    private String ticketNumber;
    
    /**
     * The date and time the violation ticket was issued.
     */
    @Column
    @Schema(nullable = true)
    @Temporal(TemporalType.TIMESTAMP)
    private Date violationDate;
    
    /**
     * The given name and last name of the disputant.
     */
    @Column
    @Schema(nullable = true)
    private String disputantName;
    
    /**
     * The enforcement officer associated to the disputed violation ticket.
     */
    @Column
    @Schema(nullable = true)
    private String enforcementOfficer;
    
    /**
     * The police detachment location.
     */
    @Column
    @Schema(nullable = true)
    private String policeDetachment;

    /**
     * The provincial court hearing location named on the violation ticket.
     */
    @Column
    @Schema(nullable = true)
    private String courthouseLocation;
    
	/**
	 * The ID of the Staff whom the dispute is assigned to be reviewed on JJ Workbench.
	 */
	@Column
	@Schema(nullable = true)
	private String jjAssignedTo;
	
	/**
	 * The ID of the jj group whom the dispute is assigned to be listed on JJ Workbench.
	 */
	@Column
	@Schema(nullable = true)
	private String jjGroupAssignedTo;

}
