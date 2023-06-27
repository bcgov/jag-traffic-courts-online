package ca.bc.gov.open.jag.tco.ocr.metrics.controller;

import java.io.FileNotFoundException;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import ca.bc.gov.open.jag.tco.ocr.metrics.model.Source;
import ca.bc.gov.open.jag.tco.ocr.metrics.service.MetricService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;

@RestController(value = "/api")
public class ImageController {

	@Autowired
	private MetricService metricService;

	/**
	 * Triggers a processing of all *.png images in the data/seed directory of this project.
	 * @param skipValidation if true, will skip validation by the citizen-api
	 * @return
	 */
	@Operation(summary = "Triggers a processing of all *.png images in the data/seed directory of this project.")
	@GetMapping("/processImages")
	public void processImages(
			@RequestParam
			@Parameter(description = "If true, will skip validation by the citizen-api")
			boolean skipValidation,
			@RequestParam
			@Parameter(description = "Target Source to use when persisting the scan results")
			Source source,
			@RequestParam(required = false)
			@Parameter(description = "If specified, process only the given image")
			String fileName) throws FileNotFoundException {
		metricService.processImages(skipValidation, fileName, source);
	}
}
