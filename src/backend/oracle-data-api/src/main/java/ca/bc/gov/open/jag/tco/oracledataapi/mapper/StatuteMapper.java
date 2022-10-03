package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import java.util.List;

import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.factory.Mappers;

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
	/** Map from ORDS Statute to OracleDataAPI Statute */
	ca.bc.gov.open.jag.tco.oracledataapi.model.Statute convertStatute(ca.bc.gov.open.jag.tco.oracledataapi.api.model.Statute statute);
	List<ca.bc.gov.open.jag.tco.oracledataapi.model.Statute> convertStatutes(List<ca.bc.gov.open.jag.tco.oracledataapi.api.model.Statute> statuteList);
}
