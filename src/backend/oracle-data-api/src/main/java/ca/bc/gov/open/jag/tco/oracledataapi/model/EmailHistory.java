package ca.bc.gov.open.jag.tco.oracledataapi.model;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;
import javax.persistence.FetchType;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.ManyToOne;
import javax.persistence.Table;
import javax.validation.constraints.Max;
import javax.validation.constraints.Min;

import com.fasterxml.jackson.annotation.JsonBackReference;

import io.swagger.v3.oas.annotations.media.Schema;
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
public class EmailHistory extends Auditable<String> {
	
	/**
	 * Primary key
	 */
	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	@Id
	@GeneratedValue
	private Long emailHistoryId;
	
	/**
	 * From.
	 */
	@Column(length = 500)
	@Schema(nullable = false)
	private String from;
	
	/**
	 * To.
	 */
	@Column(length = 500)
	@Schema(nullable = false)
	private String recipients;
	
	/**
	 * Body
	 */
	@Column(length = 4000)
	@Schema(nullable = false)
	private String emailBody;
		
	/**
	 * Has the email been successfully sent?
	 * Only means a success code issued from email server
	 * Who knows what happens after that
	 */
	@Column
	@Schema(nullable = false)
	@Enumerated(EnumType.STRING)
    private YesNo successfullySent;
	
    /**
     * The violation ticket number.
     */
    @Column(length = 50)
    @Schema(nullable = false)
    private String ticketNumber;
    
}