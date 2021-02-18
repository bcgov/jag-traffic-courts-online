export interface Ticket {
  id?: number;
  userId: string;
  violationTicketNumber: string;
  courtLocation: string;
  violationDate: Date;
  surname: string;
  givenNames: string;
  mailing: string;
  postal: string;
  city: string;
  province: string;
  license: string;
  provLicense: string;
  homePhone: string;
  workPhone: string;
  birthdate: Date;
  lawyerPresent: boolean;
  interpreterRequired: boolean;
  interpreterLanguage: string;
  callWitness: boolean;
  counts: Count[];
}

export interface Count {
  countNo: number;
  statuteId: number;
  description: string;
}
