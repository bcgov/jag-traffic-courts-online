import { Additional } from './additional.model';
import { Disputant } from './disputant.model';
import { OffenceDispute } from './offenceDispute.model';

export interface TicketDispute {
  violationTicketNumber: string;
  violationTime: string;

  // Part A
  disputant: Disputant;

  // Offences and Part B
  offences: OffenceDispute[];

  // Part C
  additional: Additional;
}
