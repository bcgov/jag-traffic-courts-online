import { Dispute } from './dispute.model';

export interface Offence {
  offenceNumber: number;
  ticketAmount: number;
  amountDue: number;
  dueDate: string;
  description: string;
  dispute: Dispute;

  // derived later on
  earlyAmount?: number;
  statusCode?: string;
  statusDesc?: string;
  notes?: string;
}
