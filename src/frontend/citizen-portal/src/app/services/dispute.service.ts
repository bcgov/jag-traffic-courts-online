import { Injectable } from '@angular/core';
import { TicketDispute } from '@shared/models/ticketDispute.model';
import { BehaviorSubject } from 'rxjs';

export interface IDisputeService {
  ticket$: BehaviorSubject<TicketDispute>;
  ticket: TicketDispute;
}

@Injectable({
  providedIn: 'root',
})
export class DisputeService {
  // private disputeSteps: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);

  // tslint:disable-next-line: variable-name
  private _ticket: BehaviorSubject<TicketDispute>;
  private _tickets: BehaviorSubject<TicketDispute[]>;

  constructor() {
    this._ticket = new BehaviorSubject<TicketDispute>(null);
    this._tickets = new BehaviorSubject<TicketDispute[]>(null);
  }

  public get ticket$(): BehaviorSubject<TicketDispute> {
    return this._ticket;
  }

  public get tickets$(): BehaviorSubject<TicketDispute[]> {
    return this._tickets;
  }

  public get ticket(): TicketDispute {
    return this._ticket.value;
  }
}
