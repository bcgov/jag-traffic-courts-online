package ca.bc.gov.open.jag.tco.oracledataapi.controller.v1_0;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

import java.security.Principal;
import java.text.ParseException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import javax.transaction.Transactional;

import org.apache.commons.collections4.IterableUtils;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.core.authority.SimpleGrantedAuthority;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.model.CustomUserDetails;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.security.PreAuthenticatedToken;
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
		List<JJDispute> allDisputes = IterableUtils.toList(jjDisputeController.getJJDisputes(null, null));
		assertEquals(0, allDisputes.size());

		// Create a couple of JJDisputes
		JJDispute dispute1 = jjDisputeRepository.save(RandomUtil.createJJDispute());
		JJDispute dispute2 = jjDisputeRepository.save(RandomUtil.createJJDispute());

		// Assert request returns one record
		JJDispute jjDispute = jjDisputeController.getJJDispute(dispute2.getTicketNumber(), false, null).getBody();
		assertNotNull(jjDispute);
		assertNotEquals(dispute1.getTicketNumber(), jjDispute.getTicketNumber());
		assertEquals(dispute2.getTicketNumber(), jjDispute.getTicketNumber());
	}

	@Test
	public void testGetJJDisputes_Name() {
		// Assert db is empty and clean
		List<JJDispute> allDisputes = IterableUtils.toList(jjDisputeController.getJJDisputes(null, null));
		assertEquals(0, allDisputes.size());

		JJDispute jjDispute1 = RandomUtil.createJJDispute();
		jjDispute1.setJjAssignedTo("Steven Strange");

		JJDispute jjDispute2 = RandomUtil.createJJDispute();
		jjDispute2.setJjAssignedTo("Tony Stark");

		// Create a JJDisputes with different assignedTo
		JJDispute dispute1 = jjDisputeRepository.save(jjDispute1);
		JJDispute dispute2 = jjDisputeRepository.save(jjDispute2);

		List<String> ticketNumbers = Arrays.asList(dispute1.getTicketNumber(), dispute2.getTicketNumber());

		// Assert request returns both records
		allDisputes = IterableUtils.toList(jjDisputeController.getJJDisputes(null, null));
		assertEquals(2, allDisputes.size());
		assertTrue(ticketNumbers.contains(allDisputes.get(0).getTicketNumber()));
		assertTrue(ticketNumbers.contains(allDisputes.get(1).getTicketNumber()));

		// Assert request returns one record
		allDisputes = IterableUtils.toList(jjDisputeController.getJJDisputes("Steven Strange", null));
		assertEquals(1, allDisputes.size());
		assertEquals(dispute1.getTicketNumber(), allDisputes.get(0).getTicketNumber());

		// Assert request returns one record
		allDisputes = IterableUtils.toList(jjDisputeController.getJJDisputes("Tony Stark", null));
		assertEquals(1, allDisputes.size());
		assertEquals(dispute2.getTicketNumber(), allDisputes.get(0).getTicketNumber());
	}

	@Test
	public void testGetJJDisputes_TicketNumber() throws ParseException {
		// Assert db is empty and clean
		List<JJDispute> allDisputes = IterableUtils.toList(jjDisputeController.getJJDisputes(null, null));
		assertEquals(0, allDisputes.size());

		JJDispute jjDispute1 = RandomUtil.createJJDispute();
		jjDispute1.setJjAssignedTo("Steven Strange");
		jjDispute1.setTicketNumber("AX12345678");

		JJDispute jjDispute2 = RandomUtil.createJJDispute();
		jjDispute2.setJjAssignedTo("Tony Stark");
		jjDispute2.setTicketNumber("AX00000000");

		// Create a JJDisputes with different assignedTo
		JJDispute dispute1 = jjDisputeRepository.save(jjDispute1);
		JJDispute dispute2 = jjDisputeRepository.save(jjDispute2);
		List<String> ticketNumbers = Arrays.asList(dispute1.getTicketNumber(), dispute2.getTicketNumber());

		// Assert request returns both records
		allDisputes = IterableUtils.toList(jjDisputeController.getJJDisputes(null, null));
		assertEquals(2, allDisputes.size());
		assertTrue(ticketNumbers.contains(allDisputes.get(0).getTicketNumber()));
		assertTrue(ticketNumbers.contains(allDisputes.get(1).getTicketNumber()));

		// Assert request returns one record
		allDisputes = IterableUtils.toList(jjDisputeController.getJJDisputes(null, "AX12345678"));
		assertEquals(1, allDisputes.size());
		assertEquals(dispute1.getTicketNumber(), allDisputes.get(0).getTicketNumber());

		// Assert request returns one record
		allDisputes = IterableUtils.toList(jjDisputeController.getJJDisputes(null, "AX00000000"));
		assertEquals(1, allDisputes.size());
		assertEquals(dispute2.getTicketNumber(), allDisputes.get(0).getTicketNumber());
	}

	@Test
	@Transactional
	public void testUpdateJJDispute() {
		// Create a single JJ Dispute with a specified remark
		JJDispute jjDispute = RandomUtil.createJJDispute();
		String ticketNumber = jjDispute.getTicketNumber();
		setPrincipal("testUser");
		jjDispute.setCourthouseLocation("Vancouver");
		jjDisputeRepository.save(jjDispute);

		// Retrieve it from the controller's endpoint to do assignment
		jjDispute = jjDisputeController.getJJDispute(ticketNumber, true, getPrincipal()).getBody();
		assertEquals(ticketNumber, jjDispute.getTicketNumber());

		// Create a new JJ Dispute with different remark value and update the existing JJ Dispute
		JJDispute updatedJJDispute = RandomUtil.createJJDispute();
		updatedJJDispute.setCourthouseLocation("Victoria");
		updatedJJDispute.setStatus(JJDisputeStatus.IN_PROGRESS);
		jjDisputeController.updateJJDispute(ticketNumber, false, this.getPrincipal(), updatedJJDispute);

		// Assert db contains only the updated JJ Dispute record.
		jjDispute = jjDisputeController.getJJDispute(ticketNumber, false, null).getBody();
		assertEquals("Victoria", jjDispute.getCourthouseLocation());
		assertEquals(JJDisputeStatus.IN_PROGRESS, jjDispute.getStatus());
		List<JJDispute> allJJDisputes = jjDisputeController.getJJDisputes(null, null);
		assertEquals(1, IterableUtils.size(allJJDisputes));
	}

	@Test
	@Transactional
	public void testAssignJJDisputesToJJ() {
		JJDispute jjDispute1 = RandomUtil.createJJDispute();
		JJDispute jjDispute2 = RandomUtil.createJJDispute();
		jjDispute2.setJjAssignedTo("Tony Stark");

		// Create a couple of JJDisputes (one unassigned and one assigned to a JJ)
		JJDispute dispute1 = jjDisputeRepository.save(jjDispute1);
		JJDispute dispute2 = jjDisputeRepository.save(jjDispute2);

		// Get the ids of the jj disputes and call the AssignJJ endpoint to assign all jj disputes to a new JJ
		List<String> ticketNumbers = new ArrayList<>();
		ticketNumbers.add(dispute1.getTicketNumber());
		ticketNumbers.add(dispute2.getTicketNumber());
		jjDisputeController.assignJJDisputesToJJ(ticketNumbers, "Steven Strange");

		// Assert JJ disputes updated with new assigned JJ
		jjDispute1 = jjDisputeController.getJJDispute(dispute1.getTicketNumber(), false, null).getBody();
		assertEquals("Steven Strange", jjDispute1.getJjAssignedTo());
		jjDispute2 = jjDisputeController.getJJDispute(dispute2.getTicketNumber(), false, null).getBody();
		assertEquals("Steven Strange", jjDispute2.getJjAssignedTo());

		// Unassign all disputes by calling the AssignJJ endpoint with null username
		jjDisputeController.assignJJDisputesToJJ(ticketNumbers, null);

		// Assert JJ disputes are unassigned
		jjDispute1 = jjDisputeController.getJJDispute(dispute1.getTicketNumber(), false, null).getBody();
		assertEquals(null, jjDispute1.getJjAssignedTo());
		jjDispute2 = jjDisputeController.getJJDispute(dispute2.getTicketNumber(), false, null).getBody();
		assertEquals(null, jjDispute2.getJjAssignedTo());
	}

	@Test
	public void testSetJJDisputeStatusToReview() {
		// Create a single JJ Dispute with status CONFIRMED
		JJDispute jjDispute = RandomUtil.createJJDispute();
		String ticketNumber = jjDispute.getTicketNumber();
		// Add the required authority role
		List<GrantedAuthority> authority = new ArrayList<>();
		authority.add(new SimpleGrantedAuthority("User"));
		CustomUserDetails user = new CustomUserDetails("testUser", "password", "testUser", "partId", authority);
		Principal principal = new PreAuthenticatedToken(user);
		// Set valid status
		jjDispute.setStatus(JJDisputeStatus.CONFIRMED);
		jjDisputeRepository.save(jjDispute);

		// Retrieve it from the controller's endpoint
		jjDispute = jjDisputeController.getJJDispute(ticketNumber, false, principal).getBody();
		assertEquals(ticketNumber, jjDispute.getTicketNumber());
		assertEquals(JJDisputeStatus.CONFIRMED, jjDispute.getStatus());

		// Set the status to REVIEW
		jjDisputeController.reviewJJDispute(ticketNumber, "Test Remark", false, principal);

		// Assert status and remark are set.
		jjDispute = jjDisputeController.getJJDispute(ticketNumber, false, principal).getBody();
		assertEquals(JJDisputeStatus.REVIEW, jjDispute.getStatus());
		assertEquals("Test Remark", jjDispute.getRemarks().get(0).getNote());
		assertEquals("testUser", jjDispute.getRemarks().get(0).getUserFullName());
	}

	@Test
	public void testSetJJDisputeStatusToAccepted() {
		// Create a single JJ Dispute with status CONFIRMED
		JJDispute jjDispute = RandomUtil.createJJDispute();
		String ticketNumber = jjDispute.getTicketNumber();
		// Add the required authority role
		List<GrantedAuthority> authority = new ArrayList<>();
		authority.add(new SimpleGrantedAuthority("User"));
		CustomUserDetails user = new CustomUserDetails("testUser", "password", "testUser", "partId", authority);
		Principal principal = new PreAuthenticatedToken(user);
		// Set valid status
		jjDispute.setStatus(JJDisputeStatus.CONFIRMED);
		jjDisputeRepository.save(jjDispute);

		// Retrieve it from the controller's endpoint
		jjDispute = jjDisputeController.getJJDispute(ticketNumber, false, principal).getBody();
		assertEquals(ticketNumber, jjDispute.getTicketNumber());
		assertEquals(JJDisputeStatus.CONFIRMED, jjDispute.getStatus());

		// Set the status to ACCEPTED
		jjDisputeController.acceptJJDispute(ticketNumber, false, principal);

		// Assert status is set correctly.
		jjDispute = jjDisputeController.getJJDispute(ticketNumber, false, principal).getBody();
		assertEquals(JJDisputeStatus.ACCEPTED, jjDispute.getStatus());
	}

	@Test
	public void testSetJJDisputeStatusToConfirmed() {
		// Create a single JJ Dispute with status NEW
		JJDispute jjDispute = RandomUtil.createJJDispute();
		String ticketNumber = jjDispute.getTicketNumber();
		// Add the required authority role
		List<GrantedAuthority> authority = new ArrayList<>();
		authority.add(new SimpleGrantedAuthority("User"));
		CustomUserDetails user = new CustomUserDetails("testUser", "password", "testUser", "partId", authority);
		Principal principal = new PreAuthenticatedToken(user);
		jjDisputeRepository.save(jjDispute);

		// Retrieve it from the controller's endpoint
		jjDispute = jjDisputeController.getJJDispute(ticketNumber, false, principal).getBody();
		assertEquals(ticketNumber, jjDispute.getTicketNumber());
		assertEquals(JJDisputeStatus.NEW, jjDispute.getStatus());

		// Set the status to ACCEPTED
		jjDisputeController.confirmJJDispute(ticketNumber, principal);

		// Assert status is set correctly.
		jjDispute = jjDisputeController.getJJDispute(ticketNumber, false, principal).getBody();
		assertEquals(JJDisputeStatus.CONFIRMED, jjDispute.getStatus());
	}

}
