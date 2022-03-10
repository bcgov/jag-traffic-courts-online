package ca.bc.gov.trafficcourtsonline.oracledatainterface.service;

import java.util.ArrayList;
import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import ca.bc.gov.trafficcourtsonline.oracledatainterface.model.Dispute;
import ca.bc.gov.trafficcourtsonline.oracledatainterface.repository.DisputeRepository;

@Service
public class DisputeService {
	@Autowired  
	DisputeRepository disputeRepository;
	
	//getting all dispute records by using the method findAll() of CrudRepository  
	public List<Dispute> getAllDisputes()   
	{  
		List<Dispute> dispute = new ArrayList<Dispute>();  
		disputeRepository.findAll().forEach(dispute1 -> dispute.add(dispute1));  
		return dispute;  
	}  
	//getting a specific record by using the method findById() of CrudRepository  
	public Dispute getDisputeById(int id)   
	{  
		return disputeRepository.findById(id).get();  
	}  
	//saving a specific record by using the method save() of CrudRepository  
	public void saveOrUpdate(Dispute dispute)   
	{  
		disputeRepository.save(dispute);  
	}  
	//deleting a specific record by using the method deleteById() of CrudRepository  
	public void delete(int id)   
	{  
		disputeRepository.deleteById(id);  
	}  
	//updating a record  
	public void update(Dispute dispute, int disputeid)   
	{  
		disputeRepository.save(dispute);  
	}  
}
