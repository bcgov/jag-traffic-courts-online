import { Dispute } from './dispute.model';

export interface Offence {
  invoiceType: string;
  offenceNumber: number;
  offenceDescription: string;
  violationDateTime: string;
  vehicleDescription: string;

  ticketedAmount: number;
  amountDue: number;
  discountAmount: number;
  discountDueDate: string;

  dispute: Dispute;

  // derived later on
  offenceStatus?: number;
  offenceStatusDesc?: string;
  notes?: string;
}
