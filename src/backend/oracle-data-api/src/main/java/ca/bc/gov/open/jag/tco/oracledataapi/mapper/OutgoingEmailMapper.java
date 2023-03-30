package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.factory.Mappers;

import ca.bc.gov.open.jag.tco.oracledataapi.model.EmailHistory;

/**
 * This mapper maps from ORDS OutgoingEmail model to Oracle Data API EmailHistory model and vice versa
 */
@Mapper
(componentModel = "spring", injectionStrategy = InjectionStrategy.CONSTRUCTOR) // This is required for tests to work
public interface OutgoingEmailMapper {

	OutgoingEmailMapper INSTANCE = Mappers.getMapper(OutgoingEmailMapper.class);

	// Map OutgoingEmail data from ORDS to Oracle Data API EmailHistory model
	@Mapping(source = "outgoingEmailId", target = "emailHistoryId")
	@Mapping(source = "emailSentDtm", target = "emailSentTs")
	@Mapping(source = "emailSubjectTxt", target = "subject")
	@Mapping(source = "successfullySentYn", target = "successfullySent")
	@Mapping(source = "entUserId", target = "createdBy")
	@Mapping(source = "entDtm", target = "createdTs")
	@Mapping(source = "updUserId", target = "modifiedBy")
	@Mapping(source = "updDtm", target = "modifiedTs")
	EmailHistory convert(ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.OutgoingEmail outgoingEmail);

	// Map Oracle Data API EmailHistory model to ORDS OutgoingEmail data
	@Mapping(target = "outgoingEmailId", source = "emailHistoryId")
	@Mapping(target = "emailSentDtm", source = "emailSentTs")
	@Mapping(target = "emailSubjectTxt", source = "subject")
	@Mapping(target = "successfullySentYn", source = "successfullySent")
	@Mapping(target = "entUserId", source = "createdBy")
	@Mapping(target = "entDtm", source = "createdTs")
	@Mapping(target = "updUserId", source = "modifiedBy")
	@Mapping(target = "updDtm", source = "modifiedTs")
	ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.OutgoingEmail convert(EmailHistory emailHistory);
}
