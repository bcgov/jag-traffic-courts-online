package ca.bc.gov.court.traffic.ticket.model;

import java.util.Objects;

import org.apache.commons.lang3.StringUtils;

import com.fasterxml.jackson.annotation.JsonIgnore;

import ca.bc.gov.court.traffic.ticket.util.DateUtil;
import io.swagger.v3.oas.annotations.media.Schema;

public class ViolationTicket {

	@Schema(example = "AX00000000")
	private String violationTicketNumber;
	@Schema(example = "Kent")
	private String surname;
	@Schema(example = "Clark")
	private String givenName;
	@Schema(example = "X", description = "Specify X to indicate a checkbox, blank otherwise")
	private String isYoungPerson;
	@Schema(example = "BC")
	private String driversLicenceProvince;
	@Schema(example = "1234567")
	private String driversLicenceNumber;
	@Schema(example = "2020")
	private String driversLicenceCreated;
	@Schema(example = "2025")
	private String driversLicenceExpiry;
	@Schema(example = "2006-01-15")
	private String birthdate;
	@Schema(example = "123 Small Lane")
	private String address;
	@Schema(example = " ", description = "Specify X to indicate a checkbox, space otherwise")
	private String isChangeOfAddress;
	@Schema(example = "Smallville")
	private String city;
	@Schema(example = "BC")
	private String province;
	@Schema(example = "V9A1L8")
	private String postalCode;
	@Schema(example = "X", description = "Specify X to indicate a checkbox, space otherwise")
	private String namedIsDriver;
	@Schema(example = " ", description = "Specify X to indicate a checkbox, space otherwise")
	private String namedIsCyclist;
	@Schema(example = " ", description = "Specify X to indicate a checkbox, space otherwise")
	private String namedIsOwner;
	@Schema(example = " ", description = "Specify X to indicate a checkbox, space otherwise")
	private String namedIsPedestrain;
	@Schema(example = " ", description = "Specify X to indicate a checkbox, space otherwise")
	private String namedIsPassenger;
	@Schema(example = " ", description = "Specify X to indicate a checkbox, space otherwise")
	private String namedIsOther;
	@Schema(example = " ", description = "Named Is Other Description")
	private String namedIsOtherDescription;
	@Schema(example = "2022-02-24")
	private String violationDate;
	@Schema(example = "15:23")
	private String violationTime;
	@Schema(example = "Smallville Bypass")
	private String violationOnHighway;
	@Schema(example = "Kent Farm")
	private String violationNearPlace;
	@Schema(example = "X", description = "Specify X to indicate a checkbox, blank otherwise")
	private String offenseIsMVA;
	@Schema(example = " ", description = "Specify X to indicate a checkbox, blank otherwise")
	private String offenseIsMCA;
	@Schema(example = " ", description = "Specify X to indicate a checkbox, blank otherwise")
	private String offenseIsCTA;
	@Schema(example = " ", description = "Specify X to indicate a checkbox, blank otherwise")
	private String offenseIsWLA;
	@Schema(example = " ", description = "Specify X to indicate a checkbox, blank otherwise")
	private String offenseIsFAA;
	@Schema(example = " ", description = "Specify X to indicate a checkbox, blank otherwise")
	private String offenseIsLCA;
	@Schema(example = " ", description = "Specify X to indicate a checkbox, blank otherwise")
	private String offenseIsTCR;
	@Schema(example = " ", description = "Specify X to indicate a checkbox, blank otherwise")
	private String offenseIsOther;
	@Schema(example = " ", description = "Offense Is Other Description")
	private String offenseIsOtherDescription;
	@Schema(example = "Excessive speeding")
	private String count1Description;
	@Schema(example = "MVA")
	private String count1ActReg;
	@Schema(example = "X", description = "Specify X to indicate a checkbox, blank otherwise")
	private String count1IsACT;
	@Schema(example = " ", description = "Specify X to indicate a checkbox, blank otherwise")
	private String count1IsREGS;
	@Schema(example = "67(b)")
	private String count1Section;
	@Schema(example = "350")
	private String count1TicketAmount;
	@Schema(example = "Driving without licence")
	private String count2Description;
	@Schema(example = "MVA")
	private String count2ActReg;
	@Schema(example = "X", description = "Specify X to indicate a checkbox, blank otherwise")
	private String count2IsACT;
	@Schema(example = " ", description = "Specify X to indicate a checkbox, blank otherwise")
	private String count2IsREGS;
	@Schema(example = "45(a)")
	private String count2Section;
	@Schema(example = "145")
	private String count2TicketAmount;
	@Schema(example = "Driving with burned out break lights")
	private String count3Description;
	@Schema(example = "MVA")
	private String count3ActReg;
	@Schema(example = "X", description = "Specify X to indicate a checkbox, blank otherwise")
	private String count3IsACT;
	@Schema(example = " ", description = "Specify X to indicate a checkbox, blank otherwise")
	private String count3IsREGS;
	@Schema(example = "124(c)(i)")
	private String count3Section;
	@Schema(example = "75")
	private String count3TicketAmount;
	@Schema(example = "BC")
	private String vehicleLicensePlateProvince;
	@Schema(example = "123ABC")
	private String vehicleLicensePlateNumber;
	@Schema(example = "AABB")
	private String vehicleNscPuj;
	@Schema(example = "12345")
	private String vehicleNscNumber;
	@Schema(example = "Jonathan Kent")
	private String vehicleRegisteredOwnerName;
	@Schema(example = "John Deer")
	private String vehicleMake;
	@Schema(example = "Tracker")
	private String vehicleType;
	@Schema(example = "Green")
	private String vehicleColour;
	@Schema(example = "2011")
	private String vehicleYear;
	@Schema(example = "123 Main St.")
	private String noticeOfDisputeAddress;
	@Schema(example = "Metropolis Court")
	private String hearingLocation;
	@Schema(example = "2022-02-24")
	private String dateOfService;
	@Schema(example = "12345")
	private String enforcementOfficerNumber;
	@Schema(example = "Metropolitan Police")
	private String detachmentLocation;

