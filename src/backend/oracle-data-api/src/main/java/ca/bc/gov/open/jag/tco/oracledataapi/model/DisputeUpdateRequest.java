package ca.bc.gov.open.jag.tco.oracledataapi.model;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.validation.constraints.NotNull;
import javax.validation.constraints.Size;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Entity
@Table
@Getter
@Setter
@NoArgsConstructor
public class DisputeUpdateRequest extends Auditable<String> {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	@Id
	@GeneratedValue
	private Long disputeUpdateRequestId;

	@Column
	private Long disputeId;

	@Column(length = 3)
	@Schema(nullable = false)
	@NotNull
	private DisputeUpdateRequestStatus status;

	@Column(length = 3)
	@Schema(nullable = false)
	@NotNull
	private DisputeUpdateRequestType updateType;

	@Column(length = 1000)
	@Schema(nullable = false)
	@Size(min = 3, max = 1000)
	@NotNull
	private String updateJson;

}
