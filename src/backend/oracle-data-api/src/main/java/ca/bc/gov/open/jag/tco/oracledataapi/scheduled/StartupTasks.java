package ca.bc.gov.open.jag.tco.oracledataapi.scheduled;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.context.event.ApplicationReadyEvent;
import org.springframework.context.ApplicationListener;
import org.springframework.stereotype.Component;

import ca.bc.gov.open.jag.tco.oracledataapi.service.LookupService;

@Component
public class StartupTasks implements ApplicationListener<ApplicationReadyEvent> {

	private Logger logger = LoggerFactory.getLogger(StartupTasks.class);

	@Value("${codetable.refresh.atStartup}")
	private boolean refreshAtStartup;

	@Value("${ords.enabled}")
    protected boolean ordsEnabled;

	@Autowired
	private LookupService lookupService;

	@Override
	public void onApplicationEvent(ApplicationReadyEvent event) {
		if (refreshAtStartup) {
			logger.debug("Refreshing code tables at startup.");
			lookupService.refresh();
		}

		if (ordsEnabled) {
			logger.info("ORDS is enabled: using ORDS as the database repository.");
		}
		else {
			logger.info("ORDS is disabled: using H2 as the database repository.");
		}
	}

}
