package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNull;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.EnumSource;
import org.springframework.beans.factory.annotation.Autowired;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.model.ContactType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeSourceType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatusType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.ViolationTicket;

public class ViolationTicketMapperTest extends BaseTestSuite{

	private ViolationTicketMapper violationTicketMapper;

	@Autowired
	private ViolationTicketMapperImpl violationTicketMapperImpl;

	@BeforeEach
	void setUp() {
		violationTicketMapper = violationTicketMapperImpl;
	}

	@Test
	void testMapToViolationTicket_shouldReturnSplitOutLawyerAddress() {
		Dispute testDispute = new Dispute();
		ViolationTicket testViolationTicket = new ViolationTicket();
		DisputeStatusType statusType = new DisputeStatusType();
		testDispute.setDisputeStatusType(statusType);
		testDispute.setStatus(DisputeStatus.NEW);
		testDispute.setViolationTicket(testViolationTicket);
		testDispute.setContactTypeCd(ContactType.INDIVIDUAL);
		// Lawyer Address string having 300 characters (max allowed length for lawyerAddress field)
		testDispute.setLawyerAddress("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce accumsan nulla quam, non aliquam erat porttitor eu. "
				+ "Vivamus ornare ante nec eros luctus, non tincidunt ipsum interdum. Aliquam felis felis, ullamcorper non rutrum sit amet, sollicitudin sit amet lectus. "
				+ "Quisque non massa dolor porttitor.");

		ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.ViolationTicket result = violationTicketMapper.convertDisputeToViolationTicketDto(testDispute);

		// Should return split out lawyer address string into 3 different fields each having 100 characters
		assertEquals("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce accumsan nulla quam, non aliquam erat", result.getDispute().getLawFirmAddrLine1Txt());
		assertEquals(" porttitor eu. Vivamus ornare ante nec eros luctus, non tincidunt ipsum interdum. Aliquam felis feli", result.getDispute().getLawFirmAddrLine2Txt());
		assertEquals("s, ullamcorper non rutrum sit amet, sollicitudin sit amet lectus. Quisque non massa dolor porttitor.", result.getDispute().getLawFirmAddrLine3Txt());
	}

	@ParameterizedTest
	@EnumSource(value = DisputeSourceType.class, names = { "CITZ", "STAF", "JSTN" })
	void testMapToViolationTicket_shouldMapDisputeSourceTypeToCode(DisputeSourceType sourceType) {
		Dispute testDispute = createMinimalDispute();
		testDispute.setDisputeSourceType(sourceType);

		ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.ViolationTicket result = violationTicketMapper.convertDisputeToViolationTicketDto(testDispute);

		assertEquals(sourceType.name(), result.getDispute().getDisputeSourceTypeCd());
	}

	@Test
	void testMapToViolationTicket_shouldNotSendUnknownOrNullDisputeSourceType() {
		Dispute unknownDispute = createMinimalDispute();
		unknownDispute.setDisputeSourceType(DisputeSourceType.UNKNOWN);
		Dispute nullDispute = createMinimalDispute();

		// UNKNOWN would fail the foreign key on OCCAM_DISPUTES, so it must be sent as null
		assertNull(violationTicketMapper.convertDisputeToViolationTicketDto(unknownDispute).getDispute().getDisputeSourceTypeCd());
		assertNull(violationTicketMapper.convertDisputeToViolationTicketDto(nullDispute).getDispute().getDisputeSourceTypeCd());
	}

	private Dispute createMinimalDispute() {
		Dispute testDispute = new Dispute();
		testDispute.setDisputeStatusType(new DisputeStatusType());
		testDispute.setStatus(DisputeStatus.NEW);
		testDispute.setViolationTicket(new ViolationTicket());
		testDispute.setContactTypeCd(ContactType.INDIVIDUAL);
		return testDispute;
	}
}
