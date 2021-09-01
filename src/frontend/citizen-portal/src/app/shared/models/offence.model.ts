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
  applyToAllCounts: boolean;

  status: string;
  offenceAgreementStatus?: string;
  reductionAppearInCourt: boolean;
  requestReduction: boolean;
  requestMoreTime: boolean;
  reductionReason?: string;
  moreTimeReason?: string;

  // derived later on
  _firstOffence: boolean;
  _offenceStatus?: string;
  _offenceStatusDesc?: string;
  _within30days?: boolean;
  _amountDue?: number;
}
