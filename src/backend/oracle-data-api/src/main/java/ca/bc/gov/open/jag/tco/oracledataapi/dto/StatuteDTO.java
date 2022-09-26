package ca.bc.gov.open.jag.tco.oracledataapi.dto;

import lombok.Builder;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
@Builder
public class StatuteDTO {
	
	private String id;

	private String actCode;

	private String sectionText;

	private String subsectionText;
	
	private String paragraphText;
	
	private String subparagraphText;
	
	private String code;
	
	private String shortDescriptionText;
	
	private String descriptionText;
}
