import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiHttpResponse } from '@core/models/api-http-response.model';
import { ApiResource } from '@core/resources/api-resource.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { Dispute } from '@shared/models/dispute.model';
import { Ticket } from '@shared/models/ticket.model';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class DisputeResourceService {
  constructor(
    private apiResource: ApiResource,
    private toastService: ToastService,
    private logger: LoggerService
  ) {}

  public getTicket(params: {
    ticketNumber: string;
    time: string;
  }): Observable<Ticket> {
    const httpParams = new HttpParams({ fromObject: params });

    return this.apiResource.get<Ticket>('tickets', httpParams).pipe(
      map((response: ApiHttpResponse<Ticket>) =>
        response ? response.result : null
      ),
      tap((ticket: Ticket) =>
        this.logger.info('DisputeResourceService::getTicket', ticket)
      ),
      map((ticket) => {
        if (ticket) {
          this.setOffenceInfo(ticket);
        }
        return ticket;
      }),
      catchError((error: any) => {
        this.toastService.openErrorToast('Ticket could not be retrieved');
        this.logger.error(
          'DisputeResourceService::getTicket error has occurred: ',
          error
        );
        throw error;
      })
    );
  }

  public getAllTickets(): Observable<Ticket[]> {
    return this.apiResource.get<Ticket[]>('alltickets').pipe(
      map((response: ApiHttpResponse<Ticket[]>) => response.result),
      tap((tickets: Ticket[]) =>
        this.logger.info('DisputeResourceService::getAllTickets', tickets)
      ),
      catchError((error: any) => {
        this.toastService.openErrorToast('Tickets could not be retrieved');
        this.logger.error(
          'DisputeResourceService::getAllTickets error has occurred: ',
          error
        );
        throw error;
      })
    );
  }

  /**
   * Update the dispute
   */
  public updateDispute(dispute: Dispute): Observable<Dispute> {
    this.logger.info('updateDispute', dispute);

    return this.apiResource.put<Dispute>('ticket').pipe(
      map((response: ApiHttpResponse<Dispute>) => response.result),
      catchError((error: any) => {
        this.toastService.openErrorToast('Dispute could not be updated');
        this.logger.error(
          'DisputeResourceService::updateDispute error has occurred: ',
          error
        );
        throw error;
      })
    );
  }

  /**
   * populate the offence object with the calculated information
   */
  private setOffenceInfo(ticket: Ticket): void {
    let balance = 0;
    ticket.offences.forEach((offence) => {
      offence.earlyAmount = 0;
      offence.notes = '';

      if (offence.amountDue > 0) {
        const todayDate = new Date();
        const dueDate = new Date(offence.dueDate);

        if (todayDate <= dueDate) {
          offence.earlyAmount = offence.ticketAmount - 25;
          offence.amountDue = offence.earlyAmount;
        }
      }

      if (offence.amountDue === 0) {
        offence.statusCode = 'PAID';
        offence.statusDesc = 'Paid';
      } else if (offence.dispute) {
        offence.statusCode = 'DISPUTE';
        offence.statusDesc = 'Dispute Submitted';
        offence.notes =
          'The dispute has been filed. An email with the court information will be sent soon.';

        // offence.statusCode = 'COURT';
        // offence.statusDesc = 'Dispute In Progress';
        // offence.notes =
        //   'A court date has been set for this dispute. Check your email for more information.';

        // offence.statusCode = 'COMPLETE';
        // offence.statusDesc = 'Dispute Settled';
      } else {
        offence.statusCode = 'UNPAID';
        offence.statusDesc = 'Outstanding Balance';
      }

      balance +=
        offence.earlyAmount > 0 ? offence.earlyAmount : offence.amountDue;
    });

    // ------------------------------------
    ticket.outstandingBalance = balance;
  }
}