	public ViolationTicket() {
	}

	public String getViolationTicketNumber() {
		return violationTicketNumber;
	}

	public void setViolationTicketNumber(String violationTicketNumber) {
		this.violationTicketNumber = violationTicketNumber;
	}

	public String getSurname() {
		return surname;
	}

	public void setSurname(String surname) {
		this.surname = surname;
	}

	public String getGivenName() {
		return givenName;
	}

	public void setGivenName(String givenName) {
		this.givenName = givenName;
	}

	public String getIsYoungPerson() {
		return isYoungPerson;
	}

	public void setIsYoungPerson(String isYoungPerson) {
		this.isYoungPerson = isYoungPerson;
	}

	public String getDriversLicenceProvince() {
		return driversLicenceProvince;
	}

	public void setDriversLicenceProvince(String driversLicenceProvince) {
		this.driversLicenceProvince = driversLicenceProvince;
	}

	public String getDriversLicenceNumber() {
		return driversLicenceNumber;
	}

	public void setDriversLicenceNumber(String driversLicenceNumber) {
		this.driversLicenceNumber = driversLicenceNumber;
	}

	public String getDriversLicenceCreated() {
		return driversLicenceCreated;
	}

	public void setDriversLicenceCreated(String driversLicenceCreated) {
		this.driversLicenceCreated = driversLicenceCreated;
	}

	public String getDriversLicenceExpiry() {
		return driversLicenceExpiry;
	}

	public void setDriversLicenceExpiry(String driversLicenceExpiry) {
		this.driversLicenceExpiry = driversLicenceExpiry;
	}

	public String getBirthdate() {
		return birthdate;
	}

	public void setBirthdate(String birthdate) {
		this.birthdate = birthdate;
	}

	@JsonIgnore
	public String getBirthdateYYYY() {
		return Objects.toString(DateUtil.getYear(DateUtil.fromDateString(birthdate)), "");
	}

