package ca.bc.gov.open.jag.tco.oracledataapi.controller.v1_0;

import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.service.JJDisputeService;
import io.swagger.v3.oas.annotations.Parameter;

@RestController(value = "JJDisputeControllerV1_0")
@RequestMapping("/api/v1.0/jj")
public class JJDisputeController {

	@Autowired
	private JJDisputeService jjDisputeService;

	private Logger logger = LoggerFactory.getLogger(JJDisputeController.class);
	
	/**
	 * GET endpoint that retrieves all the jj disputes optionally filtered by jjGroupAssignedTo and/or jjAssignedTo from the database
	 * @param jjGroupAssignedTo if specified, will filter the result set to those assigned to the specified jj group.
	 * @param jjAssignedTo if specified, will filter the result set to those assigned to the specified jj staff.
	 * @return list of all jj disputes
	 */
	@GetMapping("/disputes")
	public List<JJDispute> getAllJJDisputes(
			@RequestParam(required = false)
			@Parameter(description = "If specified, will retrieve the records which are assigned to the specified jj group")
			String jjGroupAssignedTo,
			@RequestParam(required = false)
			@Parameter(description = "If specified, will retrieve the records which are assigned to the specified jj staff")
			String jjAssignedTo) {
		logger.debug("getAllJJDisputes called");
		
		return jjDisputeService.getAllJJDisputes(jjGroupAssignedTo, jjAssignedTo);
	}
}
