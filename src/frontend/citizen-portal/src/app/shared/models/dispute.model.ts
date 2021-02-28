export interface Dispute {
  id?: number;
  // userId: string;
  // lawyerPresent: boolean;
  // interpreterRequired: boolean;
  // interpreterLanguage: string;
  // callWitness: boolean;
  // counts: Count[];
}

export interface Count {
  countNo: number;
  statuteId: number;
  description: string;
}
