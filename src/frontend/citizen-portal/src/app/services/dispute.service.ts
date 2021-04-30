import { Injectable } from '@angular/core';
import { TcoTicketDispute } from '@shared/models/tcoTicketDispute.model';
import { BehaviorSubject } from 'rxjs';

export interface IDisputeService {
  ticket$: BehaviorSubject<TcoTicketDispute>;
  ticket: TcoTicketDispute;
}

@Injectable({
  providedIn: 'root',
})
export class DisputeService {
  private disputeSteps: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);

  // tslint:disable-next-line: variable-name
  private _ticket: BehaviorSubject<TcoTicketDispute>;
  private _tickets: BehaviorSubject<TcoTicketDispute[]>;

  constructor() {
    this._ticket = new BehaviorSubject<TcoTicketDispute>(null);
    this._tickets = new BehaviorSubject<TcoTicketDispute[]>(null);
  }

  public get ticket$(): BehaviorSubject<TcoTicketDispute> {
    return this._ticket;
  }

  public get tickets$(): BehaviorSubject<TcoTicketDispute[]> {
    return this._tickets;
  }

  public get ticket(): TcoTicketDispute {
    return this._ticket.value;
  }
}
