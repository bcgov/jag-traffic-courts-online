import { Injectable } from '@angular/core';
import { ApiHttpResponse } from '@core/models/api-http-response.model';
import { ApiResource } from '@core/resources/api-resource.service';
import { LoggerService } from '@core/services/logger.service';
import { TcoTicketDispute } from '@shared/models/tcoTicketDispute.model';
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

  public getTicket(): Observable<TcoTicketDispute> {
    return this.apiResource.get<TcoTicketDispute>('ticket').pipe(
      map((response: ApiHttpResponse<TcoTicketDispute>) => response.result),
      catchError((error: any) => {
        this.logger.error(
          '[getTicket] DisputeResourceService::ticket error has occurred: ',
          error
        );
        throw error;
      })
    );
  }

  public getDispute(): Observable<TcoTicketDispute> {
    return this.apiResource.get<TcoTicketDispute>('dispute').pipe(
      map((response: ApiHttpResponse<TcoTicketDispute>) => response.result),
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
