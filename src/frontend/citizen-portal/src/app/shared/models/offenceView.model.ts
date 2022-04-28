export class OffenceView {
  offenceNumber?: number;
  ticketedAmount?: number;
  amountDue?: number;
  violationDateTime?: string | null;
  offenceDescription?: string | null;
  vehicleDescription?: string | null;
  discountAmount?: number;
  discountDueDate?: string | null;
  invoiceType?: string | null;
  offenceAgreementStatus?: string | null;
  requestReduction?: boolean;
  requestMoreTime?: boolean;
  reductionAppearInCourt?: boolean | null;
  reductionReason?: string | null;
  moreTimeReason?: string | null;
  status?: any;
  _applyToAllCounts: boolean;
  _allowApplyToAllCounts: boolean;
  _firstOffence: boolean;
  _offenceStatus?: string;
  _offenceStatusDesc?: string;
  _within30days?: boolean;
  _amountDue?: number;
}
