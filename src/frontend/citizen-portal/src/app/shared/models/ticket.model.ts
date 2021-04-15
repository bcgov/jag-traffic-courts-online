import { Offence } from './offence.model';

export interface Ticket {
  violationTicketNumber: string;
  violationTime: string;
  violationDate: string;
  offences: Offence[];

  // derived later on
  outstandingBalance?: number;
  disputesExist?: boolean;
}
