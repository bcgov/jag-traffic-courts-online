import { Additional } from './additional.model';
import { OffenceDispute } from './offenceDispute.model';

export interface CountDispute {
  violationTicketNumber: string;
  violationTime: string;
  violationDate: string;

  // Part B
  offence: OffenceDispute;

  // Part C
  additional: Additional;
}