	@JsonIgnore
	public String getBirthdateMM() {
		return Objects.toString(DateUtil.getMonth(DateUtil.fromDateString(birthdate)), "");
	}

	@JsonIgnore
	public String getBirthdateDD() {
		return Objects.toString(DateUtil.getDay(DateUtil.fromDateString(birthdate)), "");
	}

	public String getAddress() {
		return address;
	}

	public void setAddress(String address) {
		this.address = address;
	}

	public String getIsChangeOfAddress() {
		return isChangeOfAddress;
	}

	public void setIsChangeOfAddress(String isChangeOfAddress) {
		this.isChangeOfAddress = isChangeOfAddress;
	}

	public String getCity() {
		return city;
	}

	public void setCity(String city) {
		this.city = city;
	}

	public String getProvince() {
		return province;
	}

	public void setProvince(String province) {
		this.province = province;
	}

	public String getPostalCode() {
		return postalCode;
	}

	public void setPostalCode(String postalCode) {
		this.postalCode = postalCode;
	}

	public String getNamedIsDriver() {
		return namedIsDriver;
	}

	public void setNamedIsDriver(String namedIsDriver) {
		this.namedIsDriver = namedIsDriver;
	}

	public String getNamedIsCyclist() {
		return namedIsCyclist;
	}

	public void setNamedIsCyclist(String namedIsCyclist) {
		this.namedIsCyclist = namedIsCyclist;
	}

	public String getNamedIsOwner() {
		return namedIsOwner;
	}

	public void setNamedIsOwner(String namedIsOwner) {
		this.namedIsOwner = namedIsOwner;
	}

	public String getNamedIsPedestrain() {
		return namedIsPedestrain;
	}

	public void setNamedIsPedestrain(String namedIsPedestrain) {
		this.namedIsPedestrain = namedIsPedestrain;
	}

	public String getNamedIsPassenger() {
		return namedIsPassenger;
	}

	public void setNamedIsPassenger(String namedIsPassenger) {
		this.namedIsPassenger = namedIsPassenger;
	}

	public String getNamedIsOther() {
		return namedIsOther;
	}

	public void setNamedIsOther(String namedIsOther) {
		this.namedIsOther = namedIsOther;
	}

	public String getNamedIsOtherDescription() {
		return namedIsOtherDescription;
	}

	public void setNamedIsOtherDescription(String namedIsOtherDescription) {
		this.namedIsOtherDescription = namedIsOtherDescription;
	}

	public String getViolationDate() {
		return violationDate;
	}

	public void setViolationDate(String violationDate) {
		this.violationDate = violationDate;
	}

	@JsonIgnore
	public String getViolationDateYYYY() {
		return Objects.toString(DateUtil.getYear(DateUtil.fromDateString(violationDate)), "");
	}

	@JsonIgnore
	public String getViolationDateMM() {
		return Objects.toString(DateUtil.getMonth(DateUtil.fromDateString(violationDate)), "");
	}

	@JsonIgnore
	public String getViolationDateDD() {
		return Objects.toString(DateUtil.getDay(DateUtil.fromDateString(violationDate)), "");
	}

	public String getViolationTime() {
		return violationTime;
	}

	public void setViolationTime(String violationTime) {
		this.violationTime = violationTime;
	}

	@JsonIgnore
	public String getViolationTimeHH() {
		if (StringUtils.isBlank(violationTime)) {
			return "";
		}
		return Objects.toString(DateUtil.getHour(DateUtil.fromTimeString(violationTime)), "");
	}

	@JsonIgnore
	public String getViolationTimeMM() {
		if (StringUtils.isBlank(violationTime)) {
			return "";
		}
		return Objects.toString(DateUtil.getMinute(DateUtil.fromTimeString(violationTime)), "");
	}

	public String getViolationOnHighway() {
		return violationOnHighway;
	}

	public void setViolationOnHighway(String violationOnHighway) {
		this.violationOnHighway = violationOnHighway;
	}

	public String getViolationNearPlace() {
		return violationNearPlace;
	}

