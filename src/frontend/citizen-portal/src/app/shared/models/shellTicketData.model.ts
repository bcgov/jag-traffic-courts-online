import { ShellTicket } from './shellTicket.model';

export interface ShellTicketData {
  shellTicket?: ShellTicket;
  filename?: string;
  ticketImage?: string;
  ticketFile?: File;
}
