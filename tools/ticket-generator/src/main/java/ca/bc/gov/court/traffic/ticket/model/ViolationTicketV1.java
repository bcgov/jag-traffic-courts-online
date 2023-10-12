package ca.bc.gov.court.traffic.ticket.model;

import io.swagger.v3.oas.annotations.media.Schema;

public class ViolationTicketV1 extends BaseViolationTicket {

	@Schema(example = "X", description = "Specify X to indicate a checkbox, blank otherwise")
	private String isYoungPerson;

	@Schema(example = " ", description = "Specify X to indicate a checkbox, space otherwise")
	private String isChangeOfAddress;

	@Schema(example = " ", description = "Specify X to indicate a checkbox, blank otherwise")
	private String offenseIsMCA;

	@Schema(example = " ", description = "Specify X to indicate a checkbox, blank otherwise")
	private String offenseIsFAA;

	@Schema(example = " ", description = "Specify X to indicate a checkbox, blank otherwise")
	private String offenseIsLCA;

	@Schema(example = " ", description = "Specify X to indicate a checkbox, blank otherwise")
	private String offenseIsTCR;

	@Schema(example = "MVA")
	private String count1ActReg;

	public String getIsYoungPerson() {
		return isYoungPerson;
	}

	public void setIsYoungPerson(String isYoungPerson) {
		this.isYoungPerson = isYoungPerson;
	}

	public String getIsChangeOfAddress() {
		return isChangeOfAddress;
	}

	public void setIsChangeOfAddress(String isChangeOfAddress) {
		this.isChangeOfAddress = isChangeOfAddress;
	}

	public String getOffenseIsMCA() {
		return offenseIsMCA;
	}

	public void setOffenseIsMCA(String offenseIsMCA) {
		this.offenseIsMCA = offenseIsMCA;
	}

	public String getOffenseIsFAA() {
		return offenseIsFAA;
	}

	public void setOffenseIsFAA(String offenseIsFAA) {
		this.offenseIsFAA = offenseIsFAA;
	}

	public String getOffenseIsLCA() {
		return offenseIsLCA;
	}

	public void setOffenseIsLCA(String offenseIsLCA) {
		this.offenseIsLCA = offenseIsLCA;
	}

	public String getOffenseIsTCR() {
		return offenseIsTCR;
	}

	public void setOffenseIsTCR(String offenseIsTCR) {
		this.offenseIsTCR = offenseIsTCR;
	}

	public String getCount1ActReg() {
		return count1ActReg;
	}

	public void setCount1ActReg(String count1ActReg) {
		this.count1ActReg = count1ActReg;
	}

}
