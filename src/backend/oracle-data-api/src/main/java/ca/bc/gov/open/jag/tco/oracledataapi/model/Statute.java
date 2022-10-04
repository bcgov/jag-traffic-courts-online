package ca.bc.gov.open.jag.tco.oracledataapi.model;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class Statute {

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
