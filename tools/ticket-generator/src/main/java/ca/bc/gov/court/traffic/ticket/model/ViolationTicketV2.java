package ca.bc.gov.court.traffic.ticket.model;

import io.swagger.v3.oas.annotations.media.Schema;

public class ViolationTicketV2 extends BaseViolationTicket {

	@Schema(example = "Y", description = "Specify Y to indicate Yes, N otherwise")
	private String isYoungPerson;

	@Schema(example = "N", description = "Specify Y to indicate Yes, N otherwise")
	private String isChangeOfAddress;

	@Schema(example = " ", description = "Specify X to indicate a checkbox, blank otherwise")
	private String offenseIsMVAR;

	@Schema(example = " ", description = "Specify X to indicate a checkbox, blank otherwise")
	private String offenseIsLCLA;

	@Schema(example = " ", description = "Specify X to indicate a checkbox, blank otherwise")
	private String offenseIsFVPA;

	@Schema(example = " ", description = "Specify X to indicate a checkbox, blank otherwise")
	private String offenseIsCCLA;

	@Schema(example = " ", description = "Specify X to indicate a checkbox, blank otherwise")
	private String offenseIsTCSR;

	@Schema(example = "N", description = "Specify Y to indicate Yes, N otherwise")
	private String isAccident;

	@Schema(example = "Sergeant Smith")
	private String witnessingEnforcementOfficerName;

	@Schema(example = "12345")
	private String witnessingEnforcementOfficerNumber;

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

	public String getOffenseIsMVAR() {
		return offenseIsMVAR;
	}

	public void setOffenseIsMVAR(String offenseIsMVAR) {
		this.offenseIsMVAR = offenseIsMVAR;
	}

	public String getOffenseIsLCLA() {
		return offenseIsLCLA;
	}

	public void setOffenseIsLCLA(String offenseIsLCLA) {
		this.offenseIsLCLA = offenseIsLCLA;
	}

	public String getOffenseIsFVPA() {
		return offenseIsFVPA;
	}

	public void setOffenseIsFVPA(String offenseIsFVPA) {
		this.offenseIsFVPA = offenseIsFVPA;
	}

	public String getOffenseIsCCLA() {
		return offenseIsCCLA;
	}

	public void setOffenseIsCCLA(String offenseIsCCLA) {
		this.offenseIsCCLA = offenseIsCCLA;
	}

	public String getOffenseIsTCSR() {
		return offenseIsTCSR;
	}

	public void setOffenseIsTCSR(String offenseIsTCSR) {
		this.offenseIsTCSR = offenseIsTCSR;
	}

	public String getIsAccident() {
		return isAccident;
	}

	public void setIsAccident(String isAccident) {
		this.isAccident = isAccident;
	}

	public String getWitnessingEnforcementOfficerName() {
		return witnessingEnforcementOfficerName;
	}

	public void setWitnessingEnforcementOfficerName(String witnessingEnforcementOfficerName) {
		this.witnessingEnforcementOfficerName = witnessingEnforcementOfficerName;
	}

	public String getWitnessingEnforcementOfficerNumber() {
		return witnessingEnforcementOfficerNumber;
	}

	public void setWitnessingEnforcementOfficerNumber(String witnessingEnforcementOfficerNumber) {
		this.witnessingEnforcementOfficerNumber = witnessingEnforcementOfficerNumber;
	}

}
