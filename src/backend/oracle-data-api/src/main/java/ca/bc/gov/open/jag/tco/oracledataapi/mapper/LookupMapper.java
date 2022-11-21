package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import java.util.List;

import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.factory.Mappers;

/**
 * Maps from ORDS lookup tables to OracleDataAPI
 */
@Mapper
public interface LookupMapper {

	LookupMapper INSTANCE = Mappers.getMapper(LookupMapper.class);

	/** Map from ORDS Statute to OracleDataAPI Statute */
	@Mapping(source = "statId", target = "id")
	@Mapping(source = "actCd", target = "actCode")
	@Mapping(source = "statSectionTxt", target = "sectionText")
	@Mapping(source = "statSubSectionTxt", target = "subsectionText")
	@Mapping(source = "statParagraphTxt", target = "paragraphText")
	@Mapping(source = "statSubParagraphTxt", target = "subparagraphText")
	@Mapping(source = "statCode", target = "code")
	@Mapping(source = "statShortDescriptionTxt", target = "shortDescriptionText")
	@Mapping(source = "statDescriptionTxt", target = "descriptionText")
	ca.bc.gov.open.jag.tco.oracledataapi.model.Statute convertStatute(ca.bc.gov.open.jag.tco.oracledataapi.api.model.Statute statute);
	List<ca.bc.gov.open.jag.tco.oracledataapi.model.Statute> convertStatutes(List<ca.bc.gov.open.jag.tco.oracledataapi.api.model.Statute> statuteList);

	/** Map from ORDS Language to OracleDataAPI Language */
	@Mapping(source = "cdlnLanguageCd", target = "code")
	@Mapping(source = "cdlnLanguageDsc", target = "description")
	ca.bc.gov.open.jag.tco.oracledataapi.model.Language convertLanguage(ca.bc.gov.open.jag.tco.oracledataapi.api.model.Language language);
	List<ca.bc.gov.open.jag.tco.oracledataapi.model.Language> convertLanguages(List<ca.bc.gov.open.jag.tco.oracledataapi.api.model.Language> languages);
}
