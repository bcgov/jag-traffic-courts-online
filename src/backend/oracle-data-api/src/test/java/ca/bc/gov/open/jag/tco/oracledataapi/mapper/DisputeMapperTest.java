package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import static org.junit.jupiter.api.Assertions.assertEquals;

import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.TestInstance;

import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.ViolationTicket;

@TestInstance(TestInstance.Lifecycle.PER_CLASS)
public class DisputeMapperTest {

    private DisputeMapperImpl disputeMapperImpl;
    private RandomUtil randomUtil;

    @BeforeEach
    void setUp() {
        disputeMapperImpl = new DisputeMapperImpl();
    }

	@Test
	void testMapToDispute_shouldReturnFullLawyerAddress() {
		ViolationTicket testViolationTicket = new ViolationTicket();
		Dispute testDispute = new Dispute();
		// Lawyer Address string split into three different address fields each having 100 characters
		testDispute.setLawFirmAddrLine1Txt("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce accumsan nulla quam, non aliquam erat");
		testDispute.setLawFirmAddrLine2Txt(" porttitor eu. Vivamus ornare ante nec eros luctus, non tincidunt ipsum interdum. Aliquam felis feli");
		testDispute.setLawFirmAddrLine3Txt("s, ullamcorper non rutrum sit amet, sollicitudin sit amet lectus. Quisque non massa dolor porttitor.");
		testViolationTicket.setDispute(testDispute);

		ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute result = disputeMapperImpl.convertViolationTicketDtoToDispute(testViolationTicket);

		assertEquals("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce accumsan nulla quam, non aliquam erat porttitor eu. "
				+ "Vivamus ornare ante nec eros luctus, non tincidunt ipsum interdum. Aliquam felis felis, ullamcorper non rutrum sit amet, sollicitudin sit amet lectus. "
				+ "Quisque non massa dolor porttitor.", result.getLawyerAddress());
	}
}
