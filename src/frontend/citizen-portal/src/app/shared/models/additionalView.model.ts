import { Additional } from "./additional.model";

export interface AdditionalView extends Additional {
  _isCourtRequired: boolean;
  _isReductionRequired: boolean;
  _isReductionNotInCourt: boolean; // When this is true, the reason is required, otherwise it is not
}
