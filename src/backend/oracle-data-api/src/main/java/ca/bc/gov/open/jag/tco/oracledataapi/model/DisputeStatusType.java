package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.ArrayList;
import java.util.List;

import com.fasterxml.jackson.annotation.JsonManagedReference;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;


@Getter
@Setter
@NoArgsConstructor
public class DisputeStatusType extends Auditable<String> {

	/**
	 * Dispute Status Type Code primary key
	 */
	private String disputeStatusTypeCode;

	/**
	 * The description of the status type
	 */
	@Schema(nullable = false)
	private DisputeStatus disputeStatusTypeDescription;

	/**
	 * Whether the dispute status is active or not Y/N
	 */
	@Schema(nullable = false)
	private YesNo Active;

	@JsonManagedReference
	private List<Dispute> disputes = new ArrayList<Dispute>();

	public void addDisputes(List<Dispute> disputes) {
		for (Dispute dispute : disputes) {
			dispute.setDisputeStatusType(this);
		}
		this.disputes.addAll(disputes);
	}

}
