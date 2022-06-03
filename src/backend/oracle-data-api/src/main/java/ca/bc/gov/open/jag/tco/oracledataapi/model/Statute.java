package ca.bc.gov.open.jag.tco.oracledataapi.model;

import org.springframework.data.annotation.Id;

import com.opencsv.bean.CsvBindByPosition;

import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * A Statute is a Violation Ticket Fine Regulation as dictated by the BC Government.
 */
@Getter
@Setter
@NoArgsConstructor
public class Statute {

	@Id
	@CsvBindByPosition(position = 0)
	private Integer code;

	@CsvBindByPosition(position = 1)
	private String act;

	@CsvBindByPosition(position = 2)
	private String section;

	@CsvBindByPosition(position = 3)
	private String description;

}
