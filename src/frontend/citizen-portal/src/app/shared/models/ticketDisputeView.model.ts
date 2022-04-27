import { AdditionalView } from './additionalView.model';
import { DisputantView } from './disputantView.model';
import { OffenceView } from './offenceView.model';

export class TicketDisputeView {
  violationTicketNumber?: string | null;
  violationTime?: string | null;
  violationDate?: Date | null;
  discountDueDate?: string | null;
  discountAmount?: number;
  disputant?: DisputantView;
  offences: OffenceView[];
  additional?: AdditionalView;

  _within30days?: boolean;
  _outstandingBalanceDue?: number;
  _totalBalanceDue?: number;
  _requestSubmitted?: boolean;
  countList?: any;
}
