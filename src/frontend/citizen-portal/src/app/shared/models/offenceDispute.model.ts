export interface OffenceDispute {
  id?: string;
  status: number;
  includeOffenceInDispute: boolean;
  offenceAgreementStatus?: string;
  requestReduction: boolean;
  requestMoreTime: boolean;
  reductionReason?: string;
  moreTimeReason?: string;
  informationCertified: boolean;
}
