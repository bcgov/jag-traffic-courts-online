import { Offence } from './offence.model';

export interface TicketDispute {
  violationTicketNumber: string;
  violationTime: string;
  offence1: Offence;
  offence2: Offence;
  offence3: Offence;
}
