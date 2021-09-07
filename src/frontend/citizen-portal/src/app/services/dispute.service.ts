import { Injectable } from '@angular/core';
import { ShellTicketData } from '@shared/models/shellTicketData.model';
import { TicketDisputeView } from '@shared/models/ticketDisputeView.model';
import { BehaviorSubject } from 'rxjs';

export interface IDisputeService {
  ticket$: BehaviorSubject<TicketDisputeView>;
  ticket: TicketDisputeView;
}

@Injectable({
  providedIn: 'root',
})
export class DisputeService implements IDisputeService {
  private _ticket: BehaviorSubject<TicketDisputeView>;
  private _shellTicketData: BehaviorSubject<ShellTicketData>;

  constructor() {
    this._ticket = new BehaviorSubject<TicketDisputeView>(null);
    this._shellTicketData = new BehaviorSubject<ShellTicketData>(null);
  }

  public get shellTicketData$(): BehaviorSubject<ShellTicketData> {
    return this._shellTicketData;
  }

  public get ticket$(): BehaviorSubject<TicketDisputeView> {
    return this._ticket;
  }

  public get ticket(): TicketDisputeView {
    return this._ticket.value;
  }

  public get shellTicketData(): ShellTicketData {
    return this._shellTicketData.value;
  }
}
