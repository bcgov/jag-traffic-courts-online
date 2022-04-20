package ca.bc.gov.open.jag.tco.oracledataapi.error;

import java.util.NoSuchElementException;

import org.springframework.dao.EmptyResultDataAccessException;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.ResponseStatus;
import org.springframework.web.bind.annotation.RestControllerAdvice;

@RestControllerAdvice
public class ControllerAdvisor {

	/**
	 * Returns an API HTTP error code of 404 if NoSuchElementException is thrown (typically when trying to GET a Dispute for a non-existent record).
	 */
	@ExceptionHandler(NoSuchElementException.class)
	@ResponseStatus(HttpStatus.NOT_FOUND)
	public ResponseEntity<Object> handleNoSuchElementException(NoSuchElementException ex) {
		return new ResponseEntity<>(HttpStatus.NOT_FOUND);
	}

	/**
	 * Returns an API HTTP error code of 400 if EmptyResultDataAccessException is thrown (typically when trying to DELETE a Dispute for a non-existent
	 * record).
	 */
	@ExceptionHandler(EmptyResultDataAccessException.class)
	@ResponseStatus(HttpStatus.BAD_REQUEST)
	public ResponseEntity<Object> handleEmptyResultDataAccessException(EmptyResultDataAccessException ex) {
		return new ResponseEntity<>(HttpStatus.BAD_REQUEST);
	}

	/**
	 * Returns an API HTTP error code of 405 if the endpoint operation is not permitted (due to business rules).
	 */
	@ExceptionHandler(NotAllowedException.class)
	@ResponseStatus(HttpStatus.BAD_REQUEST)
	public ResponseEntity<Object> NotAllowedExceptionException(NotAllowedException ex) {
		return new ResponseEntity<>(ex.getMessage(), HttpStatus.METHOD_NOT_ALLOWED);
	}

}
