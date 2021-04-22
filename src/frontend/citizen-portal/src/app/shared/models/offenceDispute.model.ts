export interface OffenceDispute {
  id?: string;
  offenceNumber: number;
  status: number;
  offenceAgreementStatus?: string;
  requestReduction: boolean;
  requestMoreTime: boolean;
  reductionReason?: string;
  moreTimeReason?: string;
  informationCertified: boolean;
}
