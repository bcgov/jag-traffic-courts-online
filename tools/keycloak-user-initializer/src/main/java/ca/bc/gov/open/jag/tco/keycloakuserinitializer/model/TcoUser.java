package ca.bc.gov.open.jag.tco.keycloakuserinitializer.model;

import com.opencsv.bean.CsvBindByPosition;

import lombok.Getter;
import lombok.Setter;
import lombok.ToString;

/**
 * @author 237563
 * 
 * Represents the user and associated group data read from the csv file
 *
 */
@Getter
@Setter
@ToString
public class TcoUser {
	
	@CsvBindByPosition(position = 0)
	private String email;
	
	@CsvBindByPosition(position = 1)
	private Boolean realmAdmin;
	
	@CsvBindByPosition(position = 2)
	private Boolean adminJudicialJustice;
	
	@CsvBindByPosition(position = 3)
	private Boolean adminVtcStaff;
	
	@CsvBindByPosition(position = 4)
	private Boolean judicialJustice;
	
	@CsvBindByPosition(position = 5)
	private Boolean vtcStaff;

}
