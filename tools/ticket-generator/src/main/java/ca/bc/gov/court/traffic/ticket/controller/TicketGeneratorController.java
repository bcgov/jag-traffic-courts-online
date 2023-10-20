package ca.bc.gov.court.traffic.ticket.controller;

import java.awt.image.BufferedImage;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.List;
import java.util.zip.ZipEntry;
import java.util.zip.ZipOutputStream;

import javax.imageio.ImageIO;

import org.apache.commons.io.FileUtils;
import org.apache.commons.io.IOUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.multipart.MultipartFile;
import org.springframework.web.server.ResponseStatusException;

import ca.bc.gov.court.traffic.ticket.model.BaseViolationTicket;
import ca.bc.gov.court.traffic.ticket.model.ViolationTicketV1;
import ca.bc.gov.court.traffic.ticket.model.ViolationTicketV2;
import ca.bc.gov.court.traffic.ticket.model.ViolationTicketVersion;
import ca.bc.gov.court.traffic.ticket.service.TicketGeneratorService;
import ca.bc.gov.court.traffic.ticket.util.RandomUtil;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Schema;
import jakarta.servlet.ServletOutputStream;
import jakarta.servlet.http.HttpServletResponse;

/**
 * Controller for generating random violation tickets.
 */
@RestController
public class TicketGeneratorController {

	@Autowired
	private TicketGeneratorService ticketGenService;

	@Operation(description = "Generate a version-specific random Violation Ticket image.")
	@GetMapping(value = "/generate/image", produces = MediaType.IMAGE_PNG_VALUE)
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

			@RequestParam(required = false, defaultValue = "false")
			@Schema(description = "If true, will populate all fields", example = "false")
			Boolean allFilled) throws Exception {

		BufferedImage ticketImage = ticketGenService.createTicket(version, writingStyle, numCounts, Boolean.TRUE.equals(allFilled));

		// return ticket as png
		ByteArrayOutputStream os = new ByteArrayOutputStream();
		ImageIO.write(ticketImage, "png", os);
		InputStream inputStream = new ByteArrayInputStream(os.toByteArray());
		return IOUtils.toByteArray(inputStream);
	}

	@Operation(description = "Generate the json data behind a version-specific random Violation Ticket.")
	@GetMapping(value = "/generate/json", produces = MediaType.APPLICATION_JSON_VALUE)
	public @ResponseBody BaseViolationTicket createJson(
			@RequestParam(required = true)
			@Schema(description = "The Violation Ticket version to generate (2022-04 is the 1st iteration, 2023-09 the 2nd).", example = "VT2", implementation = ViolationTicketVersion.class)
			ViolationTicketVersion version,

			@RequestParam(required = false)
			@Schema(description = "The number of counts to populate, default: 3", example = "3")
			Integer numCounts,

			@RequestParam(required = false, defaultValue = "false")
			@Schema(description = "If true, will populate all fields", example = "false")
			Boolean allFilled) throws Exception {

		if (ViolationTicketVersion.VT1.equals(version)) {
			BaseViolationTicket randomTicket = RandomUtil.randomTicket(ViolationTicketVersion.VT1, numCounts, allFilled);
			return randomTicket;
		}
		else {
			BaseViolationTicket randomTicket = RandomUtil.randomTicket(ViolationTicketVersion.VT2, numCounts, allFilled);
			return randomTicket;
		}
	}

	@Operation(description = "Generate a 2022-04 Violation Ticket with specific values.")
	@PostMapping(value = "/generate/v1", produces = MediaType.IMAGE_PNG_VALUE)
	public @ResponseBody byte[] createTicket(
			@RequestBody(required = true)
			ViolationTicketV1 violationTicket,

			@RequestParam(required = false)
			@Parameter(description = "A specific writing style, enter a value between 1 and 8", example = "5")
			Integer writingStyle) throws Exception {

		BufferedImage ticketImage = ticketGenService.createTicketV1(violationTicket, writingStyle, null, false);

		// return ticket as png
		ByteArrayOutputStream os = new ByteArrayOutputStream();
		ImageIO.write(ticketImage, "png", os);
		InputStream inputStream = new ByteArrayInputStream(os.toByteArray());
		return IOUtils.toByteArray(inputStream);
	}

	@Operation(description = "Generate a 2023-09 Violation Ticket with specific values.")
	@PostMapping(value = "/generate/v2", produces = MediaType.IMAGE_PNG_VALUE)
	public @ResponseBody byte[] createTicket(
			@RequestBody(required = true)
			ViolationTicketV2 violationTicket,

			@RequestParam(required = false)
			@Parameter(description = "A specific writing style, enter a value between 1 and 8", example = "5")
			Integer writingStyle) throws Exception {

		BufferedImage ticketImage = ticketGenService.createTicketV2(violationTicket, writingStyle, null, false);

		// return ticket as png
		ByteArrayOutputStream os = new ByteArrayOutputStream();
		ImageIO.write(ticketImage, "png", os);
		InputStream inputStream = new ByteArrayInputStream(os.toByteArray());
		return IOUtils.toByteArray(inputStream);
	}

	@Operation(description = "Download a zip of images generated from an XLSX file.")
	@PostMapping(value = "/generate/images", consumes = MediaType.MULTIPART_FORM_DATA_VALUE, produces = MediaType.APPLICATION_OCTET_STREAM_VALUE)
	public @ResponseBody void createTickets(
			@RequestParam("file")
			@Parameter(description = "An XLSX file with 1 or more Violation Tickets. The spreadsheet must conform to the establish structure.", required = true)
			MultipartFile xlsx,

			HttpServletResponse response) throws Exception {

		if (!xlsx.getOriginalFilename().endsWith(".xlsx")) {
			throw new ResponseStatusException(HttpStatus.BAD_REQUEST, "Invalid file type - must be .xlsx");
		}

		Path path = Files.createTempFile(null, ".zip");
		File file = path.toFile();

		try {

			// create and auto-close output streams
			try (FileOutputStream fos = new FileOutputStream(file);
					ZipOutputStream zos = new ZipOutputStream(fos)) {

				List<BaseViolationTicket> violationTickets = ticketGenService.extractTickets(xlsx);
				for (BaseViolationTicket violationTicket : violationTickets) {

					ZipEntry ze = new ZipEntry(violationTicket.getViolationTicketNumber() + ".png");
					zos.putNextEntry(ze);

					BufferedImage ticketImage = ticketGenService.createTicketImage(violationTicket);

					// return ticket as png
					ByteArrayOutputStream os = new ByteArrayOutputStream();
					ImageIO.write(ticketImage, "png", os);
					InputStream inputStream = new ByteArrayInputStream(os.toByteArray());
					IOUtils.copy(inputStream, zos);

				}
			}

			// Set the content type and content disposition headers
			response.setContentType("application/octet-stream");
			response.setHeader("Content-Disposition", "attachment; filename=" + file.getName());

			try (InputStream inputStream = new FileInputStream(file); ServletOutputStream outputStream = response.getOutputStream();) {
				IOUtils.copy(inputStream, outputStream);
			}

		} finally {
			// clean up
			FileUtils.deleteQuietly(file);
		}
	}

}
