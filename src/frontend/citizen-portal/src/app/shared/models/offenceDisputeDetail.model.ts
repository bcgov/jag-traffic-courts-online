export interface OffenceDisputeDetail {
  id?: string;
  offenceNumber: number;
  status: number;
  offenceAgreementStatus?: string;
  reductionAppearInCourt?: string;
  requestReduction: boolean;
  requestMoreTime: boolean;

  reductionReason?: string;
  moreTimeReason?: string;
  informationCertified: boolean;

  /*
  requestMethod: string;
  offenceAgreementStatus
  */
}
