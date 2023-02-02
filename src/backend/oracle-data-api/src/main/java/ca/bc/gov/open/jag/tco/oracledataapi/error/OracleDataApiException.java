package ca.bc.gov.open.jag.tco.oracledataapi.error;

/**
 * Generic Exception for Oracle Data API
 * 
 * @author 237563
 *
 */
public class OracleDataApiException extends RuntimeException{

	private static final long serialVersionUID = 1L;
	
	public OracleDataApiException(String message) {
		super(message);
	}

	public OracleDataApiException(String message, Throwable cause) {
		super(message, cause);
	}
}
