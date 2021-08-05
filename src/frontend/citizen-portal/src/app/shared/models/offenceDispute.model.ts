import { Additional } from './additional.model';
import { OffenceDisputeDetail } from './offenceDisputeDetail.model';

export interface OffenceDispute {
  violationTicketNumber: string;
  violationTime: string;
  offenceNumber: number;
  // informationCertified: boolean;

  // Part B
  offenceDisputeDetail: OffenceDisputeDetail;

  // Part C
  additional: Additional;
}
