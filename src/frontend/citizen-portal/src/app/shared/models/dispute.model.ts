export interface Dispute {
  violationTicketNumber: string;
  offenceNumber: number;

  emailAddress?: string;

  offenceAgreementStatus?: string;
  requestReduction?: string;
  requestTime?: string;
  reductionReason?: string;
  timeReason?: string;

  lawyerPresent?: boolean;
  interpreterRequired?: boolean;
  interpreterLanguage?: string;
  witnessPresent?: boolean;

  informationCertified?: boolean;

  statusCode?: string;
  status?: string;
  note?: string;
}
