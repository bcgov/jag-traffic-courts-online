package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import org.mapstruct.Mapper;
import org.mapstruct.factory.Mappers;

import ca.bc.gov.open.jag.tco.oracledataapi.api.model.ViolationTicket;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;

@Mapper
public interface ViolationTicketMapper {
	
	ViolationTicketMapper INSTANCE = Mappers.getMapper(ViolationTicketMapper.class);
	
	ViolationTicket convertDisputeToViolationTicketDto (Dispute dispute);
}
