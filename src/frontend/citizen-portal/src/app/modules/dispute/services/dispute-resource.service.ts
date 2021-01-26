import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoggerService } from '@core/services/logger.service';
import { environment } from '@env/environment';
import { Ticket } from '@shared/models/ticket.model';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class DisputeResourceService {
  private baseUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient, private logger: LoggerService) {}

  public ticket(): Observable<Ticket> {
    return this.httpClient.get<any>(this.baseUrl + '/ticket').pipe(
      catchError((error: any) => {
        this.logger.error(
          '[Dispute] DisputeResource::ticket error has occurred: ',
          error
        );
        throw error;
      })
    );
  }
}
