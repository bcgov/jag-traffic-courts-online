package ca.bc.gov.open.jag.tco.ocr.metrics.service;

import java.io.IOException;
import java.nio.file.DirectoryStream;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.Arrays;
import java.util.List;
import java.util.Map.Entry;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import __occam_package_.api.TicketsApi;
import __occam_package_.api.model.Field;
import __occam_package_.api.model.OcrViolationTicket;
import ca.bc.gov.open.jag.tco.ocr.metrics.model.Document;
import ca.bc.gov.open.jag.tco.ocr.metrics.model.FieldComparison;
import ca.bc.gov.open.jag.tco.ocr.metrics.model.GlobalValidationError;
import ca.bc.gov.open.jag.tco.ocr.metrics.model.Source;
import ca.bc.gov.open.jag.tco.ocr.metrics.repository.DocumentRepository;
import ca.bc.gov.open.jag.tco.ocr.metrics.repository.QueryRepository;

@Service
public class MetricService {

	private Logger logger = LoggerFactory.getLogger(MetricService.class);
	private static final String[] extensions = new String[] { "png", "jpg", "jpeg" };

	@Autowired
	private TicketsApi ticketsApi;

	@Autowired
	private DocumentRepository documentRepository;

	@Autowired
	private QueryRepository queryRepository;


	public void processImages(boolean skipValidation, String fileName, Source source) {
		if (fileName != null) {
			Path path = Paths.get("./data/images/" + fileName);
			processImage(skipValidation, path, source);
		}
		else {
			// Get a Path object that refers to a directory
			Path dir = Paths.get("./data/images");

			iterateImages(dir, skipValidation, source);
		}

		logger.info("Done processing.");
	}

	private void iterateImages(Path dir, boolean skipValidation, Source source) {
		// Create a DirectoryStream object with a filter for .png files only
		try (DirectoryStream<Path> stream = Files.newDirectoryStream(dir)) {
			// Loop through each path in the stream
			for (Path path : stream) {
				if (Files.isDirectory(path)) {
					// Recursively call this method on subdirectories
					iterateImages(path, skipValidation, source);
				}

				// process the file if it has a valid extension (.png, .jpg, or .jpeg)
				if (Arrays.stream(extensions).anyMatch(path.toString().toLowerCase()::endsWith)) {
					processImage(skipValidation, path, source);
				}
			}
		} catch (IOException e) {
			e.printStackTrace();
		}

	}

	private void processImage(boolean skipValidation, Path path, Source source) {
		logger.info("processing {}", path);
		String filePath = path.toString();

		try {

			OcrViolationTicket violationTicket = ticketsApi.apiTicketsAnalysePost(path.toFile(), Boolean.valueOf(!skipValidation));

			Document document = new Document(filePath, violationTicket.getGlobalConfidence(), source);
			for (Entry<String, Field> entry : violationTicket.getFields().entrySet()) {
				String fieldName = entry.getKey();
				Field field = entry.getValue();
				document.getFields().add(new ca.bc.gov.open.jag.tco.ocr.metrics.model.Field(fieldName, field));
			}

			documentRepository.deleteByFileNameAndSource(filePath, source);
			documentRepository.save(document);
		} catch (Exception e) {
			logger.error("Could not process " + filePath, e.getLocalizedMessage());
			Document document = new Document(filePath, Float.valueOf("0"), source);
			document.getGlobalValidationErrors().add(new GlobalValidationError(e.getLocalizedMessage()));
			documentRepository.deleteByFileNameAndSource(filePath, source);
			documentRepository.save(document);
		}

	}

	public List<FieldComparison> getComparisonReport(Source source1, Source source2) {
		return queryRepository.customQuery(source1, source2);
	}

	public Long getTotalFields() {
		return queryRepository.getTotalFields();
	}

}
