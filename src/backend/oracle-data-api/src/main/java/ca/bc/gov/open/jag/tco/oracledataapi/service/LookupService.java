package ca.bc.gov.open.jag.tco.oracledataapi.service;

import java.io.BufferedReader;
import java.io.FileNotFoundException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.URISyntaxException;
import java.util.ArrayList;
import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.redis.core.RedisTemplate;
import org.springframework.stereotype.Service;

import com.opencsv.bean.CsvToBeanBuilder;

import ca.bc.gov.open.jag.tco.oracledataapi.model.Statute;
import io.swagger.v3.core.util.Json;

@Service
public class LookupService {

	private Logger log = LoggerFactory.getLogger(LookupService.class);
	private static final String STATUTES = "Statutes";

	@Autowired
	private RedisTemplate<String, String> redis;

	public void refresh() {
		log.debug("Refreshing code tables in redis.");

		try {
			List<Statute> statutes = getStatutes();
			String json = Json.pretty(statutes);

			// replace the Statutes key with a new json-serialized version of the statutes list.
			redis.opsForValue().set(STATUTES, json);
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
