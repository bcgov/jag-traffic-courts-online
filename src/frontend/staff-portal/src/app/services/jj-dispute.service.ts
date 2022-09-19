import { ConfigService } from '@config/config.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { Observable, BehaviorSubject, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { EventEmitter, Injectable } from '@angular/core';
import { JJService, JJDispute, JJDisputeStatus, JJDisputeRemark } from 'app/api';
import { AuthService } from './auth.service';
import { cloneDeep } from 'lodash';

@Injectable({
  providedIn: 'root',
})
export class JJDisputeService {
  private _JJDisputes: BehaviorSubject<JJDispute[]> = new BehaviorSubject<JJDispute[]>(null);
  private _JJDispute: BehaviorSubject<JJDispute> = new BehaviorSubject<JJDispute>(null);
  public jjDisputeStatusesSorted: JJDisputeStatus[] = [JJDisputeStatus.New, JJDisputeStatus.Review, JJDisputeStatus.InProgress, JJDisputeStatus.Confirmed, JJDisputeStatus.RequireCourtHearing, JJDisputeStatus.RequireMoreInfo, JJDisputeStatus.DataUpdate, JJDisputeStatus.Accepted];
  public JJDisputeStatusEditable: JJDisputeStatus[] = [JJDisputeStatus.New, JJDisputeStatus.Review, JJDisputeStatus.InProgress];
  public JJDisputeStatusComplete: JJDisputeStatus[] = [JJDisputeStatus.Confirmed, JJDisputeStatus.RequireCourtHearing, JJDisputeStatus.RequireMoreInfo];
  public refreshDisputes: EventEmitter<any> = new EventEmitter();

  // TODO dynamically get list of JJs
  public jjList: JJTeamMember[] = [
    { idir: "ldame@idir", name: "Lorraine Dame" },
    { idir: "philbold@idir", name: "Phil Bolduc" },
    { idir: "choban@idir", name: "Chris Hoban" },
    { idir: "kneufeld@idir", name: "Kevin Neufeld" },
    { idir: "cohiggins@idir", name: "Colm O'Higgins" },
    { idir: "bkarahan@idir", name: "Burak Karahan" },
    { idir: "twwong@idir", name: "Tsunwai Wong" },
    { idir: "ewong@idir", name: "Elaine Wong" },
    { idir: "jmoffet@idir", name: "Jeffrey Moffet" },
    { idir: "rspress@idir", name: "Roberta Press" },
  ];

  constructor(
    private toastService: ToastService,
    private logger: LoggerService,
    private configService: ConfigService,
    private jjApiService: JJService,
    private authService: AuthService
  ) {
  }

  /**
     * Get the JJ disputes from RSI
     *
     * @param none
     */
  public getJJDisputes(): Observable<JJDispute[]> {
    return this.jjApiService.apiJjDisputesGet()
      .pipe(
        map((response: JJDispute[]) => {
          this.logger.info('jj-DisputeService::getJJDisputes', response);
          this._JJDisputes.next(response);
          return response;
        }),
        catchError((error: any) => {
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error(
            'jj-DisputeService::getJJDisputes error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  /**
   * Get the JJ disputes from RSI by IDIR
   *
   * @param none
   */
  public getJJDisputesByIDIR(idir: string): Observable<JJDispute[]> {
    return this.jjApiService.apiJjDisputesGet(idir)
      .pipe(
        map((response: JJDispute[]) => {
          this.logger.info('jj-DisputeService::getJJDisputes', response);
          this._JJDisputes.next(response);
          return response;
        }),
        catchError((error: any) => {
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error(
            'jj-DisputeService::getJJDisputes error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  /**
     * Put the JJ dispute to RSI by Id.
     *
     * @param ticketNumber, jjDispute
     */
  public putJJDispute(ticketNumber: string, jjDispute: JJDispute, checkVTC: boolean, remarks?: string): Observable<JJDispute> {
    let input = cloneDeep(jjDispute);
    if (remarks) {
      this.addRemarks(input, remarks);
    }
    return this.jjApiService.apiJjTicketNumberPut(ticketNumber, checkVTC, input)
      .pipe(
        map((response: any) => {
          this.logger.info('jj-DisputeService::putJJDispute', response)
          return response ? response : null
        }),
        catchError((error: any) => {
          var errorMsg = error?.error?.detail != null ? error.error.detail : this.configService.dispute_error;
          this.toastService.openErrorToast(errorMsg);
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error(
            'jj-DisputeService::putJJDispute error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  public get JJDisputes$(): Observable<JJDispute[]> {
    return this._JJDisputes.asObservable();
  }

  public get JJDisputes(): JJDispute[] {
    return this._JJDisputes.value;
  }

  /**
   * Get the dispute from RSI by Id.
   *
   * @param disputeId
   */
  public getJJDispute(disputeId: string, assignVTC: boolean): Observable<JJDispute> {
    return this.jjApiService.apiJjJJDisputeIdGet(disputeId, assignVTC)
      .pipe(
        map((response: JJDispute) => {
          this.logger.info('jj-DisputeService::getJJDispute', response)
          return response ? response : null
        }),
        catchError((error: any) => {
          var errorMsg = error?.error?.detail != null ? error.error.detail : this.configService.dispute_error;
          this.toastService.openErrorToast(errorMsg);
          this.logger.error(
            'jj-DisputeService::getJJDispute error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  public get JJDispute$(): Observable<JJDispute> {
    return this._JJDispute.asObservable();
  }

  public get JJDispute(): JJDispute {
    return this._JJDispute.value;
  }

  public addRemarks(jJDispute: JJDispute, remarksText: string): JJDispute {
    if (!jJDispute.remarks) {
      jJDispute.remarks = [];
    }
    let remarks: JJDisputeRemark = {
      userFullName: this.authService.userFullName,
      note: remarksText
    }
    jJDispute.remarks.push(remarks);
    return jJDispute;
  }

  public addDays(initialDate: string, numDays: number): Date {
    var futureDate = new Date(initialDate);
    futureDate.setDate(futureDate.getDate() + numDays);
    return futureDate;
  }
}

export interface JJTeamMember { idir: string, name: string; }
