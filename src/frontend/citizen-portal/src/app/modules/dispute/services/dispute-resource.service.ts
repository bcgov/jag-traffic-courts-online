import { Injectable } from '@angular/core';
import { ApiHttpResponse } from '@core/models/api-http-response.model';
import { ApiResource } from '@core/resources/api-resource.service';
import { LoggerService } from '@core/services/logger.service';
import { Dispute } from '@shared/models/dispute.model';
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
          '[getTicket] DisputeResourceService::ticket error has occurred: ',
          error
        );
        throw error;
      })
    );
  }

  public getTickets(): Observable<Ticket[]> {
    return this.apiResource.get<Ticket[]>('tickets').pipe(
      map((response: ApiHttpResponse<Ticket[]>) => response.result),
      catchError((error: any) => {
        this.logger.error(
          '[getTickets] DisputeResourceService::tickets error has occurred: ',
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

  public getDisputes(): Observable<Dispute[]> {
    return this.apiResource.get<Dispute[]>('disputes').pipe(
      map((response: ApiHttpResponse<Dispute[]>) => response.result),
      catchError((error: any) => {
        this.logger.error(
          '[getDisputes] DisputeResourceService::disputes error has occurred: ',
          error
        );
        throw error;
      })
    );
  }

  public createDispute(dispute: Dispute): Observable<Dispute> {
    this.logger.info('createDispute', dispute);

    return this.apiResource.post<Dispute>('ticket').pipe(
      map((response: ApiHttpResponse<Dispute>) => response.result),
      catchError((error: any) => {
        this.logger.error(
          '[createDispute] DisputeResourceService::dispute error has occurred: ',
          error
        );
        throw error;
      })
    );
  }

  public updateDispute(dispute: Dispute): Observable<Dispute> {
    this.logger.info('updateDispute', dispute);

    return this.apiResource.put<Dispute>('ticket').pipe(
      map((response: ApiHttpResponse<Dispute>) => response.result),
      catchError((error: any) => {
        this.logger.error(
          '[updateDispute] DisputeResourceService::dispute error has occurred: ',
          error
        );
        throw error;
      })
    );
  }
}
