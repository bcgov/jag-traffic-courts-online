import { Injectable } from '@angular/core';
import { Ticket } from '@shared/models/ticket.model';
import { BehaviorSubject } from 'rxjs';

export interface IDisputeService {
  ticket$: BehaviorSubject<Ticket>;
  ticket: Ticket;
  // ticketDispute: TicketDispute;
}

@Injectable({
  providedIn: 'root',
})
export class DisputeService {
  private disputeSteps: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);

  // tslint:disable-next-line: variable-name
  private _ticket: BehaviorSubject<Ticket>;
  private _tickets: BehaviorSubject<Ticket[]>;
  // private _ticketDispute: BehaviorSubject<TicketDispute>;

  constructor() {
    this._ticket = new BehaviorSubject<Ticket>(null);
    this._tickets = new BehaviorSubject<Ticket[]>(null);
    // this._ticketDispute = new BehaviorSubject<TicketDispute>(null);
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

  // public get ticketDispute$(): BehaviorSubject<TicketDispute> {
  //   return this._ticketDispute;
  // }

  // public get ticketDispute(): TicketDispute {
  //   return this._ticketDispute.value;
  // }

  // private createDispute(
  //   violationTicketNumber: string,
  //   offenceNumber: number
  // ): Dispute {
  //   return {
  //     violationTicketNumber,
  //     offenceNumber,
  //     status: 0,
  //     requestReduction: false,
  //     requestMoreTime: false,
  //     lawyerPresent: false,
  //     interpreterRequired: false,
  //     witnessPresent: false,
  //     informationCertified: false,
  //   };
  // }

  // public getTicketDispute(ticket: Ticket, offences: Offence[]): TicketDispute {
  //   let offence1: Offence = null;
  //   let offence2: Offence = null;
  //   let offence3: Offence = null;

  //   offences.forEach((offence) => {
  //     console.log('getTicketDispute', offence);
  //     if (offence.offenceNumber === 1) {
  //       if (!offence.dispute) {
  //         offence1 = offence;
  //         offence1.dispute = this.createDispute(
  //           ticket.violationTicketNumber,
  //           offence.offenceNumber
  //         );
  //       }
  //     } else if (offence.offenceNumber === 2) {
  //       if (!offence.dispute) {
  //         offence2 = offence;
  //         offence2.dispute = this.createDispute(
  //           ticket.violationTicketNumber,
  //           offence.offenceNumber
  //         );
  //       }
  //     } else if (offence.offenceNumber === 3) {
  //       if (!offence.dispute) {
  //         offence3 = offence;
  //         offence3.dispute = this.createDispute(
  //           ticket.violationTicketNumber,
  //           offence.offenceNumber
  //         );
  //       }
  //     }
  //   });

  //   const ticketDispute = {
  //     violationTicketNumber: ticket.violationTicketNumber,
  //     violationTime: ticket.violationTime,
  //     offence1,
  //     offence2,
  //     offence3,
  //   };

  //   console.log('ticketDispute', ticketDispute);
  //   return ticketDispute;
  // }
}
