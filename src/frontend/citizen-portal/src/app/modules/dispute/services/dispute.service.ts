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
  private newSteps: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);

  // tslint:disable-next-line: variable-name
  private _ticket: BehaviorSubject<Ticket>;

  constructor() {
    this._ticket = new BehaviorSubject<Ticket>(null);

    let steps = this.steps$.value;
    steps.push({ title: 'Review', value: null, pageName: 1 });
    steps.push({ title: 'Count', value: null, pageName: 2 });
    this.steps$.next(steps);
  }

  public get ticket$(): BehaviorSubject<Ticket> {
    return this._ticket;
  }

  public get ticket(): Ticket {
    return this._ticket.value;
  }

  public get steps$(): BehaviorSubject<any> {
    return this.newSteps;
  }
}
