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
public class Country {

	private String ctryId;
	
	private String ctryLongNm;
	
}
