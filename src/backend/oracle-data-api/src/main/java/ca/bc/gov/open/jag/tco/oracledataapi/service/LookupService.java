package ca.bc.gov.open.jag.tco.oracledataapi.service;

import java.util.List;

import ca.bc.gov.open.jag.tco.oracledataapi.api.handler.ApiException;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Language;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Statute;

public interface LookupService {

	public void refresh();

	public List<Statute> getStatutes() throws ApiException;

	public List<Language> getLanguages() throws ApiException;

}
