export class AdditionalView {
  lawyerPresent?: boolean;
  interpreterRequired?: boolean;
  interpreterLanguage?: string | null;
  witnessPresent?: boolean;
  numberOfWitnesses?: number | null;
  requestReduction?: boolean;
  requestMoreTime?: boolean;
  reductionReason?: string | null;
  moreTimeReason?: string | null;
  _isCourtRequired: boolean;
  _isReductionRequired: boolean;
  _isReductionNotInCourt: boolean; // When this is true, the reason is required, otherwise it is not
}
