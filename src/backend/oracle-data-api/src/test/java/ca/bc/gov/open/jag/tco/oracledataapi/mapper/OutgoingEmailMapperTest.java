package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import static org.junit.jupiter.api.Assertions.assertEquals;

import java.util.UUID;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.OutgoingEmail;
import ca.bc.gov.open.jag.tco.oracledataapi.model.EmailHistory;
import ca.bc.gov.open.jag.tco.oracledataapi.model.YesNo;
import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;

public class OutgoingEmailMapperTest extends BaseTestSuite{

	private OutgoingEmailMapper outgoingEmailMapper;
	private RandomUtil randomUtil;
	
	@Autowired
	private OutgoingEmailMapperImpl outgoingEmailMapperImpl;

	@BeforeEach
	void setUp() {
		outgoingEmailMapper = outgoingEmailMapperImpl;
	}

	@Test
	void testMapToOutgoingEmail() {
		long emailHistoryId = 3L;
		long occamDisputeId = 5L;
		String subject = UUID.randomUUID().toString();
		String plainTextContent = UUID.randomUUID().toString();
		String fromEmailAddress = RandomUtil.randomEmailAddress();
		String toEmailAddress = RandomUtil.randomEmailAddress();
				
		EmailHistory emailHistory = new EmailHistory();
		emailHistory.setEmailHistoryId(emailHistoryId);
		emailHistory.setOccamDisputeId(occamDisputeId);
		emailHistory.setSubject(subject);
		emailHistory.setPlainTextContent(plainTextContent);
		emailHistory.setSuccessfullySent(YesNo.N);
		emailHistory.setFromEmailAddress(fromEmailAddress);
		emailHistory.setToEmailAddress(toEmailAddress);

		// test conversion to ORDS outoingEmail
		ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.OutgoingEmail result = outgoingEmailMapper.convert(emailHistory);
		assertEquals(emailHistoryId, Long.valueOf(result.getOutgoingEmailId()));
		assertEquals(occamDisputeId, Long.valueOf(result.getOccamDisputeId()));
		assertEquals(subject, result.getEmailSubjectTxt());
		assertEquals(plainTextContent, result.getPlainTextContent());
		assertEquals(fromEmailAddress, result.getFromEmailAddress());
		assertEquals(toEmailAddress, result.getToEmailAddress());
		
		OutgoingEmail outgoingEmail = new OutgoingEmail();
		outgoingEmail.setOutgoingEmailId(String.valueOf(emailHistoryId));
		outgoingEmail.setOccamDisputeId(String.valueOf(occamDisputeId));
		outgoingEmail.setEmailSubjectTxt(subject);
		outgoingEmail.setPlainTextContent(plainTextContent);
		outgoingEmail.setFromEmailAddress(fromEmailAddress);
		outgoingEmail.setToEmailAddress(toEmailAddress);
		
		// test conversion to EmailHistory model
		EmailHistory emailHistoryResult = outgoingEmailMapper.convert(outgoingEmail);
		assertEquals(emailHistoryId, emailHistoryResult.getEmailHistoryId());
		assertEquals(occamDisputeId, emailHistoryResult.getOccamDisputeId());
		assertEquals(subject, emailHistoryResult.getSubject());
		assertEquals(plainTextContent, emailHistoryResult.getPlainTextContent());
		assertEquals(fromEmailAddress, emailHistoryResult.getFromEmailAddress());
		assertEquals(toEmailAddress, emailHistoryResult.getToEmailAddress());
	}
}
