package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.factory.Mappers;

import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequest;

/**
 * This mapper maps from ORDS disputeUpdateRequest model to Oracle Data API disputeUpdateRequest model
 */
@Mapper
(componentModel = "spring", injectionStrategy = InjectionStrategy.CONSTRUCTOR) // This is required for tests to work
public interface DisputeUpdateRequestMapper {

	DisputeUpdateRequestMapper INSTANCE = Mappers.getMapper(DisputeUpdateRequestMapper.class);

	// Map disputeUpdateRequest data from ORDS to Oracle Data API disputeUpdateRequest model
	@Mapping(source = "disputeUpdateRequestId", target = "disputeUpdateRequestId")
	@Mapping(source = "disputeId", target = "disputeId")
	@Mapping(source = "disputeUpdateStatTypeCd", target = "status")
	@Mapping(source = "disputeUpdateReqTypeCd", target = "updateType")
	@Mapping(source = "requestJsonTxt", target = "updateJson")
	@Mapping(source = "entUserId", target = "createdBy")
	@Mapping(source = "entDtm", target = "createdTs")
	@Mapping(source = "updUserId", target = "modifiedBy")
	@Mapping(source = "updDtm", target = "modifiedTs")
	DisputeUpdateRequest convert(ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.DisputeUpdateRequest disputeUpdateRequest);
}
