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

	@Schema(nullable = true, description = "Null if there is no email address, true if the email address has been successfully verified, false otherwise.")
	private Boolean isEmailAddressVerified;
	
	@Schema(nullable = true, description="This represents user's request to appear in court if 'YES' or provide written reasons if 'NO', null if there was no response.")
	private YesNo requestCourtAppearanceYn;

	/**
	 * 
	 * @param disputeId
	 * @param noticeOfDisputeGuid
	 * @param disputeStatus
	 * @param isEmailAddressVerified <code>NULL</code> if there is no email address, <code>TRUE</code> if the email address has been successfully verified, <code>FALSE</code> otherwise.
	 */
	public DisputeResult(Long disputeId, String noticeOfDisputeGuid, DisputeStatus disputeStatus, Boolean isEmailAddressVerified, YesNo requestCourtAppearanceYn) {
		this.disputeId = disputeId;
		this.noticeOfDisputeGuid = noticeOfDisputeGuid;
		this.disputeStatus = disputeStatus;
		this.isEmailAddressVerified  = isEmailAddressVerified;
		this.requestCourtAppearanceYn = requestCourtAppearanceYn;
	}

}
