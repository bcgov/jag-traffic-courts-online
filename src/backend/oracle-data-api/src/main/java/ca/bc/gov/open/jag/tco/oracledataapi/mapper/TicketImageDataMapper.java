package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.factory.Mappers;

import ca.bc.gov.open.jag.tco.oracledataapi.model.TicketImageData;
import ca.bc.gov.open.jag.tco.oracledataapi.model.TicketImageDataJustinDocument;
import ca.bc.gov.open.jag.tco.oracledataapi.model.TicketImageDataDocumentKey;

/**
 * This mapper maps from ORDS TicketImageDataGetResponseResult model to Oracle Data API TicketImageData model, and reverse for parameters which 
 * go in body of request
 */
@Mapper
(componentModel = "spring", injectionStrategy = InjectionStrategy.CONSTRUCTOR) // This is required for tests to work
public interface TicketImageDataMapper {

	TicketImageDataMapper INSTANCE = Mappers.getMapper(TicketImageDataMapper.class);

	// Map Image data from ORDS to Oracle Data API JJDisputeImageData model
	@Mapping(source = "create_date", target = "createDate")
	@Mapping(source = "version", target = "version")
	@Mapping(source = "documents", target = "documents")
	static
	TicketImageData convert(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.TicketImageDataGetResponseResult ticketImageDataGetResponseResult) {
		// TODO Auto-generated method stub
		return null;
	}
	
	// Map Image document from ORDS to Oracle Data API JJDisputeImageDocument model
	@Mapping(source = "reportType", target = "reportType", qualifiedByName="mapReportType")
	@Mapping(source = "reportFormat", target = "reportFormat")
	@Mapping(source = "partId", target = "partId")
	@Mapping(source = "participantName", target = "participantName")
	@Mapping(source = "index", target = "index")
	@Mapping(source = "data", target = "data")
	static
	TicketImageDataJustinDocument convert(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.TicketImageDataJustinDocument ticketImageDocument) {
		return null;
	};
	
	// Map parameters from Oracle Data API TicketImageDataDocumentKey to ORDS API TicketImageDataDocumentKey
	@Mapping(source = "rccId", target="rccId")
	@Mapping(source = "reportTypes", target="reportTypes", qualifiedByName="map_report_types")
	static
	ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.TicketImageDataDocumentKey convert(TicketImageDataDocumentKey ticketImageDocumentKey) {
		// TODO Auto-generated method stub
		return null;
	}
}
