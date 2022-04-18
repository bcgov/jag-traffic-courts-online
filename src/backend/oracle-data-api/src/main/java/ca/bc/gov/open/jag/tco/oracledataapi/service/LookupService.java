package ca.bc.gov.open.jag.tco.oracledataapi.service;

import java.io.BufferedReader;
import java.io.FileNotFoundException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.URISyntaxException;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.function.Function;
import java.util.stream.Collectors;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.opencsv.bean.CsvToBeanBuilder;

import ca.bc.gov.open.jag.tco.oracledataapi.model.Statute;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.StatuteRepository;

@Service
public class LookupService {

	private Logger log = LoggerFactory.getLogger(LookupService.class);

	@Autowired
	private StatuteRepository statuteRepository;

	public void refresh() {
		log.debug("Refreshing code tables in redis.");

		try {
			List<Statute> newStatutes = getStatutes();

			// Index statutes by code
			Map<Integer, Statute> map = newStatutes.stream().collect(Collectors.toMap(Statute::getCode, Function.identity()));
			Set<Integer> newStatutesIds = map.keySet();

			// Remove statutes that are no longer in the list from Oracle
			Iterable<Statute> oldStatutes = statuteRepository.findAll();
			for (Statute statute : oldStatutes) {
				if (statute != null && !newStatutesIds.contains(statute.getCode())) {
					statuteRepository.delete(statute);
				}
			}

			// Insert/update all statutes with those from Oracle
			statuteRepository.saveAll(newStatutes);

		} catch (Exception e) {
			log.error("Could not update redis", e);
		}
	}

	private List<Statute> getStatutes() throws URISyntaxException, FileNotFoundException {
		try (InputStream stream = getClass().getClassLoader().getResourceAsStream("data/statutes.csv");
			BufferedReader reader = new BufferedReader(new InputStreamReader(stream))) {
			List<Statute> statutes = new CsvToBeanBuilder<Statute>(reader)
					.withType(Statute.class)
					.withSkipLines(1)
					.build()
					.parse();
			return statutes;
		}
		catch (Exception e) {
			log.error("Could not read statutes.csv", e);
			return new ArrayList<Statute>();
		}
	}

}
