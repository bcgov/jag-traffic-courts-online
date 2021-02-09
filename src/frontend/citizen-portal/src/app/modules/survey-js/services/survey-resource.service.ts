import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoggerService } from '@core/services/logger.service';
import { environment } from '@env/environment';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';

@Injectable({
  providedIn: 'root',
})
export class SurveyResourceService {
  private baseUrl = environment.apiUrl;

  constructor(
    private httpClient: HttpClient,
    private mockDisputeService: MockDisputeService,
    private logger: LoggerService
  ) {}

  public ticket(): Observable<any> {
    return of(this.mockDisputeService.ticket);
  }

  public test(info: string): Observable<any> {
    if (info === 'AE11111111') {
      // return of({ success: false });

      return this.httpClient.get<any>(this.baseUrl + '/bad').pipe(
        catchError((error: any) => {
          this.logger.error(
            '[Survey] SurveyResourceService::error has occurred: ',
            error
          );
          throw error;
        })
      );
    }

    // return of({ success: true });
    return this.httpClient.get<any>(this.baseUrl + '/good').pipe(
      catchError((error: any) => {
        this.logger.error(
          '[Survey] SurveyResourceService::error has occurred: ',
          error
        );
        throw error;
      })
    );
  }
}
