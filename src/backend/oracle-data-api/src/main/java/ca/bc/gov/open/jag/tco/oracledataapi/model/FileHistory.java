package ca.bc.gov.open.jag.tco.oracledataapi.model;

import jakarta.validation.constraints.NotNull;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

//mark class as an Entity
@Getter
@Setter
@NoArgsConstructor
public class FileHistory extends Auditable<String> {

	/**
	 * Primary key
	 */
	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	private Long fileHistoryId;

	/**
	 * The occam dispute id.
	 */
	@Schema(nullable = false)
	@NotNull
	private Long disputeId;

	/**
	 * File history entry type
	 */
	@Schema(nullable = false)
	@NotNull
	private AuditLogEntryType auditLogEntryType;

	/**
	 * description
	 */
	@Schema(nullable = true)
	private String description;

	/**
	 * VTC staff's comment in Ticket Validation
	 */
	@Schema(description = "VTC staff's comment in Ticket Validation", nullable = true)
	private String comment;

	/**
	 * Action By User
	 */
	@Schema(nullable = true)
	private String actionByApplicationUser;

	public String determineSchema() {
		return this.auditLogEntryType.getSchema();
	}
}