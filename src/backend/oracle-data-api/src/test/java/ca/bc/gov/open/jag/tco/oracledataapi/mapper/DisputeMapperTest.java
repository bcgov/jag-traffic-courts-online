package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import static org.junit.jupiter.api.Assertions.assertEquals;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.api.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.api.model.ViolationTicket;

public class DisputeMapperTest extends BaseTestSuite{

	private DisputeMapper disputeMapper;

	@Test
	void testMapToDispute_shouldReturnFullLawyerAddress() {
		ViolationTicket testViolationTicket = new ViolationTicket();
		Dispute testDispute = new Dispute();
		// Lawyer Address string split into three different address fields each having 100 characters
		testDispute.setLawFirmAddrLine1Txt("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce accumsan nulla quam, non aliquam erat");
		testDispute.setLawFirmAddrLine2Txt(" porttitor eu. Vivamus ornare ante nec eros luctus, non tincidunt ipsum interdum. Aliquam felis feli");
		testDispute.setLawFirmAddrLine3Txt("s, ullamcorper non rutrum sit amet, sollicitudin sit amet lectus. Quisque non massa dolor porttitor.");
		testViolationTicket.setDispute(testDispute);

		ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute result = disputeMapper.convertViolationTicketDtoToDispute(testViolationTicket);

		assertEquals("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce accumsan nulla quam, non aliquam erat porttitor eu. "
				+ "Vivamus ornare ante nec eros luctus, non tincidunt ipsum interdum. Aliquam felis felis, ullamcorper non rutrum sit amet, sollicitudin sit amet lectus. "
				+ "Quisque non massa dolor porttitor.", result.getLawyerAddress());
	}
}
