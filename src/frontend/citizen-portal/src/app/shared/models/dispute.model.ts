export interface Dispute {
  id?: number;
  violationTicketNumber: string;
  offenceNumber: number;

  emailAddress?: string;

  offenceAgreementStatus?: string;
  requestReduction: boolean;
  requestMoreTime: boolean;
  reductionReason?: string;
  moreTimeReason?: string;

  lawyerPresent: boolean;
  interpreterRequired: boolean;
  interpreterLanguage?: string;
  witnessPresent: boolean;

  informationCertified: boolean;

  statusCode?: string;
  status?: string;
  note?: string;
}
