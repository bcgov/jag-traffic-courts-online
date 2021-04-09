import { Injectable } from '@angular/core';
import { Offence } from '@shared/models/offence.model';
import { TicketDispute } from '@shared/models/ticket-dispute.model';
import { Ticket } from '@shared/models/ticket.model';
import { BehaviorSubject } from 'rxjs';

export interface IDisputeService {
  ticket$: BehaviorSubject<Ticket>;
  ticket: Ticket;
  ticketDispute: TicketDispute;
}

@Injectable({
  providedIn: 'root',
})
export class DisputeService {
  private disputeSteps: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);

  // tslint:disable-next-line: variable-name
  private _ticket: BehaviorSubject<Ticket>;
  private _tickets: BehaviorSubject<Ticket[]>;
  private _ticketDispute: BehaviorSubject<TicketDispute>;

  constructor() {
    this._ticket = new BehaviorSubject<Ticket>(null);
    this._tickets = new BehaviorSubject<Ticket[]>(null);
    this._ticketDispute = new BehaviorSubject<TicketDispute>(null);
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

  public get ticketDispute$(): BehaviorSubject<TicketDispute> {
    return this._ticketDispute;
  }

  public get ticketDispute(): TicketDispute {
    return this._ticketDispute.value;
  }

  public getDisputeTicket(ticket: Ticket, oneOffence: Offence): TicketDispute {
    if (!oneOffence.dispute) {
      oneOffence.dispute = {
        violationTicketNumber: ticket.violationTicketNumber,
        offenceNumber: oneOffence.offenceNumber,
      };
    }

    const ticketDispute = {
      violationTicketNumber: ticket.violationTicketNumber,
      violationTime: ticket.violationTime,
      offence: oneOffence,
    };

    return ticketDispute;
  }
}
