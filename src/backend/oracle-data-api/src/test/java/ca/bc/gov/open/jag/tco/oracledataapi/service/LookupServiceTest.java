package ca.bc.gov.open.jag.tco.oracledataapi.service;

import static org.assertj.core.api.Assertions.assertThat;
import static org.mockito.Mockito.when;

import java.util.ArrayList;
import java.util.List;

import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.mock.mockito.MockBean;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.api.LookupValuesApi;
import ca.bc.gov.open.jag.tco.oracledataapi.api.handler.ApiException;
import ca.bc.gov.open.jag.tco.oracledataapi.api.model.Statute;
import ca.bc.gov.open.jag.tco.oracledataapi.api.model.StatutesListResult;

public class LookupServiceTest extends BaseTestSuite {

	@MockBean
	private LookupValuesApi lookupValuesApi;

	@Autowired
	private LookupService service;

	@Test
	public void testGetAllStatutes() throws ApiException {
		if ("ords".equals(lookupRepositorySrc)) {
			List<Statute> statutes = new ArrayList<>();
			Statute statute = new Statute();
			statute.setStatId("20153");
			statute.setActCd("MVA");
			statute.setStatSectionTxt("10");
			statute.setStatSubSectionTxt("1");
			statute.setStatParagraphTxt(null);
			statute.setStatSubParagraphTxt(null);
			statute.setStatCode("MVA 10(1) ");
			statute.setStatShortDescriptionTxt("Special licence for tractors, etc.");
			statute.setStatDescriptionTxt(null);
			statutes.add(statute);

			StatutesListResult statutesListResult = new StatutesListResult();
			statutesListResult.setStatuteCodeValues(statutes);
			when(this.lookupValuesApi.statutesList()).thenReturn(statutesListResult);
		}

		var result = service.getAllStatutes();

		assertThat(result).isNotNull();
		assertThat(result.size() > 0);
		assertThat("20153".equals(result.get(0).getId()));
	}

}
