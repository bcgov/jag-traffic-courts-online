import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiHttpResponse } from '@core/models/api-http-response.model';
import { ApiResource } from '@core/resources/api-resource.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { Offence } from '@shared/models/offence.model';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { CountDispute } from '@shared/models/countDispute.model';
import { ConfigService } from '@config/config.service';
import { TcoTicketDispute } from '@shared/models/tcoTicketDispute.model';

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
  }): Observable<TcoTicketDispute> {
    const httpParams = new HttpParams({ fromObject: params });

    return this.apiResource.get<TcoTicketDispute>('tickets', httpParams).pipe(
      map((response: ApiHttpResponse<TcoTicketDispute>) =>
        response ? response.result : null
      ),
      tap((ticket: TcoTicketDispute) =>
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
    ticketDispute: TcoTicketDispute
  ): Observable<TcoTicketDispute> {
    this.logger.info('createTicketDispute', ticketDispute);

    return this.apiResource
      .post<TcoTicketDispute>('disputes/createTicketDispute', ticketDispute)
      .pipe(
        map((response: ApiHttpResponse<TcoTicketDispute>) => null),
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
  public createCountDispute(
    countDispute: CountDispute
  ): Observable<TcoTicketDispute> {
    this.logger.info('createCountDispute', countDispute);

    return this.apiResource
      .post<TcoTicketDispute>('disputes', countDispute)
      .pipe(
        map((response: ApiHttpResponse<TcoTicketDispute>) => null),
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
    const disputeStatus = row.offenceDisputeDetail
      ? row.offenceDisputeDetail.status
      : null;
    const status = disputeStatus ? disputeStatus : row.amountDue > 0 ? -1 : -2;
    return status;
  }

  /**
   * populate the offence object with the calculated information
   */
  private setOffenceInfo(ticket: TcoTicketDispute): void {
    let balance = 0;
    let disputesExist = false;
    ticket.offences.forEach((offence) => {
      offence.offenceStatus = this.getOffenceInfo(offence);

      if (offence.offenceDisputeDetail) {
        disputesExist = true;
      }

      balance += offence.amountDue;
    });

    // ------------------------------------
    ticket.outstandingBalance = balance;
    ticket.disputesExist = disputesExist;
  }
}
