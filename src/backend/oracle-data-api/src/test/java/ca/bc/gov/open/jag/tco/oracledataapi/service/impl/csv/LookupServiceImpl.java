package ca.bc.gov.open.jag.tco.oracledataapi.service.impl.csv;

import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.List;

import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.stereotype.Service;

import com.opencsv.CSVReaderBuilder;

import ca.bc.gov.open.jag.tco.oracledataapi.model.Language;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Statute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Agency;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.handler.ApiException;
import ca.bc.gov.open.jag.tco.oracledataapi.service.impl.BaseLookupService;

@Service
@ConditionalOnProperty(name = "repository.lookup", havingValue = "csv", matchIfMissing = false)
public class LookupServiceImpl extends BaseLookupService {

	@Override
	public List<Statute> getStatutes() {
		List<Statute> statutes = new ArrayList<Statute>();
		try (InputStream stream = getClass().getClassLoader().getResourceAsStream("data/statutes.csv");
			BufferedReader reader = new BufferedReader(new InputStreamReader(stream))) {
			for (String[] row : new CSVReaderBuilder(reader).withSkipLines(1).build()) {
				Statute statute = new Statute();
				statute.setId(row.length > 0 ? row[0] : null);
				statute.setActCode(row.length > 1 ? row[1] : null);
				statute.setSectionText(row.length > 2 ? row[2] : null);
				statute.setSubsectionText(row.length > 3 ? row[3] : null);
				statutes.add(statute);
			}
		}
		catch (Exception e) {
			log.error("Could not read statutes.csv", e);
			return new ArrayList<Statute>();
		}
		return statutes;
	}

	@Override
	public List<Language> getLanguages() throws ApiException {
		List<Language> languages = new ArrayList<Language>();
		try (InputStream stream = getClass().getClassLoader().getResourceAsStream("data/languages.csv");
			BufferedReader reader = new BufferedReader(new InputStreamReader(stream))) {
			for (String[] row : new CSVReaderBuilder(reader).withSkipLines(1).build()) {
				Language language = new Language();
				language.setCode(row.length > 0 ? row[0] : null);
				language.setDescription(row.length > 1 ? row[1] : null);
				languages.add(language);
			}
		}
		catch (Exception e) {
			log.error("Could not read languages.csv", e);
			return new ArrayList<Language>();
		}
		return languages;
	}
	
	@Override
	public List<Agency> getAgencies() throws ApiException {
		List<Agency> agencies = new ArrayList<Agency>();
		try (InputStream stream = getClass().getClassLoader().getResourceAsStream("data/agencies.csv");
			BufferedReader reader = new BufferedReader(new InputStreamReader(stream))) {
			for (String[] row : new CSVReaderBuilder(reader).withSkipLines(1).build()) {
				Agency agency = new Agency();
				agency.setId(row.length > 0 ? row[0] : null);
				agency.setName(row.length > 1 ? row[1] : null);
				agency.setTypeCode(row.length > 2 ? row[2] : null);
				agencies.add(agency);
			}
		}
		catch (Exception e) {
			log.error("Could not read agencies.csv", e);
			return new ArrayList<Agency>();
		}
		return agencies;
	}
}
