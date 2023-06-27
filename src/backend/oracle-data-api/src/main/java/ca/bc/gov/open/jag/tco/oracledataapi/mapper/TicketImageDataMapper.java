package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.Named;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import ca.bc.gov.open.jag.tco.oracledataapi.model.TicketImageDataDocumentType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.TicketImageDataJustinDocument;
import ca.bc.gov.open.jag.tco.oracledataapi.service.DisputeService;

/**
 * This mapper maps from ORDS TicketImageDataJustinDocument model to Oracle Data API TicketImageDataJustinDocument model 
 * 
 */
@Mapper(componentModel = "spring", injectionStrategy = InjectionStrategy.CONSTRUCTOR) // This is required for tests to work
public abstract class TicketImageDataMapper extends BaseMapper {
	
	private Logger logger = LoggerFactory.getLogger(DisputeService.class);

	// Map Image document from ORDS to Oracle Data API JJDisputeImageDocument model
	@Mapping(source = "reportType", target = "reportType", qualifiedByName="mapReportType")
	@Mapping(source = "reportFormat", target = "reportFormat")
	@Mapping(source = "partId", target = "partId")
	@Mapping(source = "participantName", target = "participantName")
	@Mapping(source = "index", target = "index")
	@Mapping(source = "data", target = "fileData")
	public abstract TicketImageDataJustinDocument convert(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.TicketImageDataJustinDocument ticketImageDocument);
	
	@Named("mapReportType")
	protected TicketImageDataDocumentType mapReportType(String reportType) {
		TicketImageDataDocumentType[] values = TicketImageDataDocumentType.values();
		for (TicketImageDataDocumentType type : values) {
			if (type.getShortName().equals(reportType)) {
				return type;
			}
		}
		return null;
	}
}
