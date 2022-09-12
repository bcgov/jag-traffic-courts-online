package ca.bc.gov.open.jag.tco.oracledataapi.controller.v1_0;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

import java.security.Principal;
import java.util.Arrays;
import java.util.Date;
import java.util.List;

import javax.transaction.Transactional;

import org.apache.commons.collections4.IterableUtils;
import org.apache.commons.lang3.time.DateUtils;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.http.ResponseEntity;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;

class JJDisputeControllerTest extends BaseTestSuite {

	@Autowired
	@Qualifier("JJDisputeControllerV1_0")
	private JJDisputeController jjDisputeController;

	@Autowired
	private JJDisputeRepository jjDisputeRepository;

	@Test
	public void testGetJJDisputeById() {
		// Assert db is empty and clean
		List<JJDispute> allDisputes = IterableUtils.toList(jjDisputeController.getAllJJDisputes(null));
		assertEquals(0, allDisputes.size());

		// Create a couple of JJDisputes
		Date now = new Date();
		Principal principal = getPrincipal("testUser");
		JJDispute dispute1 = jjDisputeRepository.save(RandomUtil.createJJDispute());
		dispute1.setModifiedTs(DateUtils.addDays(now, -1));
		dispute1.setVtcAssignedTo(principal.getName());
		jjDisputeController.updateJJDispute(dispute1.getTicketNumber(), principal, dispute1);
			
		JJDispute dispute2 = jjDisputeRepository.save(RandomUtil.createJJDispute());
		dispute2.setModifiedTs(DateUtils.addDays(now, -1));
		dispute2.setVtcAssignedTo(principal.getName());
		jjDisputeController.updateJJDispute(dispute2.getTicketNumber(), principal, dispute2);
		 
		// Assert request returns one record
		JJDispute jjDispute = jjDisputeController.getJJDispute(dispute2.getTicketNumber(), principal).getBody();
		assertNotNull(jjDispute);
		assertNotEquals(dispute1.getTicketNumber(), jjDispute.getTicketNumber());
		assertEquals(dispute2.getTicketNumber(), jjDispute.getTicketNumber());
	}

	@Test
	public void testGetAllJJDisputes() {
		// Assert db is empty and clean
		List<JJDispute> allDisputes = IterableUtils.toList(jjDisputeController.getAllJJDisputes(null));
		assertEquals(0, allDisputes.size());

		// Create a JJDisputes with different assignedTo
		 JJDispute dispute1 = jjDisputeRepository.save(RandomUtil.createJJDispute().toBuilder()
				 .jjAssignedTo("Steven Strange")
				 .build());
		 JJDispute dispute2 = jjDisputeRepository.save(RandomUtil.createJJDispute().toBuilder()
				 .jjAssignedTo("Tony Stark")
				 .build());
		 List<String> ticketNumbers = Arrays.asList(dispute1.getTicketNumber(), dispute2.getTicketNumber());

		// Assert request returns both records
		allDisputes = IterableUtils.toList(jjDisputeController.getAllJJDisputes(null));
		assertEquals(2, allDisputes.size());
		assertTrue(ticketNumbers.contains(allDisputes.get(0).getTicketNumber()));
		assertTrue(ticketNumbers.contains(allDisputes.get(1).getTicketNumber()));

		// Assert request returns one record
		allDisputes = IterableUtils.toList(jjDisputeController.getAllJJDisputes("Steven Strange"));
		assertEquals(1, allDisputes.size());
		assertEquals(dispute1.getTicketNumber(), allDisputes.get(0).getTicketNumber());

		// Assert request returns one record
		allDisputes = IterableUtils.toList(jjDisputeController.getAllJJDisputes("Tony Stark"));
		assertEquals(1, allDisputes.size());
		assertEquals(dispute2.getTicketNumber(), allDisputes.get(0).getTicketNumber());
	}
	
	@Test
	@Transactional
	public void testUpdateJJDispute() {
		// Create a single JJ Dispute with a specified remark
		JJDispute jjDispute = RandomUtil.createJJDispute();
		jjDispute.setCourthouseLocation("Vancouver");
		Principal principal = getPrincipal("testUser");
		jjDispute.setVtcAssignedTo(principal.getName());
		jjDisputeRepository.save(jjDispute);

		// Create a new JJ Dispute with different remark value and update the existing JJ Dispute
		JJDispute updatedJJDispute = RandomUtil.createJJDispute();
		updatedJJDispute.setCourthouseLocation("Victoria");
		updatedJJDispute.setStatus(JJDisputeStatus.IN_PROGRESS);
		jjDisputeController.updateJJDispute(jjDispute.getTicketNumber(), principal, updatedJJDispute);

		// Assert db contains only the updated JJ Dispute record.
		jjDispute = jjDisputeController.getJJDispute(jjDispute.getTicketNumber(), principal).getBody();
		assertEquals("Victoria", jjDispute.getCourthouseLocation());
		assertEquals(JJDisputeStatus.IN_PROGRESS, jjDispute.getStatus());
		List<JJDispute> allJJDisputes = jjDisputeController.getAllJJDisputes(null);
		assertEquals(1, IterableUtils.size(allJJDisputes));
	}
	
	// Helper method to return an instance of Principal
	private Principal getPrincipal(String name) {
		return new Principal() {
			@Override
			public String getName() {
				return name;
			}
		};
	}

}
