import { Injectable } from '@angular/core';
import { Ticket } from '@shared/models/ticket.model';
import { BehaviorSubject } from 'rxjs';

export interface IDisputeService {
  ticket$: BehaviorSubject<Ticket>;
  ticket: Ticket;
}

@Injectable({
  providedIn: 'root',
})
export class DisputeService {
  private disputeSteps: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);

  // tslint:disable-next-line: variable-name
  private _ticket: BehaviorSubject<Ticket>;
  private _tickets: BehaviorSubject<Ticket[]>;

  constructor() {
    this._ticket = new BehaviorSubject<Ticket>(null);
    this._tickets = new BehaviorSubject<Ticket[]>(null);
  }

  public get ticket$(): BehaviorSubject<Ticket> {
    return this._ticket;
  }

  public get tickets$(): BehaviorSubject<Ticket[]> {
    return this._tickets;
  }

  public get ticket(): Ticket {
    return this._ticket.value;
  }
}
