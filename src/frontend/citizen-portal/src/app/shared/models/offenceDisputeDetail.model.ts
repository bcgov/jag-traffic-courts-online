export interface OffenceDisputeDetail {
  id?: string;
  offenceNumber: number;
  status: number;
  offenceAgreementStatus?: string;
  requestReduction: boolean;
  requestMoreTime: boolean;

  reductionReason?: string;
  moreTimeReason?: string;
  informationCertified: boolean;

  /*
  requestMethod: string;
  offenceAgreementStatus

    1 Nothing, I will pay the fine for this count
    2 Request a modification
        requestReduction: boolean;
        requestMoreTime: boolean;
        requestMethod
          1. Provide written reasons online
              reductionReason?: string;
              moreTimeReason?: string;
          2. Appear before a judge
    3 Dispute the charge


  */
}
