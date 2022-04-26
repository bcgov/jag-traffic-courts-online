package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import java.util.UUID;

import org.springframework.data.repository.CrudRepository;

import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;

public interface DisputeRepository extends CrudRepository<Dispute, UUID>{

}
