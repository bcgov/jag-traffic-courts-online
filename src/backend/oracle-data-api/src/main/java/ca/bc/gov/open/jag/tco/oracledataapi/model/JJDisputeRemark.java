package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.FetchType;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.ManyToOne;
import javax.persistence.Table;
import javax.validation.constraints.Size;

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
@Entity
//defining class name as Table name
@Table
@Getter
@Setter
@NoArgsConstructor
public class JJDisputeRemark extends Auditable<String> {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	@Id
	@GeneratedValue
    private Long id;

	/**
	 * Name and surname of the JJ/Staff who adds the remark for the dispute.
	 */
	@Column
    private String userFullName;

	/**
	 * JJ's remark notes that will be added to the dispute.
	 */
	@Size(max = 500)
	@Column(length = 500)
	@Schema(maxLength = 500)
	private String note;

	@Column
	private Date remarksMadeTs;

	@JsonBackReference
	@ManyToOne(targetEntity=JJDispute.class, fetch = FetchType.LAZY)
	@JoinColumn(name = "ticket_number")
	@Schema(hidden = true)
	private JJDispute jjDispute;

}
