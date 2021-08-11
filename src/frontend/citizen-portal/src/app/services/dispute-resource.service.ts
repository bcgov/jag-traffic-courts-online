import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ConfigService } from '@config/config.service';
import { ApiHttpResponse } from '@core/models/api-http-response.model';
import { ApiResource } from '@core/resources/api-resource.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { Offence } from '@shared/models/offence.model';
import { ShellTicket } from '@shared/models/shellTicket.model';
import { TicketDispute } from '@shared/models/ticketDispute.model';
import { Observable } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class DisputeResourceService {
  constructor(
    private apiResource: ApiResource,
    private toastService: ToastService,
    private logger: LoggerService,
    private configService: ConfigService
  ) {}

  /**
   * Get the ticket from RSI.
   *
   * @param params containing the ticketNumber and time
   */
  public getTicket(params: {
    ticketNumber: string;
    time: string;
  }): Observable<TicketDispute> {
    const httpParams = new HttpParams({ fromObject: params });

    return this.apiResource
      .get<TicketDispute>('tickets/ticket', httpParams)
      .pipe(
        map((response: ApiHttpResponse<TicketDispute>) =>
          response ? response.result : null
        ),
        tap((ticket: TicketDispute) =>
          this.logger.info('DisputeResourceService::getTicket', ticket)
        ),
        map((ticket) => {
          if (ticket) {
            this.setOffenceInfo(ticket);
          }
          return ticket;
        }),
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
   * Create the ticket dispute
   *
   * @param dispute The dispute to be created
   */
  public createTicketDispute(
    ticketDispute: TicketDispute
  ): Observable<TicketDispute> {
    this.logger.info(
      'DisputeResourceService::createTicketDispute',
      ticketDispute
    );

    return this.apiResource
      .post<TicketDispute>('disputes/ticketDispute', ticketDispute)
      .pipe(
        map((response: ApiHttpResponse<TicketDispute>) =>
          response ? response.result : null
        ),
        tap((newDisputeTicket: TicketDispute) => {
          this.setOffenceInfo(newDisputeTicket);

          this.toastService.openSuccessToast(
            'The request has been successfully submitted'
          );

          this.logger.info(
            'DisputeResourceService::NEW_DISPUTE_TICKET',
            newDisputeTicket
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
  public createShellTicket(ticket: ShellTicket): Observable<TicketDispute> {
    this.logger.info('DisputeResourceService::createShellTicket', ticket);

    return this.apiResource
      .post<TicketDispute>('tickets/shellTicket', ticket)
      .pipe(
        map((response: ApiHttpResponse<TicketDispute>) =>
          response ? response.result : null
        ),
        tap((newShellTicket: TicketDispute) => {
          this.setOffenceInfo(newShellTicket);

          this.toastService.openSuccessToast(
            'The ticket has been successfully created'
          );

          this.logger.info(
            'DisputeResourceService:: NEW_SHELL_TICKET',
            newShellTicket
          );
        }),
        map((shellTicket) => {
          return shellTicket;
        }),
        catchError((error: any) => {
          this.toastService.openErrorToast('Ticket could not be created');
          this.logger.error(
            'DisputeResourceService::createShellTicket error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  private getOffenceStatusDesc(
    status: number,
    offenceAgreementStatus: string,
    amountDue: number
  ): string {
    let desc = '';

    if (
      offenceAgreementStatus &&
      (offenceAgreementStatus === 'DISPUTE' ||
        offenceAgreementStatus === 'REDUCTION')
    ) {
      switch (status) {
        case 0:
          desc = 'Dispute created';
          break;
        case 1:
          desc = 'Dispute submitted';
          break;
        case 2:
          desc = 'In progress';
          break;
        case 3:
          desc = 'Resolved';
          break;
        case 4:
          desc = 'Rejected';
          break;
        default:
          desc = 'Unknown dispute status';
          break;
      }
    } else {
      if (amountDue > 0) {
        desc = 'Balance outstanding';
      } else {
        desc = 'Paid';
      }
    }

    return desc;
  }

  // private getAgreementStatusDesc(
  //   status: string,
  //   requestReduction: boolean,
  //   requestMoreTime: boolean
  // ): string {
  //   let desc = 'Unknown status: ' + status;

  //   switch (status) {
  //     case 'NOTHING':
  //       desc = 'No action at this time';
  //       break;
  //     case 'PAY':
  //       desc = 'Pay for this count';
  //       break;
  //     case 'REDUCTION':
  //       if (requestReduction && requestMoreTime) {
  //         desc = 'Request a fine reduction and more time to pay';
  //       } else if (requestReduction) {
  //         desc = 'Request a fine reduction';
  //       } else {
  //         desc = 'Request more time to pay';
  //       }
  //       break;
  //     case 'DISPUTE':
  //       desc = 'Dispute the charge';
  //       break;
  //   }
  //   return desc;
  // }

  /**
   * populate the offence object with the calculated information
   */
  private setOffenceInfo(ticket: TicketDispute): void {
    let balance = 0;
    let total = 0;
    let requestSubmitted = false;

    ticket.offences.forEach((offence) => {
      offence.offenceStatusDesc = this.getOffenceStatusDesc(
        offence.status,
        offence.offenceAgreementStatus,
        offence.amountDue
      );

      // offence.offenceAgreementStatusDesc = this.getAgreementStatusDesc(
      //   offence.offenceAgreementStatus,
      //   offence.requestReduction,
      //   offence.requestMoreTime
      // );

      if (offence.offenceAgreementStatus) {
        requestSubmitted = true;
      }

      balance += offence.amountDue;
      total += offence.ticketedAmount;
    });

    // ------------------------------------
    ticket.outstandingBalanceDue = balance;
    ticket.totalBalanceDue = total;
    ticket.requestSubmitted = requestSubmitted;
  }
}
