package ca.bc.gov.court.traffic.ticket.controller;

import java.awt.image.BufferedImage;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.InputStream;

import javax.imageio.ImageIO;

import org.apache.commons.io.IOUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.bind.annotation.RestController;

import ca.bc.gov.court.traffic.ticket.model.ViolationTicket;
import ca.bc.gov.court.traffic.ticket.service.TicketGeneratorService;
import io.swagger.v3.oas.annotations.Parameter;

@RestController
public class TicketGeneratorController {

	@Autowired
	private TicketGeneratorService ticketGenService;

	@GetMapping(value = "/generate", produces = MediaType.IMAGE_PNG_VALUE)
	public @ResponseBody byte[] createTicket(
			@RequestParam(required = false)
			@Parameter(description = "A specific writing style, enter a value between 1 and 8", example = "5")
			Integer writingStyle,
			@RequestParam(required = false)
			@Parameter(description = "The number of counts to populate, default: 3", example = "3")
			Integer numCounts) throws Exception {

		BufferedImage ticketImage = ticketGenService.createTicket(writingStyle, numCounts);

		// return ticket as png
		ByteArrayOutputStream os = new ByteArrayOutputStream();
		ImageIO.write(ticketImage, "png", os);
		InputStream inputStream = new ByteArrayInputStream(os.toByteArray());
		return IOUtils.toByteArray(inputStream);
	}

	@PostMapping(value = "/generate", produces = MediaType.IMAGE_PNG_VALUE)
	public @ResponseBody byte[] createTicket(
			@RequestBody (required = true)
			ViolationTicket violationTicket,
			@RequestParam(required = false)
			@Parameter(description = "A specific writing style, enter a value between 1 and 8", example = "5")
			Integer writingStyle,
			@RequestParam(required = false)
			@Parameter(description = "The number of counts to populate, default: 3", example = "3")
			Integer numCounts) throws Exception {

		BufferedImage ticketImage = ticketGenService.createTicket(violationTicket, writingStyle, numCounts);

		// return ticket as png
		ByteArrayOutputStream os = new ByteArrayOutputStream();
		ImageIO.write(ticketImage, "png", os);
		InputStream inputStream = new ByteArrayInputStream(os.toByteArray());
		return IOUtils.toByteArray(inputStream);
	}

}
