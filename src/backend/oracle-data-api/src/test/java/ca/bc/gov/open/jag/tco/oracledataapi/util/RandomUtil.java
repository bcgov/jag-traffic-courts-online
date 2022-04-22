package ca.bc.gov.open.jag.tco.oracledataapi.util;

import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;

public class RandomUtil {

	public static Dispute createDispute() {
		Dispute dispute = new Dispute();
		dispute.setStatus(DisputeStatus.NEW);
		dispute.setGivenNames("Steven");
		dispute.setDisputantSurname("Strange");
		return dispute;
	}

}
