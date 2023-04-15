package ca.bc.gov.open.jag.tco.oracledataapi.service;

import static org.assertj.core.api.Assertions.assertThat;

import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.mock.mockito.MockBean;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.LookupValuesApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.handler.ApiException;

public class LookupServiceTest extends BaseTestSuite {

	@MockBean
	private LookupValuesApi lookupValuesApi;

	@Autowired
	private LookupService service;

	@Test
	public void testGetStatutes() throws ApiException {
		var result = service.getStatutes();

		assertThat(result).isNotNull();
		assertThat(result.size() > 0);
		assertThat("20153".equals(result.get(0).getId()));
	}

	@Test
	public void testGetLanguages() throws ApiException {
		var result = service.getLanguages();

		assertThat(result).isNotNull();
		assertThat(result.size() > 0);
		assertThat("ALB".equals(result.get(0).getCode()));
		assertThat("Albanian".equals(result.get(0).getDescription()));
	}

	@Test
	public void testGetAgencies() throws ApiException {
		var result = service.getAgencies();

		assertThat(result).isNotNull();
		assertThat(result.size() > 0);
		assertThat("18861.0045".equals(result.get(0).getId()));
		assertThat("Kelowna Adult Forensic Psychiatric Services".equals(result.get(0).getName()));
	}
	
	@Test
	public void testGetProinvices() throws ApiException {
		var result = service.getProvinces();
		
		assertThat(result).isNotNull();
		assertThat(result.size() > 0);
		assertThat("1".equals(result.get(0).getCtryId()));
		assertThat("1".equals(result.get(0).getProvSeqNo()));
		assertThat("British Columbia".equals(result.get(0).getProvNm()));
		assertThat("BC".equals(result.get(0).getProvAbbreviationCd()));
	}
}
