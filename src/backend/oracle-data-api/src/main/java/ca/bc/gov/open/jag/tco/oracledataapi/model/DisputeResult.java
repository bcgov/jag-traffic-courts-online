package ca.bc.gov.open.jag.tco.oracledataapi.model;

import io.swagger.v3.oas.annotations.media.Schema;
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

	private String noticeOfDisputeGuid;

	private DisputeStatus disputeStatus;

	@Schema(nullable = true)
	private JJDisputeStatus jjDisputeStatus;

	@Schema(nullable = true)
	private JJDisputeHearingType jjDisputeHearingType;

	private Boolean isEmailAddressVerified = Boolean.FALSE;

	/**
	 * 
	 * @param disputeId
	 * @param noticeOfDisputeGuid
	 * @param disputeStatus
	 * @param isEmailAddressVerified TRUE if there is no email address or if the email address has been successfully verified, FALSE otherwise.
	 */
	public DisputeResult(Long disputeId, String noticeOfDisputeGuid, DisputeStatus disputeStatus, Boolean isEmailAddressVerified) {
		this.disputeId = disputeId;
		this.noticeOfDisputeGuid = noticeOfDisputeGuid;
		this.disputeStatus = disputeStatus;
		this.isEmailAddressVerified  = isEmailAddressVerified;
	}

}
