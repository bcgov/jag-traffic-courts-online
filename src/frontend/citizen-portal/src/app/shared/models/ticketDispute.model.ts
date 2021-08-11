import { Additional } from './additional.model';
import { Disputant } from './disputant.model';
import { Offence } from './offence.model';
export interface TicketDispute {
  violationTicketNumber: string;
  violationTime: string;
  violationDate: string;

  discountDueDate?: string;

  disputant: Disputant;

  // Offences
  offences: Offence[];

  // Court information
  additional: Additional;

  // derived later on
  outstandingBalanceDue?: number;
  totalBalanceDue?: number;
  requestSubmitted?: boolean;
}