	public void setViolationNearPlace(String violationNearPlace) {
		this.violationNearPlace = violationNearPlace;
	}

	public String getOffenseIsMVA() {
		return offenseIsMVA;
	}

	public void setOffenseIsMVA(String offenseIsMVA) {
		this.offenseIsMVA = offenseIsMVA;
	}

	public String getOffenseIsMCA() {
		return offenseIsMCA;
	}

	public void setOffenseIsMCA(String offenseIsMCA) {
		this.offenseIsMCA = offenseIsMCA;
	}

	public String getOffenseIsCTA() {
		return offenseIsCTA;
	}

	public void setOffenseIsCTA(String offenseIsCTA) {
		this.offenseIsCTA = offenseIsCTA;
	}

	public String getOffenseIsWLA() {
		return offenseIsWLA;
	}

	public void setOffenseIsWLA(String offenseIsWLA) {
		this.offenseIsWLA = offenseIsWLA;
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

	public String getOffenseIsOther() {
		return offenseIsOther;
	}

	public void setOffenseIsOther(String offenseIsOther) {
		this.offenseIsOther = offenseIsOther;
	}

	public String getOffenseIsOtherDescription() {
		return offenseIsOtherDescription;
	}

	public void setOffenseIsOtherDescription(String offenseIsOtherDescription) {
		this.offenseIsOtherDescription = offenseIsOtherDescription;
	}

	public String getCount1Description() {
		return count1Description;
	}

	public void setCount1Description(String count1Description) {
		this.count1Description = count1Description;
	}

	public String getCount1ActReg() {
		return count1ActReg;
	}

	public void setCount1ActReg(String count1ActReg) {
		this.count1ActReg = count1ActReg;
	}

	public String getCount1IsACT() {
		return count1IsACT;
	}

	public void setCount1IsACT(String count1IsACT) {
		this.count1IsACT = count1IsACT;
	}

	public String getCount1IsREGS() {
		return count1IsREGS;
	}

	public void setCount1IsREGS(String count1IsREGS) {
		this.count1IsREGS = count1IsREGS;
	}

	public String getCount1Section() {
		return count1Section;
	}

	public void setCount1Section(String count1Section) {
		this.count1Section = count1Section;
	}

	public String getCount1TicketAmount() {
		return count1TicketAmount;
	}

	public void setCount1TicketAmount(String count1TicketAmount) {
		this.count1TicketAmount = count1TicketAmount;
	}

	public String getCount2Description() {
		return count2Description;
	}

	public void setCount2Description(String count2Description) {
		this.count2Description = count2Description;
	}

	public String getCount2ActReg() {
		return count2ActReg;
	}

	public void setCount2ActReg(String count2ActReg) {
		this.count2ActReg = count2ActReg;
	}

	public String getCount2IsACT() {
		return count2IsACT;
	}

	public void setCount2IsACT(String count2IsACT) {
		this.count2IsACT = count2IsACT;
	}

	public String getCount2IsREGS() {
		return count2IsREGS;
	}

	public void setCount2IsREGS(String count2IsREGS) {
		this.count2IsREGS = count2IsREGS;
	}

	public String getCount2Section() {
		return count2Section;
	}

	public void setCount2Section(String count2Section) {
		this.count2Section = count2Section;
	}

	public String getCount2TicketAmount() {
		return count2TicketAmount;
	}

	public void setCount2TicketAmount(String count2TicketAmount) {
		this.count2TicketAmount = count2TicketAmount;
	}

	public String getCount3Description() {
		return count3Description;
	}

	public void setCount3Description(String count3Description) {
		this.count3Description = count3Description;
	}

	public String getCount3ActReg() {
		return count3ActReg;
	}

	public void setCount3ActReg(String count3ActReg) {
		this.count3ActReg = count3ActReg;
	}

	public String getCount3IsACT() {
		return count3IsACT;
	}

	public void setCount3IsACT(String count3IsACT) {
		this.count3IsACT = count3IsACT;
	}

	public String getCount3IsREGS() {
		return count3IsREGS;
	}

	public void setCount3IsREGS(String count3IsREGS) {
		this.count3IsREGS = count3IsREGS;
	}

	public String getCount3Section() {
		return count3Section;
	}

	public void setCount3Section(String count3Section) {
		this.count3Section = count3Section;
	}

	public String getCount3TicketAmount() {
		return count3TicketAmount;
	}

	public void setCount3TicketAmount(String count3TicketAmount) {
		this.count3TicketAmount = count3TicketAmount;
	}

	public String getVehicleLicensePlateProvince() {
		return vehicleLicensePlateProvince;
	}

	public void setVehicleLicensePlateProvince(String vehicleLicensePlateProvince) {
		this.vehicleLicensePlateProvince = vehicleLicensePlateProvince;
	}

	public String getVehicleLicensePlateNumber() {
		return vehicleLicensePlateNumber;
	}

	public void setVehicleLicensePlateNumber(String vehicleLicensePlateNumber) {
		this.vehicleLicensePlateNumber = vehicleLicensePlateNumber;
	}

	public String getVehicleNscPuj() {
		return vehicleNscPuj;
	}

	public void setVehicleNscPuj(String vehicleNscPuj) {
		this.vehicleNscPuj = vehicleNscPuj;
	}

	public String getVehicleNscNumber() {
		return vehicleNscNumber;
	}

	public void setVehicleNscNumber(String vehicleNscNumber) {
		this.vehicleNscNumber = vehicleNscNumber;
	}

	public String getVehicleRegisteredOwnerName() {
		return vehicleRegisteredOwnerName;
	}

	public void setVehicleRegisteredOwnerName(String vehicleRegisteredOwnerName) {
		this.vehicleRegisteredOwnerName = vehicleRegisteredOwnerName;
	}

	public String getVehicleMake() {
		return vehicleMake;
	}

	public void setVehicleMake(String vehicleMake) {
		this.vehicleMake = vehicleMake;
	}

	public String getVehicleType() {
		return vehicleType;
	}

	public void setVehicleType(String vehicleType) {
		this.vehicleType = vehicleType;
	}

	public String getVehicleColour() {
		return vehicleColour;
	}

	public void setVehicleColour(String vehicleColour) {
		this.vehicleColour = vehicleColour;
	}

	public String getVehicleYear() {
		return vehicleYear;
	}

	public void setVehicleYear(String vehicleYear) {
		this.vehicleYear = vehicleYear;
	}

	public String getNoticeOfDisputeAddress() {
		return noticeOfDisputeAddress;
	}

	public void setNoticeOfDisputeAddress(String noticeOfDisputeAddress) {
		this.noticeOfDisputeAddress = noticeOfDisputeAddress;
	}

	public String getHearingLocation() {
		return hearingLocation;
	}

	public void setHearingLocation(String hearingLocation) {
		this.hearingLocation = hearingLocation;
	}

	public String getDateOfService() {
		return dateOfService;
	}

	public void setDateOfService(String dateOfService) {
		this.dateOfService = dateOfService;
	}

	@JsonIgnore
	public String getDateOfServiceYYYY() {
		return Objects.toString(DateUtil.getYear(DateUtil.fromDateString(dateOfService)), "");
	}

	@JsonIgnore
	public String getDateOfServiceMM() {
		return Objects.toString(DateUtil.getMonth(DateUtil.fromDateString(dateOfService)), "");
	}

	@JsonIgnore
	public String getDateOfServiceDD() {
		return Objects.toString(DateUtil.getDay(DateUtil.fromDateString(dateOfService)), "");
	}

	public String getEnforcementOfficerNumber() {
		return enforcementOfficerNumber;
	}

	public void setEnforcementOfficerNumber(String enforcementOfficerNumber) {
		this.enforcementOfficerNumber = enforcementOfficerNumber;
	}

	public String getDetachmentLocation() {
		return detachmentLocation;
	}

	public void setDetachmentLocation(String detachmentLocation) {
		this.detachmentLocation = detachmentLocation;
	}

}
