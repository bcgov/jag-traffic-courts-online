package ca.bc.gov.open.jag.tco.ocr.metrics.controller;

import java.io.FileNotFoundException;
import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.ModelMap;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.servlet.ModelAndView;

import ca.bc.gov.open.jag.tco.ocr.metrics.model.FieldComparison;
import ca.bc.gov.open.jag.tco.ocr.metrics.service.MetricService;

@Controller
public class ReportController {

	@Autowired
	private MetricService metricService;

	@RequestMapping("/report")
	public ModelAndView fieldReport(@ModelAttribute("cmd") ReportForm cmd, ModelMap model) throws FileNotFoundException {
		List<FieldComparison> fieldComparisons = metricService.getComparisonReport();
		Long totalFields = metricService.getTotalFields();

		model.addAttribute("fieldComparisons", fieldComparisons);
		model.addAttribute("mismatchCount", Integer.valueOf(fieldComparisons.size()));
		model.addAttribute("totalCount", Integer.valueOf(totalFields.intValue()));
		model.addAttribute("successRate", Double.valueOf(1.0 - (((double)fieldComparisons.size()) / ((double)metricService.getTotalFields()))));
		return new ModelAndView("report", model);
	}

}
