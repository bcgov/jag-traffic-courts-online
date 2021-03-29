import { Dispute } from './dispute.model';

export interface Ticket {
  violationTicketNumber: string;
  violationTime: string;
  offences: Offence[];

  // derived later on
  outstandingBalance?: number;
}

export interface Offence {
  offenceNumber: number;
  ticketAmount: number;
  amountDue: number;
  dueDate: string;
  description: string;

  // derived later on
  earlyAmount?: number;
  statusCode?: string;
  statusDesc?: string;
  notes?: string;

  dispute: Dispute;
}
