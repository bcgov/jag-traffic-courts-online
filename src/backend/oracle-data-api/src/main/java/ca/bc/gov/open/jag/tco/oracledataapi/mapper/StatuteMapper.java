package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import java.util.List;

import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.factory.Mappers;

import ca.bc.gov.open.jag.tco.oracledataapi.dto.StatuteDTO;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Statute;

@Mapper
public interface StatuteMapper {
	
	StatuteMapper INSTANCE = Mappers.getMapper(StatuteMapper.class);
	
	@Mapping(source = "statId", target = "id")
	@Mapping(source = "actCd", target = "actCode")
	@Mapping(source = "statSectionTxt", target = "sectionText")
	@Mapping(source = "statSubSectionTxt", target = "subsectionText")
	@Mapping(source = "statParagraphTxt", target = "paragraphText")
	@Mapping(source = "statSubParagraphTxt", target = "subparagraphText")
	@Mapping(source = "statCode", target = "code")
	@Mapping(source = "statShortDescriptionTxt", target = "shortDescriptionText")
	@Mapping(source = "statDescriptionTxt", target = "descriptionText")
	StatuteDTO convertStatute(Statute statute);
	List<StatuteDTO> convertStatutes(List<Statute> statuteList);
}
