package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.UUID;

import javax.persistence.Column;
import javax.persistence.Entity;
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
public class DisputedCount extends Auditable<String> {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	@Id
	@GeneratedValue
    private UUID id;

	/**
	 * Represents the disputant plea on count.
	 */
	@Column
	private Plea plea;

	/**
	 * The count number.
	 */
	@Column
	@Min(1) @Max(3)
	private int count;

	/**
	 * The disputant is requesting time to pay the ticketed amount.
	 */
	@Column
	private boolean requestTimeToPay;

	/**
	 * The disputant is requesting a reduction of the ticketed amount.
	 */
	@Column
	private boolean requestReduction;

	/**
	 * Does the want to appear in court?
	 */
	@Column
	private boolean appearInCourt;

	@JsonBackReference
	@ManyToOne(targetEntity=Dispute.class, fetch = FetchType.LAZY)
	@Schema(hidden = true)
	private Dispute dispute;

}
