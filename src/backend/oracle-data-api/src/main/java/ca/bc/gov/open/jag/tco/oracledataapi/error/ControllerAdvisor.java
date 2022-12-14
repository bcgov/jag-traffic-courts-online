package ca.bc.gov.open.jag.tco.oracledataapi.error;

import java.util.ArrayList;
import java.util.List;
import java.util.NoSuchElementException;

import javax.validation.ConstraintViolationException;

import org.hibernate.cfg.NotYetImplementedException;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
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

	private Logger logger = LoggerFactory.getLogger(ControllerAdvisor.class);

	/**
	 * Returns an API HTTP error code of 404 if NoSuchElementException is thrown (typically when trying to GET a Dispute for a non-existent record).
	 */
	@ExceptionHandler(NoSuchElementException.class)
	@ResponseStatus(HttpStatus.NOT_FOUND)
	public ResponseEntity<Object> handleNoSuchElementException(NoSuchElementException ex) {
		logger.debug("handleNoSuchElementException", ex);
		return getResponse(HttpStatus.NOT_FOUND, "Record Not Found", ex);
	}

	/**
	 * Returns an API HTTP error code of 400 if EmptyResultDataAccessException is thrown (typically when trying to DELETE a Dispute for a non-existent
	 * record).
	 */
	@ExceptionHandler(EmptyResultDataAccessException.class)
	@ResponseStatus(HttpStatus.BAD_REQUEST)
	public ResponseEntity<Object> handleEmptyResultDataAccessException(EmptyResultDataAccessException ex) {
		logger.debug("handleEmptyResultDataAccessException", ex);
		return getResponse(HttpStatus.BAD_REQUEST, "Record Not Found", ex);
	}

	/**
	 * Returns an API HTTP error code of 405 if the endpoint operation is not permitted (due to business rules).
	 */
	@ExceptionHandler(NotAllowedException.class)
	@ResponseStatus(HttpStatus.METHOD_NOT_ALLOWED)
	public ResponseEntity<Object> handleNotAllowedException(NotAllowedException ex) {
		logger.debug("handleNotAllowedException", ex);
		return getResponse(HttpStatus.METHOD_NOT_ALLOWED, "Operation Not Permitted", ex);
	}

	/**
	 * Returns an API HTTP error code of 400 if there are validation errors.
	 */
	@ExceptionHandler(MethodArgumentNotValidException.class)
	@ResponseStatus(HttpStatus.BAD_REQUEST)
	public ResponseEntity<Object> handleMethodArgumentNotValidException(MethodArgumentNotValidException ex) {
		logger.debug("handleMethodArgumentNotValidException", ex);
		return getResponse(HttpStatus.BAD_REQUEST, "Validation Failed", ex);
	}

	/**
	 * Returns an API HTTP error code of 400 if there are validation errors.
	 */
	@ExceptionHandler(ConstraintViolationException.class)
	@ResponseStatus(HttpStatus.BAD_REQUEST)
	public ResponseEntity<Object> handleConstraintViolationException(ConstraintViolationException ex) {
		logger.debug("handleConstraintViolationException", ex);
		return getResponse(HttpStatus.BAD_REQUEST, "Validation Failed", ex);
	}

	/**
	 * Returns an API HTTP error code of 400 if there are bad parameters.
	 */
	@ExceptionHandler(IllegalArgumentException.class)
	@ResponseStatus(HttpStatus.BAD_REQUEST)
	public ResponseEntity<Object> handleIllegalArgumentException(IllegalArgumentException ex) {
		logger.debug("handleIllegalArgumentException", ex);
		return getResponse(HttpStatus.BAD_REQUEST, "Bad Parameters", ex);
	}

	/**
	 * Returns an API HTTP error code of 400 if there are validation errors.
	 */
	@ExceptionHandler(HttpMessageNotReadableException.class)
	@ResponseStatus(HttpStatus.BAD_REQUEST)
	public ResponseEntity<Object> handleHttpMessageNotReadableException(HttpMessageNotReadableException ex) {
		logger.debug("handleHttpMessageNotReadableException", ex);
		return getResponse(HttpStatus.BAD_REQUEST, "Validation Failed", ex);
	}

	/**
	 * Returns an API HTTP error code of 500 if a NotYetImplementedException is thrown.
	 */
	@ExceptionHandler(NotYetImplementedException.class)
	@ResponseStatus(HttpStatus.INTERNAL_SERVER_ERROR)
	public ResponseEntity<Object> handleNotYetImplementedException(NotYetImplementedException ex) {
		logger.debug("handleNotYetImplementedException", ex);
		return getResponse(HttpStatus.INTERNAL_SERVER_ERROR, "Internal Server Error", ex);
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
