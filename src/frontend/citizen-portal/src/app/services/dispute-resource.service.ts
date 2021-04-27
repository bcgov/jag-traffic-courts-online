import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiHttpResponse } from '@core/models/api-http-response.model';
import { ApiResource } from '@core/resources/api-resource.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { Ticket } from '@shared/models/ticket.model';
import { Offence } from '@shared/models/offence.model';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { TicketDispute } from '@shared/models/ticketDispute.model';
import { CountDispute } from '@shared/models/countDispute.model';
import { ConfigService } from '@config/config.service';

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
  public createTicketDispute(ticketDispute: TicketDispute): Observable<Ticket> {
    this.logger.info('createTicketDispute', ticketDispute);

    return this.apiResource.post<Ticket>('disputes', ticketDispute).pipe(
      map((response: ApiHttpResponse<Ticket>) => null),
      catchError((error: any) => {
        this.toastService.openErrorToast(
          this.configService.dispute_create_error
        );
        this.logger.error(
          'DisputeResourceService::createTicketDispute error has occurred: ',
          error
        );
        throw error;
      })
    );
  }

  /**
   * Create a count dispute
   */
  public createCountDispute(countDispute: CountDispute): Observable<Ticket> {
    this.logger.info('createCountDispute', countDispute);

    return this.apiResource.post<Ticket>('disputes', countDispute).pipe(
      map((response: ApiHttpResponse<Ticket>) => null),
      catchError((error: any) => {
        this.toastService.openErrorToast(
          this.configService.dispute_create_error
        );
        this.logger.error(
          'DisputeResourceService::createCountDispute error has occurred: ',
          error
        );
        throw error;
      })
    );
  }

  private getOffenceInfo(row: Offence): number {
    const disputeStatus = row.offenceDispute ? row.offenceDispute.status : null;
    const status = disputeStatus ? disputeStatus : row.amountDue > 0 ? -1 : -2;
    return status;
  }

  /**
   * populate the offence object with the calculated information
   */
  private setOffenceInfo(ticket: Ticket): void {
    let balance = 0;
    let disputesExist = false;
    ticket.offences.forEach((offence) => {
      offence.offenceStatus = this.getOffenceInfo(offence);

      if (offence.offenceDispute) {
        disputesExist = true;
      }

      balance += offence.amountDue;
    });

    // ------------------------------------
    ticket.outstandingBalance = balance;
    ticket.disputesExist = disputesExist;
  }
}
