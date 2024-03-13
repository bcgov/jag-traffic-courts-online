package ca.bc.gov.open.jag.tco.oracledataapi.config;

import org.aspectj.lang.ProceedingJoinPoint;
import org.aspectj.lang.annotation.Around;
import org.aspectj.lang.annotation.Aspect;
import org.aspectj.lang.annotation.Pointcut;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

@Aspect
@Component
//@EnableRetry
public class RetryAspect {

	private static Logger logger = LoggerFactory.getLogger(RetryAspect.class);

	@Value("${ords.api.retry.count}")
	private int maxAttempts;

	@Value("${ords.api.retry.delay}")
	private long retryDelay;

	// This pointcut will match any public method on any class in the ords repository package (DisputeRepositoryImpl, JJDisputeRepositoryImpl, etc.)
	// as well as the LookupValuesApi class that retrieves lookup values from ords.
	@Pointcut("execution(public * ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.ords.*.*(..)) || "
			+ "execution(public * ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.LookupValuesApi.*(..))")
	public void retryOnOrdsApiMethods() {
	}

	//Unfortunately, @Retryable is not supported in AspectJ so this doesn't work
	//@Retryable(maxAttemptsExpression = "${ords.api.retry.count}", backoff = @Backoff(delayExpression = "${ords.api.retry.delay}"))
	@Around("retryOnOrdsApiMethods()")
	public Object around(ProceedingJoinPoint pjp) throws Throwable {
		for (int i = 0; i < maxAttempts; i++) {
			try {
				return pjp.proceed();
			} 			
			// Thrown by the Occam API, usually Policy Falsified error
			catch (ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.handler.ApiException e) {
				handleException(pjp, i, e);
			} 			
			// Thrown by the TCO API, usually Policy Falsified error
			catch (ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.handler.ApiException e) {
				handleException(pjp, i, e);
			} 			
			// javax.ws.rs.InternalServerErrorException is thrown when there is a database error (no point retrying these)
			//catch (javax.ws.rs.InternalServerErrorException e) {
			//	handleException(pjp, i, e);
			//}
		}
		return null; // should never reach here
	}

	private void handleException(ProceedingJoinPoint pjp, int i, Exception e) throws Exception {
		if (i < maxAttempts - 1) {
			// delay before retrying
			logger.debug("Attempt {} failed, retrying: {}", i + 1, pjp.getSignature().toShortString());
			Thread.sleep(retryDelay);
		}

		else {
			// re-throw the exception as this is the last attempt
			logger.error("Attempt {} failed, calling: {}", maxAttempts, pjp.getSignature().toShortString(), e);
			throw e;
		}
	}

}
