package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.Named;
import org.mapstruct.factory.Mappers;

import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequest;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequestStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequestType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.ShortNamedEnum;

/**
 * This mapper maps from ORDS disputeUpdateRequest model to Oracle Data API disputeUpdateRequest model and vice versa
 */
@Mapper
(componentModel = "spring", injectionStrategy = InjectionStrategy.CONSTRUCTOR) // This is required for tests to work
public interface DisputeUpdateRequestMapper {

	DisputeUpdateRequestMapper INSTANCE = Mappers.getMapper(DisputeUpdateRequestMapper.class);

	// Map disputeUpdateRequest data from ORDS to Oracle Data API disputeUpdateRequest model
	@Mapping(source = "disputeUpdateRequestId", target = "disputeUpdateRequestId")
	@Mapping(source = "disputeId", target = "disputeId")
	@Mapping(source = "disputeUpdateStatTypeCd", target = "status", qualifiedByName="mapDisputeUpdateStatType")
	@Mapping(source = "disputeUpdateReqTypeCd", target = "updateType", qualifiedByName="mapDisputeUpdateReqType")
	@Mapping(source = "requestJsonTxt", target = "updateJson")
	@Mapping(source = "entUserId", target = "createdBy")
	@Mapping(source = "entDtm", target = "createdTs")
	@Mapping(source = "updUserId", target = "modifiedBy")
	@Mapping(source = "updDtm", target = "modifiedTs")
	DisputeUpdateRequest convert(ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.DisputeUpdateRequest disputeUpdateRequest);

	// Map Oracle Data API disputeUpdateRequest model to ORDS isputeUpdateRequest data
	@Mapping(target = "disputeUpdateRequestId", source = "disputeUpdateRequestId")
	@Mapping(target = "disputeId", source = "disputeId")
	@Mapping(target = "disputeUpdateStatTypeCd", source = "status", qualifiedByName="mapEnumToShortName")
	@Mapping(target = "disputeUpdateReqTypeCd", source = "updateType", qualifiedByName="mapEnumToShortName")
	@Mapping(target = "requestJsonTxt", source = "updateJson")
	@Mapping(target = "entUserId", source = "createdBy")
	@Mapping(target = "entDtm", source = "createdTs")
	@Mapping(target = "updUserId", source = "modifiedBy")
	@Mapping(target = "updDtm", source = "modifiedTs")
	ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.DisputeUpdateRequest convert(DisputeUpdateRequest disputeUpdateRequest);

	@Named("mapDisputeUpdateStatType")
	default DisputeUpdateRequestStatus mapDisputeUpdateStatType(String statusShortCd) {
		DisputeUpdateRequestStatus[] values = DisputeUpdateRequestStatus.values();
		for (DisputeUpdateRequestStatus status : values) {
			if (status.getShortName().equals(statusShortCd)) {
				return status;
			}
		}
		return null;
	}

	@Named("mapDisputeUpdateReqType")
	default DisputeUpdateRequestType mapDisputeUpdateReqType(String statusShortCd) {
		DisputeUpdateRequestType[] values = DisputeUpdateRequestType.values();
		for (DisputeUpdateRequestType type : values) {
			if (type.getShortName().equals(statusShortCd)) {
				return type;
			}
		}
		return null;
	}

	@Named("mapEnumToShortName")
	default String mapShortNamedEnum(ShortNamedEnum code) {
		return code.getShortName();
	}
}
