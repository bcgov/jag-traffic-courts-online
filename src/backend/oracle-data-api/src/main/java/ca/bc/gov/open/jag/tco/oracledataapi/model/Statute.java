package ca.bc.gov.open.jag.tco.oracledataapi.model;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * A Statute is a Violation Ticket Fine Regulation as dictated by the BC Government.
 */
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class Statute {

	private String statId;

	private String actCd;

	private String statSectionTxt;

	private String statSubSectionTxt;
	
	private String statParagraphTxt;
	
	private String statSubParagraphTxt;
	
	private String statCode;
	
	private String statShortDescriptionTxt;
	
	private String statDescriptionTxt;

}
