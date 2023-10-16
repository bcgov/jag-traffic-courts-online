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

import ca.bc.gov.court.traffic.ticket.model.ViolationTicketV1;
import ca.bc.gov.court.traffic.ticket.model.ViolationTicketV2;
import ca.bc.gov.court.traffic.ticket.model.ViolationTicketVersion;
import ca.bc.gov.court.traffic.ticket.service.TicketGeneratorService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Schema;

/**
 * Controller for generating random violation tickets.
 */
@RestController
public class TicketGeneratorController {

	@Autowired
	private TicketGeneratorService ticketGenService;

	@Operation(description = "Generate a version-specific random Violation Ticket.")
	@GetMapping(value = "/generate", produces = MediaType.IMAGE_PNG_VALUE)
	public @ResponseBody byte[] createTicket(
			@RequestParam(required = true)
			@Schema(description = "The Violation Ticket version to generate (2022-04 is the 1st iteration, 2023-09 the 2nd).", example = "VT2", implementation = ViolationTicketVersion.class)
			ViolationTicketVersion version,

			@RequestParam(required = false)
			@Schema(description = "A specific writing style, enter a value between 1 and 8", example = "5")
			Integer writingStyle,

			@RequestParam(required = false)
			@Schema(description = "The number of counts to populate, default: 3", example = "3")
			Integer numCounts,

			@RequestParam(required = false)
			@Schema(description = "If true, will populate all fields", example = "true")
			Boolean allFilled) throws Exception {

		BufferedImage ticketImage = ticketGenService.createTicket(version, writingStyle, numCounts, Boolean.TRUE.equals(allFilled));

		// return ticket as png
		ByteArrayOutputStream os = new ByteArrayOutputStream();
		ImageIO.write(ticketImage, "png", os);
		InputStream inputStream = new ByteArrayInputStream(os.toByteArray());
		return IOUtils.toByteArray(inputStream);
	}

	@Operation(description = "Generate a 2022-04 Violation Ticket with specific values.")
	@PostMapping(value = "/generate/v1", produces = MediaType.IMAGE_PNG_VALUE)
	public @ResponseBody byte[] createTicket(
			@RequestBody (required = true)
			ViolationTicketV1 violationTicket,

			@RequestParam(required = false)
			@Parameter(description = "A specific writing style, enter a value between 1 and 8", example = "5")
			Integer writingStyle,

			@RequestParam(required = false)
			@Parameter(description = "The number of counts to populate, default: 3", example = "3")
			Integer numCounts) throws Exception {

		BufferedImage ticketImage = ticketGenService.createTicketV1(violationTicket, writingStyle, numCounts, false);

		// return ticket as png
		ByteArrayOutputStream os = new ByteArrayOutputStream();
		ImageIO.write(ticketImage, "png", os);
		InputStream inputStream = new ByteArrayInputStream(os.toByteArray());
		return IOUtils.toByteArray(inputStream);
	}

	@Operation(description = "Generate a 2023-09 Violation Ticket with specific values.")
	@PostMapping(value = "/generate/v2", produces = MediaType.IMAGE_PNG_VALUE)
	public @ResponseBody byte[] createTicket(
			@RequestBody (required = true)
			ViolationTicketV2 violationTicket,

			@RequestParam(required = false)
			@Parameter(description = "A specific writing style, enter a value between 1 and 8", example = "5")
			Integer writingStyle,

			@RequestParam(required = false)
			@Parameter(description = "The number of counts to populate, default: 3", example = "3")
			Integer numCounts) throws Exception {

		BufferedImage ticketImage = ticketGenService.createTicketV2(violationTicket, writingStyle, numCounts, false);

		// return ticket as png
		ByteArrayOutputStream os = new ByteArrayOutputStream();
		ImageIO.write(ticketImage, "png", os);
		InputStream inputStream = new ByteArrayInputStream(os.toByteArray());
		return IOUtils.toByteArray(inputStream);
	}

}
