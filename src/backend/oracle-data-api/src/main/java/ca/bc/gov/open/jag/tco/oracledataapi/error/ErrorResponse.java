package ca.bc.gov.open.jag.tco.oracledataapi.error;

import java.util.List;

import javax.xml.bind.annotation.XmlRootElement;

import org.springframework.http.HttpStatus;

import lombok.Getter;
import lombok.Setter;

@XmlRootElement(name = "error")
@Getter
@Setter
public class ErrorResponse {

	private int status;

	private String message;

	private List<String> details;

	public ErrorResponse(HttpStatus status, String message, List<String> details) {
		this.status = status.value();
		this.message = message;
		this.details = details;
	}

}
