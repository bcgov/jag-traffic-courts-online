package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.factory.Mappers;

import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeImageData;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeImageDocument;

/**
 * This mapper maps from ORDS AuditLogEntry model to Oracle Data API FileHistory model and vice versa
 */
@Mapper
(componentModel = "spring", injectionStrategy = InjectionStrategy.CONSTRUCTOR) // This is required for tests to work
public interface JJDisputeImageDataMapper {

	JJDisputeImageDataMapper INSTANCE = Mappers.getMapper(JJDisputeImageDataMapper.class);

	// Map Image data from ORDS to Oracle Data API JJDisputeImageData model
	@Mapping(source = "create_date", target = "createDate")
	@Mapping(source = "version", target = "version")
	@Mapping(source = "documents", target = "documents")
	static
	JJDisputeImageData convert(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.TicketImageDataGetResponseResult ticketImageDataGetResponseResult) {
		// TODO Auto-generated method stub
		return null;
	}
	
	// Map Image document from ORDS to Oracle Data API JJDisputeImageDocument model
	@Mapping(source = "report_type", target = "reportType", qualifiedByName="mapReportType")
	@Mapping(source = "report_format", target = "reportFormat")
	@Mapping(source = "part_id", target = "partId")
	@Mapping(source = "participant_name", target = "participantName")
	@Mapping(source = "index", target = "index")
	@Mapping(source = "data", target = "data")
	JJDisputeImageDocument convert(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JustinDocument ticketImageDocument);
}
