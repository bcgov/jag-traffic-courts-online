package ca.bc.gov.open.jag.tco.oracledataapi.service;

import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.redis.core.RedisTemplate;
import org.springframework.stereotype.Service;
import org.springframework.web.reactive.function.client.WebClient;

import ca.bc.gov.open.jag.tco.oracledataapi.dto.StatuteDTO;
import ca.bc.gov.open.jag.tco.oracledataapi.mapper.StatuteMapper;
import ca.bc.gov.open.jag.tco.oracledataapi.model.GetStatutesListServiceResponse;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Statute;
import io.swagger.v3.core.util.Json;

@Service
public class LookupService {

	private Logger log = LoggerFactory.getLogger(LookupService.class);
	private static final String STATUTES = "Statutes";
	private final WebClient webClient;
	
	public LookupService(WebClient webClient) {
		super();
		this.webClient = webClient;
	}

	@Autowired
	private RedisTemplate<String, String> redis;

	public void refresh() {
		log.debug("Refreshing code tables in redis.");

		try {
			// Get all statutes from the ORDS webclient service and convert them to DTO using Mapstruct
			List<StatuteDTO> statutes = StatuteMapper.INSTANCE.convertStatutes(getAllStatutes());
			
			String json = Json.pretty(statutes);

			// replace the Statutes key with a new json-serialized version of the statutes list.
			redis.opsForValue().set(STATUTES, json);
		} catch (Exception e) {
			log.error("Could not update redis", e);
		}
	}
	
	public List<Statute> getAllStatutes() {
		GetStatutesListServiceResponse response = 
				webClient
                .get()
                .uri("/statutes")
                .retrieve()
                .bodyToMono(GetStatutesListServiceResponse.class)
                .doOnSuccess(resp -> log.debug("Successfully returned the statutes {}, from ORDS", resp))
                .doOnError(exception -> log.error("Failed to return statutes from ORDS", exception))
                .block();
		
		return response.getStatuteCodeValues();
	}

}
