package ca.bc.gov.open.jag.tco.oracledatainterface;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.scheduling.annotation.EnableScheduling;

@SpringBootApplication
@EnableScheduling
public class OracleDataInterfaceApplication {

	private static Logger logger = LoggerFactory.getLogger(OracleDataInterfaceApplication.class);

	public static void main(String[] args) {
		SpringApplication.run(OracleDataInterfaceApplication.class, args);

		logger.info("Application up. Log level set to '{}'", ((ch.qos.logback.classic.Logger)logger).getEffectiveLevel());
	}

}
