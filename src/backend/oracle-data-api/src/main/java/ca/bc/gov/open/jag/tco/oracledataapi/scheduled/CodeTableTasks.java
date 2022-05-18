package ca.bc.gov.open.jag.tco.oracledataapi.scheduled;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;

import ca.bc.gov.open.jag.tco.oracledataapi.service.LookupService;

@Component
@ConditionalOnProperty(
        name = "cronjob.codetable.refresh.enabled",
        havingValue = "true",
        matchIfMissing = false)
public class CodeTableTasks {

	private Logger logger = LoggerFactory.getLogger(CodeTableTasks.class);

	@Autowired
	private LookupService lookupService;

	@Scheduled(cron = "${cronjob.codetable.refresh.cron}")
	public void refresh() {
		logger.debug("Scheduled 'codeTableRefresh' cron job called.");
		lookupService.refresh();
	}

}
