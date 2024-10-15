package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.Date;

import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
public class DisputeUpdateRequest extends Auditable<String> {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	private Long disputeUpdateRequestId;

	private Long disputeId;

	@Schema(nullable = false)
	@NotNull
	private DisputeUpdateRequestStatus status;

	@Schema(nullable = false)
	@NotNull
	private DisputeUpdateRequestType updateType;

	@Schema(nullable = false)
	@Size(min = 3, max = 4000)
	@NotNull
	private String updateJson;


	@Schema(description = "Current state of the Disputant's contact information in JSON format", maxLength = 4000, nullable = true)
	@Size(max = 4000)
	private String currentJson;

	@Schema(description = "Date and time the status was last updated", nullable = true)
	private Date statusUpdateTs;

}
