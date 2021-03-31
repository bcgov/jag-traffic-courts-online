export interface Dispute {
  emailAddress?: string;

  count?: string;
  count1A1?: string;
  count1A2?: string;
  reductionReason?: string;
  timeReason?: string;
  count1B1?: string;
  count1B2?: string;

  lawyerPresent?: boolean;
  interpreterRequired?: boolean;
  interpreterLanguage?: string;
  callWitness?: boolean;

  certifyCorrect?: boolean;

  statusCode?: string;
  status?: string;
  note?: string;
}
