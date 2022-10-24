package ca.bc.gov.open.jag.tco.oracledataapi;

import static org.junit.jupiter.api.Assertions.assertNotNull;

import java.io.UnsupportedEncodingException;
import java.net.URI;
import java.security.Principal;

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
import org.springframework.test.web.servlet.MockMvc;
import org.springframework.test.web.servlet.ResultActions;
import org.springframework.test.web.servlet.request.MockHttpServletRequestBuilder;
import org.springframework.test.web.servlet.request.MockMvcRequestBuilders;
import org.springframework.web.util.UriComponentsBuilder;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.JsonMappingException;
import com.fasterxml.jackson.databind.ObjectMapper;

import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputeRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRepository;

@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
@AutoConfigureMockMvc(addFilters = false)
@ActiveProfiles("test")
@Transactional
public class BaseTestSuite {

	@Autowired
	protected TestRestTemplate restTemplate;

	@Autowired
	protected MockMvc mvc;

	@Value("${ords.enabled}")
	protected boolean ordsEnabled;

	@Autowired
	protected DisputeRepository disputeRepository;

	@Autowired
	protected JJDisputeRepository jjDisputeRepository;

	private Principal principal;

	@BeforeEach
	protected void beforeEach() throws Exception {
		// only delete the repo if this is a local H2 repository.
		if (!ordsEnabled) {
			disputeRepository.deleteAll();
			jjDisputeRepository.deleteAll();
		}

		setPrincipal("System");
	}

	/** Prepends the base restTemplate url to the path. */
	protected UriComponentsBuilder fromUriString(String url) {
		return UriComponentsBuilder.fromUriString("/api/v1.0" + url);
	}

    /**
     * Performs a GET request against the given uriBuilder
     * @param uriBuilder a the builder used to build a URI
     */
    protected ResultActions get(UriComponentsBuilder uriBuilder) throws Exception {
        assertNotNull(uriBuilder);
        return get(uriBuilder.build().encode().toUri());
    }

    /**
     * Performs a GET request against the given uri
     * @param uri
     */
	protected ResultActions get(URI uri) throws Exception {
		MockHttpServletRequestBuilder requestBuilder = MockMvcRequestBuilders.get(uri);
		return mvc.perform(requestBuilder);
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

	/** Extracts and maps the response body to the specified typeReference. */
	protected <T> T mapResult(ResultActions resultActions, TypeReference<T> typeReference) throws JsonMappingException, JsonProcessingException, UnsupportedEncodingException {
		return new ObjectMapper().readValue(resultActions.andReturn().getResponse().getContentAsString(), typeReference);
	}

	/** Helper method to return an instance of Principal */
	private Principal getPrincipal(String name) {
		return new Principal() {
			@Override
			public String getName() {
				return name;
			}
		};
	}

	/** Returns the currently logged in user */
	protected Principal getPrincipal() {
		if (principal == null) {
			principal = getPrincipal("System");
		}
		return principal;
	}

	/** Sets the currently logged in user */
	protected void setPrincipal(String name) {
		this.principal = getPrincipal(name);
	}

	/** Serializes the given object to a json string. */
	protected String asJsonString(final Object obj) {
	    try {
	        return new ObjectMapper().writeValueAsString(obj);
	    } catch (Exception e) {
	        throw new RuntimeException(e);
	    }
	}

}
