import { Disputant } from 'app/api';
import { Address } from './address.model';

// tslint:disable-next-line
export interface DisputantView extends Disputant {
  // TODO remove once backend has added this to their side
  mailingAddress: Address;
}
