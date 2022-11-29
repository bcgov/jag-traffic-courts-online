package ca.bc.gov.open.jag.tco.oracledataapi.error;

public class BadDataException extends RuntimeException {

	private static final long serialVersionUID = 1L;

	public BadDataException(String reason, Object... args) {
		super(String.format(reason, args));
	}

}
