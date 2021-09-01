export interface Additional {
  id?: string;
  lawyerPresent: boolean;
  interpreterRequired: boolean;
  interpreterLanguage?: string;
  witnessPresent: boolean;
  numberOfWitnesses: number;

  requestReduction: boolean;
  requestMoreTime: boolean;
  reductionReason?: string;
  moreTimeReason?: string;

  _isCourtRequired: boolean;
  _isReductionRequired: boolean;
  _isReductionNotInCourt: boolean;
}
