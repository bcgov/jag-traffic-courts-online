import { Injectable } from '@angular/core';
import { ApiHttpResponse } from '@core/models/api-http-response.model';
import { ApiResource } from '@core/resources/api-resource.service';
import { LoggerService } from '@core/services/logger.service';
import { environment } from '@env/environment';
import { Ticket } from '@shared/models/ticket.model';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class SurveyResourceService {
  private baseUrl = environment.apiUrl;

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

  public createDispute(ticket: Ticket): Observable<Ticket> {
    this.logger.info('createDispute', ticket);

    return this.apiResource.post<Ticket>('ticket').pipe(
      map((response: ApiHttpResponse<Ticket>) => response.result),
      catchError((error: any) => {
        this.logger.error(
          '[createDispute] DisputeResourceService::ticket error has occurred: ',
          error
        );
        throw error;
      })
    );
  }

  public updateDispute(ticket: Ticket): Observable<Ticket> {
    this.logger.info('updateDispute', ticket);

    return this.apiResource.put<Ticket>('ticket').pipe(
      map((response: ApiHttpResponse<Ticket>) => response.result),
      catchError((error: any) => {
        this.logger.error(
          '[updateDispute] DisputeResourceService::ticket error has occurred: ',
          error
        );
        throw error;
      })
    );
  }
}
