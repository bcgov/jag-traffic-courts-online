export interface Offence {
  id?: string;
  offenceNumber: number;
  invoiceType?: string;
  offenceDescription?: string;
  violationDateTime?: string;
  vehicleDescription?: string;

  ticketedAmount: number;
  amountDue: number;
  discountDueDate?: string;
  discountAmount: number;

  status: string;
  offenceAgreementStatus?: string;
  reductionAppearInCourt: boolean;
}
