import { TicketDispute } from 'app/api';
import { AdditionalView } from './additionalView.model';
import { DisputantView } from './disputantView.model';
import { OffenceView } from './offenceView.model';

export interface TicketDisputeView extends Omit<TicketDispute, 'disputant' | 'offences' | 'additional'> {
  disputant: DisputantView;
  offences: OffenceView[];
  additional: AdditionalView;

  _within30days?: boolean;
  _outstandingBalanceDue?: number;
  _totalBalanceDue?: number;
  _requestSubmitted?: boolean;
}
