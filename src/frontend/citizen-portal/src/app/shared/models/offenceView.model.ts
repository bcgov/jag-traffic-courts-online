import { Offence } from 'app/api';

export interface OffenceView extends Offence {
  _applyToAllCounts: boolean;
  _allowApplyToAllCounts: boolean;
  _firstOffence: boolean;
  _offenceStatus?: string;
  _offenceStatusDesc?: string;
  _within30days?: boolean;
  _amountDue?: number;
}
