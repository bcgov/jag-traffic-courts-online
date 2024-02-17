package ca.bc.gov.open.jag.tco.ocr.metrics.repository;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.transaction.annotation.Transactional;

import ca.bc.gov.open.jag.tco.ocr.metrics.model.Document;
import ca.bc.gov.open.jag.tco.ocr.metrics.model.Source;

public interface DocumentRepository extends JpaRepository<Document, Long> {

	@Transactional
	public void deleteByFileNameAndSource(String fileName, Source source);

}
