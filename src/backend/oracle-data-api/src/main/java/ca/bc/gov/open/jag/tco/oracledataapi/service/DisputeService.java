package ca.bc.gov.open.jag.tco.oracledataapi.service;

import java.util.ArrayList;
import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputeRepository;

@Service
public class DisputeService {
	@Autowired  
	DisputeRepository disputeRepository;

	/**
	 * Retrieves all {@link Dispute} records, delegating to CrudRepository  
	 * @return
	 */
	public List<Dispute> getAllDisputes()   
	{  
		List<Dispute> dispute = new ArrayList<Dispute>();  
		disputeRepository.findAll().forEach(dispute1 -> dispute.add(dispute1));  
		return dispute;  
	}  

	/**
	 * Retrieves a specific {@link Dispute} by using the method findById() of CrudRepository
	 * @param id of the Dispute to be returned
	 * @return
	 */
	public Dispute getDisputeById(int id)   
	{  
		return disputeRepository.findById(id).get();  
	}  
 
	/**
	 * Saves a specific {@link Dispute} by using the method save() of CrudRepository 
	 * @param dispute to be saved
	 */
	public void saveOrUpdate(Dispute dispute)   
	{  
		disputeRepository.save(dispute);  
	}  

	/**
	 * Deletes a specific {@link Dispute} by using the method deleteById() of CrudRepository  
	 * @param id of the dispute to be deleted
	 */
	public void delete(int id)   
	{  
		disputeRepository.deleteById(id);  
	}  

	/**
	 * Updates a specific {@link Dispute}, delegating to CrudRepository 
	 * @param dispute
	 * @param disputeid
	 */
	public void update(Dispute dispute, int disputeid)   
	{  
		disputeRepository.save(dispute);  
	}  
}
