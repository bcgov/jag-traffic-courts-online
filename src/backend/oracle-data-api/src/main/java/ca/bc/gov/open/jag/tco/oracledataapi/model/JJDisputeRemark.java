package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.Date;
import jakarta.validation.constraints.Size;

import com.fasterxml.jackson.annotation.JsonBackReference;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

//mark class as an Entity
/**
 * @author 237563
 *
 * Represents a note/comment on a JJ dispute that is for internal use of JJs.
 *
 */
@Getter
@Setter
@NoArgsConstructor
public class JJDisputeRemark extends Auditable<String> {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	private Long id;

	/**
	 * Name and surname of the JJ/Staff who adds the remark for the dispute.
	 */
	private String userFullName;

	/**
	 * JJ's remark notes that will be added to the dispute.
	 */
	@Size(max = 4000)
	@Schema(maxLength = 4000)
	private String note;

	private Date remarksMadeTs;

	@JsonBackReference
	@Schema(hidden = true)
	private JJDispute jjDispute;

}
