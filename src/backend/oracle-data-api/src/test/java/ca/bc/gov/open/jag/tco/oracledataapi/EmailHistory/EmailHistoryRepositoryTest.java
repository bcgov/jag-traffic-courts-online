package ca.bc.gov.open.jag.tco.oracledataapi.EmailHistory;

import ca.bc.gov.open.jag.tco.oracledataapi.mapper.OutgoingEmailMapperImpl;
import ca.bc.gov.open.jag.tco.oracledataapi.model.EmailHistory;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.OutgoingEmailApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.OutgoingEmail;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.OutgoingEmailListResponse;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.OutgoingEmailResponseResult;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.EmailHistoryRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.ords.EmailHistoryRepositoryImpl;
import ca.bc.gov.open.jag.tco.oracledataapi.service.EmailHistoryService;
import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.Mock;
import org.mockito.Mockito;
import org.mockito.MockitoAnnotations;

import java.util.List;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;

public class EmailHistoryRepositoryTest {

    private EmailHistoryRepositoryImpl sut;

    @Mock
    private OutgoingEmailApi apiMock;

    private OutgoingEmailMapperImpl mapper;

    @BeforeEach
    public void setUp() {
        MockitoAnnotations.openMocks(this);
        mapper = new OutgoingEmailMapperImpl();
        sut =new EmailHistoryRepositoryImpl(apiMock, mapper);
    }

    @Test
    public void findByTicketNumberTest() {
        String ticketNumber = RandomUtil.randomTicketNumber();
        OutgoingEmailListResponse response = new OutgoingEmailListResponse();
        OutgoingEmail outgoingEmail = RandomUtil.createOutgoingEmail();
        response.addOutgoingEmailsItem(outgoingEmail);
        Mockito.when(apiMock.v1OutgoingEmailListGet(ticketNumber)).thenReturn(response);

        List<EmailHistory>  controllerResponse = sut.findByTicketNumber(ticketNumber);
        Mockito.verify(apiMock).v1OutgoingEmailListGet(ticketNumber);
        assertNotNull(controllerResponse);

        EmailHistory emailHistory = controllerResponse.get(0);

        assertEquals(outgoingEmail.getOutgoingEmailId(), emailHistory.getEmailHistoryId().toString());
        assertEquals(outgoingEmail.getEmailSentDtm(), emailHistory.getEmailSentTs());
        assertEquals(outgoingEmail.getFromEmailAddress(), emailHistory.getFromEmailAddress());
        assertEquals(outgoingEmail.getToEmailAddress(), emailHistory.getToEmailAddress());
        assertEquals(outgoingEmail.getEntDtm(), emailHistory.getCreatedTs());
        assertEquals(outgoingEmail.getUpdUserId(), emailHistory.getModifiedBy());
        assertEquals(outgoingEmail.getUpdDtm(), emailHistory.getModifiedTs());
        assertEquals(outgoingEmail.getCcEmailAddress(), emailHistory.getCcEmailAddress());
        assertEquals(outgoingEmail.getBccEmailAddress(), emailHistory.getBccEmailAddress());
        assertEquals(outgoingEmail.getEmailSubjectTxt(), emailHistory.getSubject());
        assertEquals(outgoingEmail.getHtmlContent(), emailHistory.getHtmlContent());
        assertEquals(outgoingEmail.getPlainTextContent(), emailHistory.getPlainTextContent());
        assertEquals(outgoingEmail.getSuccessfullySentYn(), emailHistory.getSuccessfullySent().toString());
        assertEquals(outgoingEmail.getDisputeId(), emailHistory.getOccamDisputeId().toString());
    }

    @Test
    public void saveTest() {
        OutgoingEmailListResponse response = new OutgoingEmailListResponse();
        EmailHistory emailHistory = RandomUtil.createEmailHistory();
        OutgoingEmail outgoingEmail = mapper.convert(emailHistory);
        OutgoingEmailResponseResult outgoingEmailResponseResult = new OutgoingEmailResponseResult();
        outgoingEmailResponseResult.setOutgoingEmailId(emailHistory.getEmailHistoryId().toString());
        outgoingEmailResponseResult.setStatus("1");
        Mockito.when(apiMock.v1ProcessOutgoingEmailPost(outgoingEmail)).thenReturn(outgoingEmailResponseResult);

        Long controllerResponse = sut.save(emailHistory);

        assertEquals(controllerResponse, emailHistory.getEmailHistoryId());
    }

}
