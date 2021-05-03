import { Additional } from './additional.model';
import { OffenceDisputeDetail } from './offenceDisputeDetail.model';

export interface CountDispute {
  violationTicketNumber: string;
  violationTime: string;

  // Part B
  offenceDisputeDetail: OffenceDisputeDetail;

  // Part C
  additional: Additional;
}
