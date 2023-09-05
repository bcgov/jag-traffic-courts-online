package ca.bc.gov.court.traffic.ticket;

import org.junit.jupiter.api.Test;

import ca.bc.gov.court.traffic.ticket.util.RandomUtil;

class RandomUtilTest {

	@Test
	void test() {
		int t = 0;
		int f = 0;
		for (int i = 0; i < 100; i++) {
			if (RandomUtil.randomBool())
				t++;
			else 
				f++;
		}
		System.out.println(t);
		System.out.println(f);
	}

}
