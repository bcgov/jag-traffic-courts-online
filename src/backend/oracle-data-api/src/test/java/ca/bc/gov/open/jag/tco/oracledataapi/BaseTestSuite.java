package ca.bc.gov.open.jag.tco.oracledataapi;

import javax.transaction.Transactional;

import org.junit.jupiter.api.BeforeEach;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.test.context.ActiveProfiles;

import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputeRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRepository;

@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
@ActiveProfiles("test")
@Transactional
public class BaseTestSuite {

	@Value("${ords.enabled}")
    protected boolean ordsEnabled;

	@Autowired
	protected DisputeRepository disputeRepository;

	@Autowired
	protected JJDisputeRepository jjDisputeRepository;

    @BeforeEach
    protected void beforeEach() throws Exception {
    	// only delete the repo if this is a local H2 repository.
    	if (!ordsEnabled) {
	    	disputeRepository.deleteAll();
	    	jjDisputeRepository.deleteAll();
    	}
    }

}
