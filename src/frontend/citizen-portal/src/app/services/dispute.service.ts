import { Injectable } from '@angular/core';
import { ShellTicket } from '@shared/models/shellTicket.model';
import { ShellTicketData } from '@shared/models/shellTicketData.model';
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
  // private _shellTicket: BehaviorSubject<ShellTicket>;
  private _shellTicketData: BehaviorSubject<ShellTicketData>;

  constructor() {
    this._ticket = new BehaviorSubject<TicketDispute>(null);
    // this._shellTicket = new BehaviorSubject<ShellTicket>(null);
    this._shellTicketData = new BehaviorSubject<ShellTicketData>(null);
  }

  // public get shellTicket$(): BehaviorSubject<ShellTicket> {
  //   return this._shellTicket;
  // }

  public get shellTicketData$(): BehaviorSubject<ShellTicketData> {
    return this._shellTicketData;
  }

  public get ticket$(): BehaviorSubject<TicketDispute> {
    return this._ticket;
  }

  public get ticket(): TicketDispute {
    return this._ticket.value;
  }

  // public get shellTicket(): ShellTicket {
  //   return this._shellTicket.value;
  // }

  public get shellTicketData(): ShellTicketData {
    return this._shellTicketData.value;
  }
}
