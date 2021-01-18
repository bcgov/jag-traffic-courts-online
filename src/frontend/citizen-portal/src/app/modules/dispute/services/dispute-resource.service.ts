import { Injectable } from '@angular/core';
import { Ticket } from '@shared/models/ticket.model';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DisputeResourceService {
  private _ticket: BehaviorSubject<Ticket>;

  constructor() {
    this._ticket = new BehaviorSubject<Ticket>(null);
  }

  public get ticket$(): BehaviorSubject<Ticket> {
    return this._ticket;
  }
}
