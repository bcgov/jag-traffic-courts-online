import { Additional } from './additional.model';
import { Disputant } from './disputant.model';
import { Offence } from './offence.model';

export interface Ticket {
  violationTicketNumber: string;
  violationTime: string;
  violationDate: string;

  // Part A
  disputant: Disputant;

  // Offences and Part B
  offences: Offence[];

  // Part C
  additional: Additional;

  // derived later on
  outstandingBalance?: number;
  disputesExist?: boolean;
}
