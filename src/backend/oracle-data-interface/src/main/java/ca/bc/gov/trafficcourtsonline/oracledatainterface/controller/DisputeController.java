package ca.bc.gov.trafficcourtsonline.oracledatainterface.controller;

import java.util.Date;
import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import ca.bc.gov.trafficcourtsonline.oracledatainterface.model.Dispute;
import ca.bc.gov.trafficcourtsonline.oracledatainterface.service.DisputeService;

@RestController  
public class DisputeController {
	@Autowired  
	DisputeService disputeService; 
	
	private Logger log = LoggerFactory.getLogger(DisputeController.class);
	
	//creating a get mapping that retrieves all the dispute detail from the database   
	@GetMapping("/disputes")  
	private List<Dispute> getAllDisputes()   
	{  
		log.info("Retrieve all disputes endpoint is called." + new Date());
		return disputeService.getAllDisputes();  
	}  
	//creating a get mapping that retrieves the detail of a specific dispute  
	@GetMapping("/dispute/{disputeId}")  
	private Dispute getDispute(@PathVariable("disputeId") int disputeId)   
	{  
		return disputeService.getDisputeById(disputeId);  
	}  
	//creating a delete mapping that deletes a specified dispute  
	@DeleteMapping("/dispute/{disputeId}")  
	private void deleteDispute(@PathVariable("disputeId") int disputeId)   
	{  
		disputeService.delete(disputeId);  
	}  
	//creating post mapping that post the dispute detail in the database  
	@PostMapping("/dispute")  
	private int saveDispute(@RequestBody Dispute dispute)   
	{  
		disputeService.saveOrUpdate(dispute);  
		return dispute.getDisputeId();  
	}  
	//creating put mapping that updates the dispute detail   
	@PutMapping("/dispute")  
	private Dispute update(@RequestBody Dispute dispute)   
	{  
		disputeService.saveOrUpdate(dispute);  
		return dispute;  
	}  
}
