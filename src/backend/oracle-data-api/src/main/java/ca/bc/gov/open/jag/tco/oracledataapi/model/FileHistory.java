package ca.bc.gov.open.jag.tco.oracledataapi.model;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.validation.constraints.NotNull;

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
public class FileHistory extends Auditable<String> {

	/**
	 * Primary key
	 */
	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	@Id
	@GeneratedValue
	private Long fileHistoryId;

	/**
	 * The occam dispute id.
	 */
	@Column
	@Schema(nullable = false)
	@NotNull
	private Long disputeId;

	/**
	 * File history entry type
	 */
	@Column(length = 4)
	@Schema(nullable = false)
	@NotNull
	private AuditLogEntryType auditLogEntryType;
	
	/**
	 * description
	 */
	@Column(length = 500)
	@Schema(nullable = true)
	private String description;

	/**
	 * VTC staff's comment in Ticket Validation
	 */
	@Column(length = 500)
	@Schema(description = "VTC staff's comment in Ticket Validation", nullable = true)
	private String comment;
	
	/**
	 * Action By User
	 */
	@Column(length = 240)
	@Schema(nullable = true)
	private String actionByApplicationUser;
	
	public String determineSchema() {
		return this.auditLogEntryType.getSchema();
	}
}