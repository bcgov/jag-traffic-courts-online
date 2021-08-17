import { Injectable } from '@angular/core';
import { ShellTicket } from '@shared/models/shellTicket.model';
import { TicketDispute } from '@shared/models/ticketDispute.model';
import { BehaviorSubject } from 'rxjs';

export interface IDisputeService {
  ticket$: BehaviorSubject<TicketDispute>;
  ticket: TicketDispute;
  shellTicket: ShellTicket;
}

@Injectable({
  providedIn: 'root',
})
export class DisputeService {
  // tslint:disable-next-line: variable-name
  private _ticket: BehaviorSubject<TicketDispute>;
  private _shellTicket: BehaviorSubject<ShellTicket>;
  // private _tickets: BehaviorSubject<TicketDispute[]>;

  constructor() {
    this._ticket = new BehaviorSubject<TicketDispute>(null);
    this._shellTicket = new BehaviorSubject<ShellTicket>(null);
    // this._tickets = new BehaviorSubject<TicketDispute[]>(null);
  }

  public get shellTicket$(): BehaviorSubject<ShellTicket> {
    return this._shellTicket;
  }

  public get ticket$(): BehaviorSubject<TicketDispute> {
    return this._ticket;
  }

  // public get tickets$(): BehaviorSubject<TicketDispute[]> {
  //   return this._tickets;
  // }

  public get ticket(): TicketDispute {
    return this._ticket.value;
  }

  public get shellTicket(): ShellTicket {
    return this._shellTicket.value;
  }
}
