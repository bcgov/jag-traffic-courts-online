package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;
import javax.validation.constraints.Size;

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
	 * The date and time the email was sent
	 */
	@Column(nullable = false)
	@Temporal(TemporalType.TIMESTAMP)
	@Schema(nullable = false)
	private Date emailSentTs;

	/**
	 * FromEmailAddress.
	 */
	@Column(length = 100, nullable = false)
	@Size(max = 100)
	@Schema(maxLength = 100, nullable = false)
	private String fromEmailAddress;

	/**
	 * ToEmailAddress.
	 */
	@Column(length = 4000, nullable = false)
	@Size(max = 4000)
	@Schema(maxLength = 4000, nullable = false)
	private String toEmailAddress;

	/**
	 * ccEmailAddress.
	 */
	@Column(length = 4000, nullable = true)
	@Size(max = 4000)
	@Schema(maxLength = 4000, nullable = true)
	private String ccEmailAddress;

	/**
	 * bccEmailAddress.
	 */
	@Column(length = 4000, nullable = true)
	@Size(max = 4000)
	@Schema(maxLength = 4000, nullable = true)
	private String bccEmailAddress;

	/**
	 * Subject
	 */
	@Column(length = 1000, nullable = false)
	@Size(max = 1000)
	@Schema(maxLength = 1000, nullable = false)
	private String subject;

	/**
	 * Body if HTML
	 */
	@Column(length = 4000, nullable = true)
	@Size(max = 4000)
	@Schema(maxLength = 4000, nullable = true)
	private String htmlContent;

	/**
	 * Body if Plain text
	 */
	@Column(length = 4000, nullable = true)
	@Size(max = 4000)
	@Schema(maxLength = 4000, nullable = true)
	private String plainTextContent;

	/**
	 * Has the email been successfully sent?
	 * Only means a success code issued from email server
	 * Who knows what happens after that
	 */
	@Column(nullable = false)
	@Schema(nullable = false)
	@Enumerated(EnumType.STRING)
	private YesNo successfullySent;

	/**
	 * The occam dispute ID.
	 */
	@Schema(nullable = false)
	private Long occamDisputeId;

}