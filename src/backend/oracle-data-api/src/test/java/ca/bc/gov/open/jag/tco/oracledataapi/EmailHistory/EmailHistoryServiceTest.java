package ca.bc.gov.open.jag.tco.oracledataapi.EmailHistory;

import ca.bc.gov.open.jag.tco.oracledataapi.model.EmailHistory;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.EmailHistoryRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.service.EmailHistoryService;
import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.TestInstance;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.Mockito;
import org.mockito.MockitoAnnotations;

import java.util.ArrayList;
import java.util.List;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;

@TestInstance(TestInstance.Lifecycle.PER_CLASS)
public class EmailHistoryServiceTest {

    @InjectMocks
    private EmailHistoryService sut;
    @Mock
    private EmailHistoryRepository repositoryMock;

    @BeforeEach
    public void setUp() {
        MockitoAnnotations.openMocks(this);
    }

    @Test
    public void getEmailHistoryByTicketNumberTest() {
        String ticketNumber = RandomUtil.randomTicketNumber();
        EmailHistory emailHistory = RandomUtil.createEmailHistory();
        List<EmailHistory> emailHistorys = new ArrayList<>();
        emailHistorys.add(emailHistory);
        Mockito.when(repositoryMock.findByTicketNumber(ticketNumber)).thenReturn(emailHistorys);

        List<EmailHistory>  controllerResponse = sut.getEmailHistoryByTicketNumber(ticketNumber);
        Mockito.verify(repositoryMock).findByTicketNumber(ticketNumber);
        assertNotNull(controllerResponse);
        assertEquals(emailHistorys, controllerResponse);
    }

    @Test
    public void getEmailHistoryByTicketNumberReturn500() throws Exception {
        String ticketNumber = RandomUtil.randomTicketNumber();
        Mockito.when(repositoryMock.findByTicketNumber(ticketNumber)).thenThrow(RuntimeException.class);

        Assertions.assertThrows(RuntimeException.class, () -> {
            sut.getEmailHistoryByTicketNumber(ticketNumber);
        });
    }

    @Test
    public void insertEmailHistoryTest() {
        EmailHistory emailHistory = RandomUtil.createEmailHistory();
        Mockito.when(repositoryMock.save(emailHistory)).thenReturn(emailHistory.getEmailHistoryId());

        Long controllerResponse = sut.insertEmailHistory(emailHistory);
        Mockito.verify(repositoryMock).save(emailHistory);
        Long resultId = controllerResponse;
        assertNotNull(resultId);
    }

    @Test
    public void insertEmailHistoryReturn500() throws Exception {
        EmailHistory emailHistory = RandomUtil.createEmailHistory();
        Mockito.when(repositoryMock.save(emailHistory)).thenThrow(RuntimeException.class);

        Assertions.assertThrows(RuntimeException.class, () -> {
            sut.insertEmailHistory(emailHistory);
        });
    }
}
