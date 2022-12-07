package ca.bc.gov.open.jag.tco.oracledataapi.model;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@AllArgsConstructor
@NoArgsConstructor
public class DisputeResult {

	private Long disputeId;

	private DisputeStatus disputeStatus;

	private JJDisputeStatus jjDisputeStatus;

	public DisputeResult(Long disputeId, DisputeStatus disputeStatus) {
		this.disputeId = disputeId;
		this.disputeStatus = disputeStatus;
	}

}
