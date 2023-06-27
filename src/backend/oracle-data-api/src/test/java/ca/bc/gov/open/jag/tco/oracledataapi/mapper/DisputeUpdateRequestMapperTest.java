package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import static org.junit.jupiter.api.Assertions.assertEquals;

import java.util.Date;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequest;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequestStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequestType;
import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;

public class DisputeUpdateRequestMapperTest extends BaseTestSuite {

	private DisputeUpdateRequestMapper disputeUpdateRequestMapper;

	@Autowired
	private DisputeUpdateRequestMapperImpl disputeUpdateRequestMapperImpl;

	@BeforeEach
	void setUp() {
		disputeUpdateRequestMapper = disputeUpdateRequestMapperImpl;
	}

	@Test
	public void testDisputeUpdateRequestMapperConvertOrdsToModel() throws Exception {
		String disputeUpdateRequestId = "5";
		String disputeId = "1";
		DisputeUpdateRequestStatus disputeUpdateStatTypeCd = DisputeUpdateRequestStatus.PENDING;
		DisputeUpdateRequestType disputeUpdateReqTypeCd = DisputeUpdateRequestType.DISPUTANT_ADDRESS;
		String requestJsonTxt = "Dispute Update Request JSON";
		Date entDtm =  RandomUtil.randomDate();
		String entUserId = "5";
		Date updDtm =  RandomUtil.randomDate();
		String updUserId = "6";

		ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.DisputeUpdateRequest source = new ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.DisputeUpdateRequest();

		source.setDisputeId(disputeId);
		source.setDisputeUpdateRequestId(disputeUpdateRequestId);
		source.setDisputeUpdateStatTypeCd(disputeUpdateStatTypeCd.getShortName());
		source.setDisputeUpdateReqTypeCd(disputeUpdateReqTypeCd.getShortName());
		source.setRequestJsonTxt(requestJsonTxt);
		source.setEntDtm(entDtm);
		source.setEntUserId(entUserId);
		source.setUpdDtm(updDtm);
		source.setUpdUserId(updUserId);

		DisputeUpdateRequest target = disputeUpdateRequestMapper.convert(source);

		assertEquals(Long.valueOf(disputeUpdateRequestId), target.getDisputeUpdateRequestId());
		assertEquals(Long.valueOf(disputeId), target.getDisputeId());
		assertEquals(disputeUpdateStatTypeCd, target.getStatus());
		assertEquals(disputeUpdateReqTypeCd, target.getUpdateType());
		assertEquals(requestJsonTxt, target.getUpdateJson());
		assertEquals(entDtm, target.getCreatedTs());
		assertEquals(entUserId, target.getCreatedBy());
		assertEquals(updDtm, target.getModifiedTs());
		assertEquals(updUserId, target.getModifiedBy());
	}

	@Test
	public void testDisputeUpdateRequestMapperConvertModelToOrds() throws Exception {
		String disputeUpdateRequestId = "5";
		String disputeId = "1";
		DisputeUpdateRequestStatus disputeUpdateStatTypeCd = DisputeUpdateRequestStatus.PENDING;
		DisputeUpdateRequestType disputeUpdateReqTypeCd = DisputeUpdateRequestType.DISPUTANT_ADDRESS;
		String requestJsonTxt = "Dispute Update Request JSON";
		Date entDtm =  RandomUtil.randomDate();
		String entUserId = "5";
		Date updDtm =  RandomUtil.randomDate();
		String updUserId = "6";

		DisputeUpdateRequest source = new DisputeUpdateRequest();

		source.setDisputeId(Long.valueOf(disputeId));
		source.setDisputeUpdateRequestId(Long.valueOf(disputeUpdateRequestId));
		source.setStatus(disputeUpdateStatTypeCd);
		source.setUpdateType(disputeUpdateReqTypeCd);
		source.setUpdateJson(requestJsonTxt);
		source.setCreatedTs(entDtm);
		source.setCreatedBy(entUserId);
		source.setModifiedTs(updDtm);
		source.setModifiedBy(updUserId);

		ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.DisputeUpdateRequest target = disputeUpdateRequestMapper.convert(source);

		assertEquals(disputeUpdateRequestId, target.getDisputeUpdateRequestId());
		assertEquals(disputeId, target.getDisputeId());
		assertEquals(disputeUpdateStatTypeCd.getShortName(), target.getDisputeUpdateStatTypeCd());
		assertEquals(disputeUpdateReqTypeCd.getShortName(), target.getDisputeUpdateReqTypeCd());
		assertEquals(requestJsonTxt, target.getRequestJsonTxt());
		assertEquals(entDtm, target.getEntDtm());
		assertEquals(entUserId, target.getEntUserId());
		assertEquals(updDtm, target.getUpdDtm());
		assertEquals(updUserId, target.getUpdUserId());
	}

}
