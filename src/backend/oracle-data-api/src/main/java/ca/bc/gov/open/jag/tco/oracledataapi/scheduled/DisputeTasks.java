package ca.bc.gov.open.jag.tco.oracledataapi.scheduled;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;

import ca.bc.gov.open.jag.tco.oracledataapi.service.DisputeService;

@Component
@ConditionalOnProperty(
        name = "cronjob.dispute.unassign.enabled",
        havingValue = "true",
        matchIfMissing = false)
public class DisputeTasks {

	private Logger logger = LoggerFactory.getLogger(DisputeTasks.class);

	@Autowired
	private DisputeService disputeService;

	@Scheduled(cron = "${cronjob.dispute.unassign.cron}")
	public void unassignDisputes() {
		logger.debug("Scheduled 'unassignDisputes' cron job called.");
		disputeService.unassignDisputes();
	}

}
