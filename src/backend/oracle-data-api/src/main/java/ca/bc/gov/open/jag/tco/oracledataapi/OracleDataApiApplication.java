package ca.bc.gov.open.jag.tco.oracledataapi;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.scheduling.annotation.EnableScheduling;

@SpringBootApplication
@EnableScheduling
public class OracleDataApiApplication {

	private static Logger logger = LoggerFactory.getLogger(OracleDataApiApplication.class);

	public static void main(String[] args) {
		SpringApplication.run(OracleDataApiApplication.class, args);

		logger.info("Application up. Log level set to '{}'", ((ch.qos.logback.classic.Logger)logger).getEffectiveLevel());
	}

}
