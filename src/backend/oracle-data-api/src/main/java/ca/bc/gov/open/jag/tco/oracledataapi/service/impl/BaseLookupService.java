package ca.bc.gov.open.jag.tco.oracledataapi.service.impl;

import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.redis.core.RedisTemplate;

import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.handler.ApiException;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Language;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Statute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Agency;
import ca.bc.gov.open.jag.tco.oracledataapi.service.LookupService;
import io.swagger.v3.core.util.Json;

public abstract class BaseLookupService implements LookupService {

	protected static Logger log = LoggerFactory.getLogger(LookupService.class);

	private static final String STATUTES = "Statutes";
	private static final String LANGUAGES = "Languages";
	private static final String AGENCIES = "Agencies";

	@Autowired
	private RedisTemplate<String, String> redis;

	@Override
	public void refresh() {
		log.debug("Refreshing code tables in redis.");

		try {
			// replace the Statutes key with a new json-serialized version of the statutes list.
			log.debug("  refreshing Statutes...");
			redis.opsForValue().set(STATUTES, Json.pretty(getStatutes()));

			// replace the Languages key with a new json-serialized version of the languages list.
			log.debug("  refreshing Languages...");
			redis.opsForValue().set(LANGUAGES, Json.pretty(getLanguages()));
			
			// replace the Agencies key with a new json-serialized version of the agencies list.
			log.debug(" refreshing Agencies...");
			redis.opsForValue().set(AGENCIES, Json.pretty(getAgencies()));

			log.debug("Code tables in redis refreshed.");
		} catch (Exception e) {
			log.error("Could not update code tables in redis", e);
		}
	}

	@Override
	public abstract List<Statute> getStatutes() throws ApiException;

	@Override
	public abstract List<Language> getLanguages() throws ApiException;
	
	@Override
	public abstract List<Agency> getAgencies() throws ApiException;

}
