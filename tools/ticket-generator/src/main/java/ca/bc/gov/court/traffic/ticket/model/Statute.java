package ca.bc.gov.court.traffic.ticket.model;

import com.opencsv.bean.CsvBindByPosition;

/**
 * A Statute is a Violation Ticket Fine Regulation as dictated by the BC Government.
 */
public class Statute {

	@CsvBindByPosition(position = 0)
	private Integer code;

	@CsvBindByPosition(position = 1)
	private String act;

	@CsvBindByPosition(position = 2)
	private String section;

	@CsvBindByPosition(position = 3)
	private String description;

	public Integer getCode() {
		return code;
	}

	public void setCode(Integer code) {
		this.code = code;
	}

	public String getAct() {
		return act;
	}

	public void setAct(String act) {
		this.act = act;
	}

	public String getSection() {
		return section;
	}

	public void setSection(String section) {
		this.section = section;
	}

	public String getDescription() {
		return description;
	}

	public void setDescription(String description) {
		this.description = description;
	}

}
