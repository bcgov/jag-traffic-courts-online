package ca.bc.gov.open.jag.tco.oracledataapi.service.impl.ords;

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.stereotype.Service;

import ca.bc.gov.open.jag.tco.oracledataapi.api.LookupValuesApi;
import ca.bc.gov.open.jag.tco.oracledataapi.api.handler.ApiException;
import ca.bc.gov.open.jag.tco.oracledataapi.mapper.StatuteMapper;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Statute;
import ca.bc.gov.open.jag.tco.oracledataapi.service.impl.BaseLookupService;

@Service
@ConditionalOnProperty(name = "ords.enabled", havingValue = "true", matchIfMissing = false)
public class LookupServiceImpl extends BaseLookupService {

	@Autowired
	private LookupValuesApi lookupValuesApi;

	@Override
	public List<Statute> getAllStatutes() throws ApiException {
		// Get all statutes from the ORDS webclient service and convert them to OracleDataApi using Mapstruct
		List<ca.bc.gov.open.jag.tco.oracledataapi.api.model.Statute> statutes = lookupValuesApi.statutesList().getStatuteCodeValues();
		return StatuteMapper.INSTANCE.convertStatutes(statutes);
	}

}
