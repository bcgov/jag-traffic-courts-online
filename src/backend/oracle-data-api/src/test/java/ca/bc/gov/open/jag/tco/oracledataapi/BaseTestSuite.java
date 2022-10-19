package ca.bc.gov.open.jag.tco.oracledataapi;

import javax.transaction.Transactional;

import org.junit.jupiter.api.BeforeEach;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.test.autoconfigure.web.servlet.AutoConfigureMockMvc;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.boot.test.web.client.TestRestTemplate;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.http.HttpMethod;
import org.springframework.http.ResponseEntity;
import org.springframework.test.context.ActiveProfiles;
import org.springframework.web.util.UriComponentsBuilder;

import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputeRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRepository;

@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
@AutoConfigureMockMvc(addFilters = false)
@ActiveProfiles("test")
@Transactional
public class BaseTestSuite {

	@Autowired
	protected TestRestTemplate restTemplate;

	@Value("${ords.enabled}")
	protected boolean ordsEnabled;

	@Autowired
	protected DisputeRepository disputeRepository;

	@Autowired
	protected JJDisputeRepository jjDisputeRepository;

	@BeforeEach
	protected void beforeEach() throws Exception {
		// only delete the repo if this is a local H2 repository.
		if (!ordsEnabled) {
			disputeRepository.deleteAll();
			jjDisputeRepository.deleteAll();
		}
	}

	/** Prepends the base restTemplate url to the path. */
	protected UriComponentsBuilder fromUriString(String url) {
		return UriComponentsBuilder.fromUriString("/api/v1.0" + url);
	}

	/** Issues a GET request to the given URL. */
	protected <T> ResponseEntity<T> getForEntity(UriComponentsBuilder builder, ParameterizedTypeReference<T> responseType) {
		return restTemplate.exchange(builder.build().encode().toUri(), HttpMethod.GET, null, responseType);
	}

	/** Issues a POST request to the given URL. */
	protected <T> ResponseEntity<T> postForEntity(UriComponentsBuilder builder, ParameterizedTypeReference<T> responseType) {
		return restTemplate.exchange(builder.build().encode().toUri(), HttpMethod.POST, null, responseType);
	}

	/** Issues a POST request to the given URL. */
	protected <T> T postForObject(UriComponentsBuilder builder, Object object, Class<T> responseType, Object... urlVariables) {
		return restTemplate.postForObject(builder.build().encode().toString(), object, responseType, urlVariables);
	}

}
