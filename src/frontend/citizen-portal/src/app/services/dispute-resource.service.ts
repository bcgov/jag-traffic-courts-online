import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ConfigService } from '@config/config.service';
import { ApiHttpErrorResponse } from '@core/models/api-http-error-response.model';
import { ApiHttpResponse } from '@core/models/api-http-response.model';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { ShellTicketView } from '@shared/models/shellTicketView.model';
import { TicketDisputeView } from '@shared/models/ticketDisputeView.model';
import { DisputeAPIService, ShellTicket, TicketAPIService, TicketDispute } from 'app/api';
import { Observable } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class DisputeResourceService {
  constructor(
    private toastService: ToastService,
    private logger: LoggerService,
    private configService: ConfigService,
    private ticketAPIService: TicketAPIService,
    private disputeAPIService: DisputeAPIService
  ) { }

  /**
   * Get the ticket from RSI.
   *
   * @param params containing the ticketNumber and time
   */
  public getTicket(params: {
    ticketNumber: string;
    time: string;
  }): Observable<TicketDisputeView> {

    return this.ticketAPIService.ticketsTicket(params.ticketNumber, params.time)
      .pipe(
        map((response: ApiHttpResponse<TicketDispute>) =>
          response ? response.result : null
        ),
        map((ticket: TicketDisputeView) => {
          if (ticket) {
            this.updateTicketViewModel(ticket);
          }

          return ticket;
        }),
        tap((updatedTicket) =>
          this.logger.info('DisputeResourceService::getTicket', updatedTicket)
        ),
        catchError((error: any) => {
          this.toastService.openErrorToast(this.configService.ticket_error);
          this.logger.error(
            'DisputeResourceService::getTicket error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  /**
   * Initiate  a ticket payment.
   *
   * @param params containing the ticketNumber, time and counts
   */
  public initiateTicketPayment(params: {
    ticketNumber: string;
    time: string;
    counts: string;
    amount: number;
  }): Observable<any> {

    return this.ticketAPIService.ticketsPay(params.ticketNumber, params.time, params.counts, params.amount.toString()).pipe(
      map((response: ApiHttpResponse<any>) =>
        response ? response.result : null
      ),
      tap((result) =>
        this.logger.info(
          'DisputeResourceService::initiateTicketPayment',
          result
        )
      ),
      catchError((error: any) => {
        this.toastService.openErrorToast('Payment could not be made');
        this.logger.error(
          'DisputeResourceService::initiateTicketPayment error has occurred: ',
          error
        );
        throw error;
      })
    );
  }

  /**
   * After a return from the initiating of a payment, notify the backend of the results
   *
   * @param params containing the id, amount, status, confNo, and transId
   */
  public makeTicketPayment(params: {
    id: string;
    amount?: string;
    status: string;
    confNo?: string;
    transId?: string;
  }): Observable<TicketDisputeView> {
    const isPaid = status === 'paid';

    if (!isPaid) {
      params.amount = '0';
      delete params.confNo;
      delete params.transId;
    }

    return this.ticketAPIService.ticketsPay2(params.id, params.status, params.amount, params.confNo, params.transId).pipe(
      map((response: ApiHttpResponse<any>) =>
        response ? response.result : null
      ),
      map((ticket: TicketDisputeView) => {
        if (ticket) {
          this.updateTicketViewModel(ticket);
        }
        return ticket;
      }),
      tap((updatedTicket) => {
        if (isPaid) {
          this.toastService.openSuccessToast('Payment was successful');
        }
        this.logger.info('DisputeResourceService::makeTicketPayment', updatedTicket);
      }),
      catchError((error: any) => {
        this.toastService.openErrorToast('Payment could not be made');
        this.logger.error(
          'DisputeResourceService::makeTicketPayment error has occurred: ',
          error
        );
        throw error;
      })
    );
  }

  /**
   * Create the ticket dispute
   *
   * @param ticketDispute The dispute to be created
   */
  public createTicketDispute(
    ticketDispute: TicketDisputeView
  ): Observable<TicketDisputeView> {
    const ticketToCreate = this.cleanTicketDispute(ticketDispute);
    this.logger.info(
      'DisputeResourceService::createTicketDispute',
      ticketToCreate
    );

    return this.disputeAPIService.disputesTicketDispute(ticketDispute).pipe(
      map((response: ApiHttpResponse<TicketDisputeView>) =>
        response ? response.result : null
      ),
      map((ticket: TicketDisputeView) => {
        if (ticket) {
          this.updateTicketViewModel(ticket);
        }
        return ticket;
      }),
      tap((updatedTicket) => {
        this.toastService.openSuccessToast(
          'The request has been successfully submitted'
        );

        this.logger.info(
          'DisputeResourceService::NEW_DISPUTE_TICKET',
          updatedTicket
        );
      }),
      catchError((error: any) => {
        this.toastService.openErrorToast('The request could not be created');
        this.logger.error(
          'DisputeResourceService::createTicketDispute error has occurred: ',
          error
        );
        throw error;
      })
    );
  }

  /**
   * Create the shell ticket
   *
   * @param ticket The ticket to be created
   */
  public createShellTicket(ticket: ShellTicketView): Observable<TicketDisputeView> {
    const ticketToCreate = this.cleanShellTicket(ticket);
    this.logger.info('DisputeResourceService::createShellTicket', ticketToCreate);

    return this.ticketAPIService.ticketsShellTicket(ticket)
      .pipe(
        map((response: ApiHttpResponse<TicketDisputeView>) =>
          response ? response.result : null
        ),
        map((savedTicket: TicketDisputeView) => {
          if (savedTicket) {
            this.updateTicketViewModel(savedTicket);
          }
          return savedTicket;
        }),
        tap((updatedTicket) => {
          this.toastService.openSuccessToast(
            'The ticket has been successfully created'
          );

          this.logger.info(
            'DisputeResourceService:: NEW_SHELL_TICKET',
            updatedTicket
          );
        }),
        catchError((error: ApiHttpErrorResponse) => {
          if (Array.isArray(error.errors) && error.errors.length > 0) {
            const msg = error.errors.join(' ');
            this.toastService.openErrorToast(msg);
          } else {
            this.toastService.openErrorToast('Ticket could not be created');
          }
          this.logger.error(
            'DisputeResourceService::createShellTicket error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  /**
   * @description
   * return the calculated offence status and description
   */
  private getOffenceStatusDesc(
    status: string,
    offenceAgreementStatus: string,
    amountDue: number
  ): { offenceStatus: string; offenceStatusDesc: string } {
    let offenceStatus = '';
    let offenceStatusDesc = '';

    if (
      offenceAgreementStatus &&
      (offenceAgreementStatus === 'DISPUTE' ||
        offenceAgreementStatus === 'REDUCTION')
    ) {
      offenceStatus = status;
      switch (status) {
        case 'New':
          offenceStatusDesc = 'Dispute created';
          break;
        case 'Submitted':
          offenceStatusDesc = 'Dispute submitted';
          break;
        case 'InProgress':
          offenceStatusDesc = 'In progress';
          break;
        case 'Complete':
          offenceStatusDesc = 'Resolved';
          break;
        case 'Rejected':
          offenceStatusDesc = 'Rejected';
          break;
        default:
          offenceStatus = 'Unknown-' + status;
          offenceStatusDesc = 'Unknown dispute status';
          break;
      }
    } else {
      if (amountDue > 0) {
        offenceStatus = 'Owe';
        offenceStatusDesc = 'Balance outstanding';
      } else {
        offenceStatus = 'Paid';
        offenceStatusDesc = 'Paid';
      }
    }

    return { offenceStatus, offenceStatusDesc };
  }

  /**
   * @description
   * return true if the date parameter is within 30 days
   */
  private isWithin30Days(discountDueDate: string): boolean {
    let isWithin = false;

    if (discountDueDate) {
      const today = new Date();

      const diff = Math.floor(
        (Date.parse(discountDueDate) - Date.parse(today.toDateString())) /
        86400000
      );

      isWithin = diff >= 0 && diff <= 30;
    }

    return isWithin;
  }

  /**
   * @description
   * correctly add the currency.
   * javascript may not add correctly so convert to whole numbers first then add.
   */
  private addMoney(a: number, b: number): number {
    return (a * 100 + b * 100) / 100;
  }

  /**
   * @description
   * populate the offence object with the calculated information
   */
  private updateTicketViewModel(ticket: TicketDisputeView): void {
    let balance = 0;
    let total = 0;
    let requestSubmitted = false;
    let first = true;
    let courtRequired = false;
    let reductionRequired = false;
    let isReductionNotInCourt = false;

    ticket.offences.forEach((offence) => {
      offence._firstOffence = first;
      if (first) {
        first = false;
      }

      offence._within30days = this.isWithin30Days(ticket.discountDueDate);
      offence._amountDue = offence.amountDue;

      if (offence._within30days) {
        offence._amountDue =
          offence.amountDue >= offence.discountAmount
            ? offence.amountDue - offence.discountAmount
            : 0;
      }

      const { offenceStatus, offenceStatusDesc } = this.getOffenceStatusDesc(
        offence.status,
        offence.offenceAgreementStatus,
        offence._amountDue
      );

      offence._offenceStatus = offenceStatus;
      offence._offenceStatusDesc = offenceStatusDesc;

      if (offence.offenceAgreementStatus) {
        requestSubmitted = true;

        if (offence.offenceAgreementStatus === 'DISPUTE') {
          courtRequired = true;
        } else if (offence.offenceAgreementStatus === 'REDUCTION') {
          reductionRequired = true;
          if (offence.reductionAppearInCourt) {
            courtRequired = true;
          } else {
            isReductionNotInCourt = true;
          }
        }
      }

      balance = this.addMoney(offence._amountDue, balance);
      total = this.addMoney(offence.amountDue, total);
    });

    // ------------------------------------
    // if the total due is 0, do not show the 'within 30 days' information
    ticket._within30days =
      total > 0 ? this.isWithin30Days(ticket.discountDueDate) : false;
    ticket._outstandingBalanceDue = balance;
    ticket._totalBalanceDue = total;
    ticket._requestSubmitted = requestSubmitted;

    if (ticket.additional) {
      ticket.additional._isCourtRequired = courtRequired;
      ticket.additional._isReductionRequired = reductionRequired;
      ticket.additional._isReductionNotInCourt = isReductionNotInCourt;
    }

    const allowApplyToAllCounts = ((!requestSubmitted) && (ticket.offences.length > 1));
    ticket.offences.forEach((offence) => {
      offence._allowApplyToAllCounts = allowApplyToAllCounts;
    });
  }

  /**
   * @description
   * strip the calculated fields from the object
   */
  private cleanTicketDispute(ticket: TicketDisputeView): TicketDispute {
    const ticketDispute = { ...ticket };
    for (const property in ticket) {
      if (property.charAt(0) === '_') {
        delete ticketDispute[property];
      }
    }
    return ticketDispute;
  }

  /**
   * @description
   * strip the calculated fields from the object
   */
  private cleanShellTicket(ticket: ShellTicketView): ShellTicket {
    const shellTicket = { ...ticket };

    // cleanup payload data
    if (shellTicket._chargeCount < 3) {
      shellTicket._count3ChargeDesc = null;
      shellTicket._count3ChargeSection = null;
      shellTicket.count3Charge = null;
      shellTicket.count3FineAmount = null;
    }

    // cleanup payload data
    if (shellTicket._chargeCount < 2) {
      shellTicket._count2ChargeDesc = null;
      shellTicket._count2ChargeSection = null;
      shellTicket.count2Charge = null;
      shellTicket.count2FineAmount = null;
    }

    for (const property in ticket) {
      if (property.charAt(0) === '_') {
        delete shellTicket[property];
      }
    }
    return shellTicket;
  }
}
