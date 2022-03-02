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
import io.swagger.v3.oas.annotations.media.Schema;

@RestController
public class TicketGeneratorController {

	@Autowired
	private TicketGeneratorService ticketGenService;
		
	@GetMapping(value = "/generate", produces = MediaType.IMAGE_PNG_VALUE)
	public @ResponseBody byte[] createTicket(@RequestParam(required = false) Integer writingStyle) throws Exception {

		BufferedImage ticketImage = ticketGenService.createTicket(writingStyle);
		
		// return ticket as png
		ByteArrayOutputStream os = new ByteArrayOutputStream();
		ImageIO.write(ticketImage, "png", os);
		InputStream inputStream = new ByteArrayInputStream(os.toByteArray());
		return IOUtils.toByteArray(inputStream);
	}
	
	@PostMapping(value = "/generate", produces = MediaType.IMAGE_PNG_VALUE)
	public @ResponseBody byte[] createTicket(@RequestBody (required = true) ViolationTicket violationTicket, @RequestParam(required = false) @Schema(example = "5", description = "Enter a number 1-9 for unique writing styles.") Integer writingStyle) throws Exception {

		BufferedImage ticketImage = ticketGenService.createTicket(violationTicket, writingStyle);
		
		// return ticket as png
		ByteArrayOutputStream os = new ByteArrayOutputStream();
		ImageIO.write(ticketImage, "png", os);
		InputStream inputStream = new ByteArrayInputStream(os.toByteArray());
		return IOUtils.toByteArray(inputStream);
	}
	
}
