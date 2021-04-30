import { Additional } from './additional.model';
import { OffenceDispute } from './offenceDispute.model';

export interface CountDispute {
  violationTicketNumber: string;
  violationTime: string;

  // Part B
  offenceDisputeDetail: OffenceDispute;

  // Part C
  additional: Additional;
}
