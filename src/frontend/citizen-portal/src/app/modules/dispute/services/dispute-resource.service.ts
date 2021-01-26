import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoggerService } from '@core/services/logger.service';
import { Ticket } from '@shared/models/ticket.model';
import { Observable } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class DisputeResourceService {

  constructor(private httpClient: HttpClient,
    private logger: LoggerService) {}

  public ticket(): Observable<Ticket> {
    return this.httpClient.get<any>("http://localhost:4200/ticket")
      .pipe(
        catchError((error: any) => {
          this.logger.error('[Dispute] DisputeResource::ticket error has occurred: ', error);
          throw error;
        })
      );
  }
}
