package ca.bc.gov.open.jag.tco.oracledataapi.scheduled;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.boot.context.event.ApplicationReadyEvent;
import org.springframework.context.ApplicationListener;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;

import ca.bc.gov.open.jag.tco.oracledataapi.service.LookupService;

@Component
@ConditionalOnProperty(
        name = "codetable.refresh.enabled",
        havingValue = "true",
        matchIfMissing = false)
public class CodeTableTasks implements ApplicationListener<ApplicationReadyEvent>  {

	private Logger logger = LoggerFactory.getLogger(CodeTableTasks.class);

	@Value("${codetable.refresh.atStartup}")
	private boolean refreshAtStartup;

	@Autowired
	private LookupService lookupService;

	@Scheduled(cron = "${codetable.refresh.cron}")
	public void refresh() {
		logger.debug("Scheduled 'codeTableRefresh' cron job called.");
		lookupService.refresh();
	}

	@Override
	public void onApplicationEvent(ApplicationReadyEvent event) {
		if (refreshAtStartup) {
			logger.debug("Refreshing code tables at startup.");
			lookupService.refresh();
		}
	}

}
