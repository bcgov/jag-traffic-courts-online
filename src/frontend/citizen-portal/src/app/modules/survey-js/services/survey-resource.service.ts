import { Injectable } from '@angular/core';
import { ApiHttpResponse } from '@core/models/api-http-response.model';
import { ApiResource } from '@core/resources/api-resource.service';
import { LoggerService } from '@core/services/logger.service';
import { Dispute } from '@shared/models/dispute.model';
import { Ticket } from '@shared/models/ticket.model';
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

  public getTicket(): Observable<Ticket> {
    return this.apiResource.get<Ticket>('ticket').pipe(
      map((response: ApiHttpResponse<Ticket>) => response.result),
      catchError((error: any) => {
        this.logger.error(
          '[getTicket] DisputeResourceService::ticket error has occurred: ',
          error
        );
        throw error;
      })
    );
  }

  public getDispute(): Observable<Dispute> {
    return this.apiResource.get<Dispute>('dispute').pipe(
      map((response: ApiHttpResponse<Dispute>) => response.result),
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
