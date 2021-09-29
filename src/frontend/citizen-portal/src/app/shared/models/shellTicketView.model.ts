import { ShellTicket } from 'app/api';
import { Address } from './address.model';

export interface ShellTicketView extends ShellTicket {
  _chargeCount: number;
  _amountOwing: number;
  _count1ChargeDesc: string;
  _count2ChargeDesc: string;
  _count3ChargeDesc: string;
  _count1ChargeSection: string;
  _count2ChargeSection: string;
  _count3ChargeSection: string;
  _mailingAddress: Address;
}
