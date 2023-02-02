package ca.bc.gov.open.jag.tco.oracledataapi.scheduled;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;

import ca.bc.gov.open.jag.tco.oracledataapi.service.JJDisputeService;

@Component
@ConditionalOnProperty(
        name = "cronjob.dispute.unassign.enabled",
        havingValue = "true",
        matchIfMissing = false)
public class JJDisputeTasks {

	private Logger logger = LoggerFactory.getLogger(JJDisputeTasks.class);

	@Autowired
	private JJDisputeService jjDisputeService;

	@Scheduled(cron = "${cronjob.jj-dispute.unassign.cron}")
	public void unassignJJDisputes() {
		logger.debug("Scheduled 'unassignJJDisputes' cron job called.");
		jjDisputeService.unassignJJDisputes();
	}

}
