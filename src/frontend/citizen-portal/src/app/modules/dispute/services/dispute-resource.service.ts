import { Injectable } from '@angular/core';
import { ApiHttpResponse } from '@core/models/api-http-response.model';
import { ApiResource } from '@core/resources/api-resource.service';
import { LoggerService } from '@core/services/logger.service';
import { Ticket } from '@shared/models/ticket.model';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class DisputeResourceService {
  constructor(
    private apiResource: ApiResource,
    private logger: LoggerService
  ) {}

  public getTicket(): Observable<Ticket> {
    return this.apiResource.get<Ticket>('ticket').pipe(
      map((response: ApiHttpResponse<Ticket>) => response.result),
      catchError((error: any) => {
        this.logger.error(
          '[GetTicket] DisputeResourceService::ticket error has occurred: ',
          error
        );
        throw error;
      })
    );
  }

  public createTicket(ticket: Ticket): Observable<Ticket> {
    this.logger.info('createTicket', ticket);

    return this.apiResource.post<Ticket>('ticket').pipe(
      map((response: ApiHttpResponse<Ticket>) => response.result),
      catchError((error: any) => {
        this.logger.error(
          '[CreateTicket] DisputeResourceService::ticket error has occurred: ',
          error
        );
        throw error;
      })
    );
  }

  public updateTicket(ticket: Ticket): Observable<Ticket> {
    this.logger.info('updateTicket', ticket);

    return this.apiResource.put<Ticket>('ticket').pipe(
      map((response: ApiHttpResponse<Ticket>) => response.result),
      catchError((error: any) => {
        this.logger.error(
          '[UpdateTicket] DisputeResourceService::ticket error has occurred: ',
          error
        );
        throw error;
      })
    );
  }
}
