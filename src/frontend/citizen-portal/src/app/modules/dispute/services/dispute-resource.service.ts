import { Injectable } from '@angular/core';
import { Ticket } from '@shared/models/ticket.model';
import { BehaviorSubject } from 'rxjs';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';

@Injectable({
  providedIn: 'root',
})
export class DisputeResourceService {
  private _ticket: BehaviorSubject<Ticket>;

  constructor(private service: MockDisputeService) {
    this._ticket = new BehaviorSubject<Ticket>(this.service.ticket);
  }

  public get ticket$(): BehaviorSubject<Ticket> {
    return this._ticket;
  }

  public get ticket(): Ticket {
    return this._ticket.value;
  }
}
