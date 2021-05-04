import { Injectable } from '@angular/core';
import { ApiHttpResponse } from '@core/models/api-http-response.model';
import { ApiResource } from '@core/resources/api-resource.service';
import { LoggerService } from '@core/services/logger.service';
import { TicketDispute } from '@shared/models/ticketDispute.model';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class SurveyResourceService {
  constructor(
    private apiResource: ApiResource,
    private logger: LoggerService
  ) {}

  public getTicket(): Observable<TicketDispute> {
    return this.apiResource.get<TicketDispute>('ticket').pipe(
      map((response: ApiHttpResponse<TicketDispute>) => response.result),
      catchError((error: any) => {
        this.logger.error(
          '[getTicket] DisputeResourceService::ticket error has occurred: ',
          error
        );
        throw error;
      })
    );
  }

  public getDispute(): Observable<TicketDispute> {
    return this.apiResource.get<TicketDispute>('dispute').pipe(
      map((response: ApiHttpResponse<TicketDispute>) => response.result),
      catchError((error: any) => {
        this.logger.error(
          '[getDispute] DisputeResourceService::dispute error has occurred: ',
          error
        );
        throw error;
      })
    );
  }
}
