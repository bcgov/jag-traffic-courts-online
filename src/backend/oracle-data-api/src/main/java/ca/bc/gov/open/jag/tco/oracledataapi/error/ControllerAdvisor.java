package ca.bc.gov.open.jag.tco.oracledataapi.error;

import java.util.ArrayList;
import java.util.List;
import java.util.NoSuchElementException;

import javax.validation.ConstraintViolationException;

import org.springframework.dao.EmptyResultDataAccessException;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.http.converter.HttpMessageNotReadableException;
import org.springframework.validation.BindException;
import org.springframework.validation.ObjectError;
import org.springframework.web.bind.MethodArgumentNotValidException;
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
		return getResponse(HttpStatus.NOT_FOUND, "Record Not Found", ex);
	}

	/**
	 * Returns an API HTTP error code of 400 if EmptyResultDataAccessException is thrown (typically when trying to DELETE a Dispute for a non-existent
	 * record).
	 */
	@ExceptionHandler(EmptyResultDataAccessException.class)
	@ResponseStatus(HttpStatus.BAD_REQUEST)
	public ResponseEntity<Object> handleEmptyResultDataAccessException(EmptyResultDataAccessException ex) {
		return getResponse(HttpStatus.BAD_REQUEST, "Record Not Found", ex);
	}

	/**
	 * Returns an API HTTP error code of 405 if the endpoint operation is not permitted (due to business rules).
	 */
	@ExceptionHandler(NotAllowedException.class)
	@ResponseStatus(HttpStatus.METHOD_NOT_ALLOWED)
	public ResponseEntity<Object> handleNotAllowedException(NotAllowedException ex) {
		return getResponse(HttpStatus.METHOD_NOT_ALLOWED, "Operation Not Permitted", ex);
	}

	/**
	 * Returns an API HTTP error code of 400 if there are validation errors.
	 */
	@ExceptionHandler(MethodArgumentNotValidException.class)
	@ResponseStatus(HttpStatus.METHOD_NOT_ALLOWED)
	public ResponseEntity<Object> handleMethodArgumentNotValidException(MethodArgumentNotValidException ex) {
		return getResponse(HttpStatus.METHOD_NOT_ALLOWED, "Validation Failed", ex);
	}

	/**
	 * Returns an API HTTP error code of 400 if there are validation errors.
	 */
	@ExceptionHandler(ConstraintViolationException.class)
	@ResponseStatus(HttpStatus.METHOD_NOT_ALLOWED)
	public ResponseEntity<Object> handleConstraintViolationException(ConstraintViolationException ex) {
		return getResponse(HttpStatus.METHOD_NOT_ALLOWED, "Validation Failed", ex);
	}

	/**
	 * Returns an API HTTP error code of 400 if there are validation errors.
	 */
	@ExceptionHandler(HttpMessageNotReadableException.class)
	@ResponseStatus(HttpStatus.METHOD_NOT_ALLOWED)
	public ResponseEntity<Object> handleHttpMessageNotReadableException(HttpMessageNotReadableException ex) {
		return getResponse(HttpStatus.METHOD_NOT_ALLOWED, "Validation Failed", ex);
	}

	/**
	 * Returns a ResponseEntity, populated with a status, message, and details properties.
	 */
	private ResponseEntity<Object> getResponse(HttpStatus httpStatus, String message, Exception ex) {
		List<String> details = new ArrayList<>();
		if (ex instanceof BindException) {
			for (ObjectError error : ((BindException)ex).getBindingResult().getAllErrors()) {
				details.add(error.getDefaultMessage());
			}
		}
		else {
			details.add(ex.getLocalizedMessage());
		}
		return new ResponseEntity<>(new ErrorResponse(httpStatus, message, details), httpStatus);
	}
}
