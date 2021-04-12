import { Offence } from './offence.model';

export interface TicketDispute {
  violationTicketNumber: string;
  violationTime: string;
  offence: Offence;
}
