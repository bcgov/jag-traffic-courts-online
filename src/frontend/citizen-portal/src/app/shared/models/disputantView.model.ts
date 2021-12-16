import { Disputant } from 'app/api';
import { Address } from './address.model';

// eslint-disable-next-line
export interface DisputantView extends Disputant {
  // TODO remove once backend has added this to their side
  _mailingAddress: Address;
}
