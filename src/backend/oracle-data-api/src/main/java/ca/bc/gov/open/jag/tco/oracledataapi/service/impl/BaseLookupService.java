package ca.bc.gov.open.jag.tco.oracledataapi.service.impl;

import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.redis.core.RedisTemplate;

import ca.bc.gov.open.jag.tco.oracledataapi.api.handler.ApiException;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Statute;
import ca.bc.gov.open.jag.tco.oracledataapi.service.LookupService;
import io.swagger.v3.core.util.Json;

public abstract class BaseLookupService implements LookupService {

	protected static Logger log = LoggerFactory.getLogger(LookupService.class);

	private static final String STATUTES = "Statutes";

	@Autowired
	private RedisTemplate<String, String> redis;

	@Override
	public void refresh() {
		log.debug("Refreshing code tables in redis.");

		try {
			List<Statute> statutes = getAllStatutes();
			String json = Json.pretty(statutes);

			// replace the Statutes key with a new json-serialized version of the statutes list.
			redis.opsForValue().set(STATUTES, json);

			log.debug("Code tables in redis refreshed.");
		} catch (Exception e) {
			log.error("Could not update redis", e);
		}
	}

	@Override
	public abstract List<Statute> getAllStatutes() throws ApiException;

}
