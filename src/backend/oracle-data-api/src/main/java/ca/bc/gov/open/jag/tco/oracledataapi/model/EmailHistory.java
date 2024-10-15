package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.Date;

import jakarta.validation.constraints.Size;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

//mark class as an Entity
@Getter
@Setter
@NoArgsConstructor
public class EmailHistory extends Auditable<String> {

	/**
	 * Primary key
	 */
	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	private Long emailHistoryId;

	/**
	 * The date and time the email was sent
	 */
	@Schema(nullable = false)
	private Date emailSentTs;

	/**
	 * FromEmailAddress.
	 */
	@Size(max = 100)
	@Schema(maxLength = 100, nullable = false)
	private String fromEmailAddress;

	/**
	 * ToEmailAddress.
	 */
	@Size(max = 4000)
	@Schema(maxLength = 4000, nullable = false)
	private String toEmailAddress;

	/**
	 * ccEmailAddress.
	 */
	@Size(max = 4000)
	@Schema(maxLength = 4000, nullable = true)
	private String ccEmailAddress;

	/**
	 * bccEmailAddress.
	 */
	@Size(max = 4000)
	@Schema(maxLength = 4000, nullable = true)
	private String bccEmailAddress;

	/**
	 * Subject
	 */
	@Size(max = 1000)
	@Schema(maxLength = 1000, nullable = false)
	private String subject;

	/**
	 * Body if HTML
	 */
	@Size(max = 4000)
	@Schema(maxLength = 4000, nullable = true)
	private String htmlContent;

	/**
	 * Body if Plain text
	 */
	@Size(max = 4000)
	@Schema(maxLength = 4000, nullable = true)
	private String plainTextContent;

	/**
	 * Has the email been successfully sent?
	 * Only means a success code issued from email server
	 * Who knows what happens after that
	 */
	@Schema(nullable = false)
	private YesNo successfullySent;

	/**
	 * The occam dispute ID.
	 */
	@Schema(nullable = false)
	private Long occamDisputeId;

}