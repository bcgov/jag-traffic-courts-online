package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import org.mapstruct.Named;

import ca.bc.gov.open.jag.tco.oracledataapi.model.ContactType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeHearingType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputedCountFinding;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Plea;
import ca.bc.gov.open.jag.tco.oracledataapi.model.ShortNamedEnum;
import ca.bc.gov.open.jag.tco.oracledataapi.model.YesNo;

public abstract class BaseMapper {
	
	@Named("mapContactType")
	protected ContactType mapContactType(String statusShortCd) {
		ContactType[] values = ContactType.values();
		for (ContactType contactType : values) {
			if (contactType.getShortName().equals(statusShortCd)) {
				return contactType;
			}
		}
		return null;
	}

	@Named("mapJJDisputeStatus")
	protected JJDisputeStatus mapJJDisputeStatus(String statusShortCd) {
		JJDisputeStatus[] values = JJDisputeStatus.values();
		for (JJDisputeStatus statusType : values) {
			if (statusType.getShortName().equals(statusShortCd)) {
				return statusType;
			}
		}
		return null;
	}
	
	@Named("mapFindingResult")
	protected JJDisputedCountFinding mapFindingResult(String statusShortCd) {
		JJDisputedCountFinding[] values = JJDisputedCountFinding.values();
		for (JJDisputedCountFinding findingCd : values) {
			if (findingCd.getShortName().equals(statusShortCd)) {
				return findingCd;
			}
		}
		return null;
	}

	@Named("mapHearingType")
	protected JJDisputeHearingType mapHearingType(String statusShortCd) {
		JJDisputeHearingType[] values = JJDisputeHearingType.values();
		for (JJDisputeHearingType hearingType : values) {
			if (hearingType.getShortName().equals(statusShortCd)) {
				return hearingType;
			}
		}
		return null;
	}

	@Named("mapPlea")
	protected Plea mapPlea(String statusShortCd) {
		Plea[] values = Plea.values();
		for (Plea hearingType : values) {
			if (hearingType.getShortName().equals(statusShortCd)) {
				return hearingType;
			}
		}
		return null;
	}

	@Named("mapShortNamedEnum")
	protected String mapShortNamedEnum(ShortNamedEnum code) {
		return code != null ? code.getShortName() : null;
	}
	
	/**
	 * Custom mapping for mapping YesNo fields to Boolean value
	 *
	 * @param value
	 * @return Boolean value of {@link YesNo} enum
	 */
	@Named("mapYnToBoolean")
	protected Boolean mapYnToBoolean(YesNo value) {
		return YesNo.Y.equals(value);
	}

	@Named("mapDisputeStatus")
	protected DisputeStatus mapDisputeStatus(String statusShortCd) {
		DisputeStatus[] values = DisputeStatus.values();
		for (DisputeStatus disputeStatus : values) {
			if (disputeStatus.toShortName().equals(statusShortCd)) {
				return disputeStatus;
			}
		}
		return null;
	}
	
	@Named("mapContactTypeCd")
	protected ContactType mapContactTypeCd(String statusShortCd) {
		ContactType[] values = ContactType.values();
		for (ContactType contactType : values) {
			if (contactType.getShortName().equals(statusShortCd)) {
				return contactType;
			}
		}
		return null;
	}

}
