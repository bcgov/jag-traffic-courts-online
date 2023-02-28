package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import org.mapstruct.Named;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import ca.bc.gov.open.jag.tco.oracledataapi.model.ContactType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeHearingType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputedCountFinding;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Plea;
import ca.bc.gov.open.jag.tco.oracledataapi.model.ShortNamedEnum;
import ca.bc.gov.open.jag.tco.oracledataapi.model.TicketImageDataDocumentType;
import ca.bc.gov.open.jag.tco.oracledataapi.service.DisputeService;

public abstract class BaseMapper {
	
	private Logger logger = LoggerFactory.getLogger(DisputeService.class);

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

	@Named("mapDisputeStatus")
	protected JJDisputeStatus mapDisputeStatus(String statusShortCd) {
		JJDisputeStatus[] values = JJDisputeStatus.values();
		for (JJDisputeStatus statusType : values) {
			if (statusType.getShortName().equals(statusShortCd)) {
				return statusType;
			}
		}
		return null;
	}
	
	@Named("mapOccamDisputeIdToLong")
	protected long mapOccamDisputeIdToLong(String occamDisputeId) {
		return Long.parseLong(occamDisputeId);
	}

	@Named("mapOccamDisputeIdToString")
	protected String mapOccamDisputeIdToString(Long occamDisputeId) {
		return occamDisputeId.toString();
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

}
