import { OffenceDispute } from './offenceDispute.model';

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

  // Part B
  offenceDispute: OffenceDispute;

  // derived later on
  offenceStatus?: number;
  offenceStatusDesc?: string;
  notes?: string;
}
