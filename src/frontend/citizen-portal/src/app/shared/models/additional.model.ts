export interface Additional {
  id?: string;
  lawyerPresent: boolean;
  interpreterRequired: boolean;
  interpreterLanguage?: string;
  witnessPresent: boolean;
  numberOfWitnesses: number;

  _isCourtRequired: boolean;
  _isReductionRequired: boolean;
}
