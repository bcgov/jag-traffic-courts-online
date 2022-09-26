package ca.bc.gov.open.jag.tco.oracledataapi.service;

import static org.assertj.core.api.Assertions.assertThat;
import static org.mockito.Mockito.when;

import java.util.ArrayList;
import java.util.List;

import org.junit.jupiter.api.Test;
import org.mockito.Mock;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.mock.mockito.MockBean;
import org.springframework.web.reactive.function.client.WebClient;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.model.GetStatutesListServiceResponse;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Statute;
import reactor.core.publisher.Mono;

public class LookupServiceTest extends BaseTestSuite {
	
	@MockBean 
	WebClient webClient;
	
	@SuppressWarnings("rawtypes")
	@Mock 
	WebClient.RequestHeadersUriSpec requestHeadersUriMock;
	@SuppressWarnings("rawtypes")
	@Mock 
	WebClient.RequestHeadersSpec requestHeadersMock;
	@Mock 
	WebClient.ResponseSpec responseMock;
	
	@Autowired
	LookupService service;

	@SuppressWarnings("unchecked")
	@Test
    public void testGetAllStatutes() {
		
		GetStatutesListServiceResponse statutesListServiceResponse = new GetStatutesListServiceResponse();
		
		List<Statute> statutes = new ArrayList<>();
		
		Statute statute = new Statute("20153", "MVA", "10", "1", null, null, "MVA 10(1) ", "Special licence for tractors, etc.", null);
		
		statutes.add(statute);
		
		statutesListServiceResponse.setStatuteCodeValues(statutes);
		
		when(this.webClient.get()).thenReturn(this.requestHeadersUriMock);
        when(this.requestHeadersUriMock.uri("/statutes")).thenReturn(this.requestHeadersMock);
        when(this.requestHeadersMock.retrieve()).thenReturn(this.responseMock);
        when(this.responseMock.bodyToMono(GetStatutesListServiceResponse.class)).thenReturn(Mono.just(statutesListServiceResponse));
        
        var result = service.getAllStatutes();
        
        assertThat(result).isNotNull();
        assertThat(result.size() > 0);
        assertThat("20153".equals(result.get(0).getStatId()));
        assertThat(result).usingRecursiveComparison().isEqualTo(statutes);
	}
}
