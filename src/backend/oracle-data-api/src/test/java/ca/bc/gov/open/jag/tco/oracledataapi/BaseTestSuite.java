package ca.bc.gov.open.jag.tco.oracledataapi;

import static org.junit.jupiter.api.Assertions.assertNotNull;

import java.io.UnsupportedEncodingException;
import java.net.URI;
import java.security.Principal;
import java.util.ArrayList;
import java.util.Date;
import java.util.Iterator;
import java.util.List;

import javax.transaction.Transactional;

import org.apache.commons.lang3.builder.Diff;
import org.apache.commons.lang3.builder.ReflectionDiffBuilder;
import org.apache.commons.lang3.builder.ToStringStyle;
import org.apache.commons.lang3.time.DateFormatUtils;
import org.assertj.core.util.Arrays;
import org.junit.jupiter.api.BeforeEach;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.test.autoconfigure.web.servlet.AutoConfigureMockMvc;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.boot.test.web.client.TestRestTemplate;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.http.HttpMethod;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.core.context.SecurityContextHolder;
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

import ca.bc.gov.open.jag.tco.oracledataapi.model.CustomUserDetails;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputeRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.security.PreAuthenticatedToken;

@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
@AutoConfigureMockMvc(addFilters = false)
@ActiveProfiles("test")
@Transactional
public class BaseTestSuite {

	@Autowired
	protected TestRestTemplate restTemplate;

	@Autowired
	protected MockMvc mvc;

	@Value("${repository.dispute}")
    protected String disputeRepositorySrc;

	@Value("${repository.jjdispute}")
    protected String jjdisputeRepositorySrc;

	@Value("${repository.lookup}")
    protected String lookupRepositorySrc;

	@Autowired
	protected DisputeRepository disputeRepository;

	@Autowired
	protected JJDisputeRepository jjDisputeRepository;

	@BeforeEach
	protected void beforeEach() throws Exception {
		// only delete the repo if this is a local H2 repository.
		if ("h2".equals(disputeRepositorySrc)) {
			disputeRepository.deleteAll();
		}
		if ("h2".equals(jjdisputeRepositorySrc)) {
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

	/** Returns the currently logged in user */
	protected Principal getPrincipal() {
		return SecurityContextHolder.getContext().getAuthentication();
	}

	/** Sets the currently logged in user */
	protected void setPrincipal(String username) {
		List<GrantedAuthority> authority = new ArrayList<>();
        authority.add(new SimpleGrantedAuthority("User"));

		CustomUserDetails user = new CustomUserDetails(username == null ? "System" : username, username == null ? "Password" : username, "System", "System", authority);
		Authentication authentication = new PreAuthenticatedToken(user);
		SecurityContextHolder.getContext().setAuthentication(authentication);
	}

	/** Serializes the given object to a json string. */
	protected String asJsonString(final Object obj) {
	    try {
	        return new ObjectMapper().writeValueAsString(obj);
	    } catch (Exception e) {
	        throw new RuntimeException(e);
	    }
	}

	protected <T> List<Diff<?>> getDifferences(T lhs, T rhs, String... ignoredFields) {
		List<Diff<?>> diffs = new ArrayList<Diff<?>>(new ReflectionDiffBuilder<T>(lhs, rhs, ToStringStyle.JSON_STYLE).build().getDiffs());
		List<Object> skippedFields = Arrays.asList(ignoredFields);

		for (Iterator<Diff<?>> iterator = diffs.iterator(); iterator.hasNext();) {
			Diff<?> diff = iterator.next();

			// skip known field differences and child objects
			if (skippedFields.contains(diff.getFieldName())) {
				iterator.remove();
			}
		}

		return diffs;
	}

	protected void logDiffs(List<Diff<?>> diffs, String objectClass) {
		if (!diffs.isEmpty()) {
			System.out.println("\n" + objectClass + " Diffs:");
			for (Diff<?> diff : diffs) {
				logDiff(diff);
			}
		}
	}

	protected void logDiff(Diff<?> diff) {
		if (diff.getKey() != null && diff.getKey() instanceof Date) {
			StringBuffer sb = new StringBuffer();
			sb.append("[");
			sb.append(diff.getFieldName());
			sb.append(": ");
			if (diff.getFieldName().endsWith("Ts")) {
				sb.append(diff.getLeft() == null ? "null" : DateFormatUtils.ISO_8601_EXTENDED_DATETIME_TIME_ZONE_FORMAT.format(diff.getLeft()));
				sb.append(", ");
				sb.append(diff.getRight() == null ? "null" : DateFormatUtils.ISO_8601_EXTENDED_DATETIME_TIME_ZONE_FORMAT.format(diff.getRight()));
			}
			else {
				sb.append(diff.getLeft() == null ? "null" : DateFormatUtils.ISO_8601_EXTENDED_DATE_FORMAT.format(diff.getLeft()));
				sb.append(", ");
				sb.append(diff.getRight() == null ? "null" : DateFormatUtils.ISO_8601_EXTENDED_DATE_FORMAT.format(diff.getRight()));
			}
			sb.append("]");
			System.out.println(sb);
		}
		else {
			System.out.println(diff.toString());
		}
	}

}
