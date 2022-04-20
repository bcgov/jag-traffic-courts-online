package ca.bc.gov.open.jag.tco.oracledataapi;

import org.junit.jupiter.api.BeforeEach;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.test.context.ActiveProfiles;

import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputeRepository;

@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
@ActiveProfiles("test")
public class BaseTestSuite {

	@Autowired
	DisputeRepository disputeRepository;

    @BeforeEach
    protected void beforeEach() throws Exception {
    	disputeRepository.deleteAll();
    }

}
