package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.validation.constraints.NotNull;
import javax.validation.constraints.Size;

import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;

import ca.bc.gov.open.jag.tco.oracledataapi.config.DateTimeDeserializer;
import ca.bc.gov.open.jag.tco.oracledataapi.config.DateTimeSerializer;
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
	@JsonSerialize(using = DateTimeSerializer.class)
	@JsonDeserialize(using = DateTimeDeserializer.class)
	private Date statusUpdateTs;

}