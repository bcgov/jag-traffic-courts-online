package ca.bc.gov.court.traffic.ticket.service;

import static org.junit.jupiter.api.Assertions.assertEquals;

import org.junit.jupiter.api.Test;


class TicketGeneratorServiceTest {

	@Test
	void splitPhrase1() {
		String phrase = "Role of the superintendent";

		String[] splitPhrase = TicketGeneratorService.splitPhrase(phrase);
		assertEquals(2, splitPhrase.length);
		assertEquals("Role of the", splitPhrase[0]);
		assertEquals("superintendent", splitPhrase[1]);
	}

	@Test
	void splitPhrase2() {
		String phrase = "No proper signalling equipment for right hand drive";

		String[] splitPhrase = TicketGeneratorService.splitPhrase(phrase);
		assertEquals(2, splitPhrase.length);
		assertEquals("No proper signalling equipment", splitPhrase[0]);
		assertEquals("for right hand drive", splitPhrase[1]);
	}
}
