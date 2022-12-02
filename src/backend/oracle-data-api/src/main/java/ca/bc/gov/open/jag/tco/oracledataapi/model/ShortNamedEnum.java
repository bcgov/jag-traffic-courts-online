package ca.bc.gov.open.jag.tco.oracledataapi.model;

/**
 * This class is a simple interface that is registered with Hibernate specifically for Enums. When serialized and deserialized from the database, the
 * short name is used instead of the full name (as required by ORDS).
 */
public interface ShortNamedEnum {

	public String getShortName();

}
