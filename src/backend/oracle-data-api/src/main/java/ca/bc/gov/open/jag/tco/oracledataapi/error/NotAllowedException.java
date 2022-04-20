package ca.bc.gov.open.jag.tco.oracledataapi.error;

public class NotAllowedException extends RuntimeException {

	private static final long serialVersionUID = 1L;

	public NotAllowedException(String reason, Object... args) {
		super(String.format(reason, args));
	}

}
