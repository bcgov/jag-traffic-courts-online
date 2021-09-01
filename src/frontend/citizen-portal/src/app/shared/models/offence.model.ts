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

  // derived later on
  _applyToAllCounts: boolean;
  _allowApplyToAllCounts: boolean;
  _firstOffence: boolean;
  _offenceStatus?: string;
  _offenceStatusDesc?: string;
  _within30days?: boolean;
  _amountDue?: number;
}
