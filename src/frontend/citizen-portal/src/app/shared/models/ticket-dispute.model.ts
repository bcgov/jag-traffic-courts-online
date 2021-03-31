import { Offence } from './ticket.model';

export interface TicketDispute {
  violationTicketNumber: string;
  violationTime: string;
  offence: Offence;
}
