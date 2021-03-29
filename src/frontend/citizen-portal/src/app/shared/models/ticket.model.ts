export interface Ticket {
  id?: number;
  userId: string;
  violationTicketNumber: string;
  violationDate: string;
  violationTime: string;
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
  counts: Count[];
  outstandingBalance?: number;
}

export interface Count {
  id: number;
  countNo: number;
  statuteId: number;
  description: string;
}
