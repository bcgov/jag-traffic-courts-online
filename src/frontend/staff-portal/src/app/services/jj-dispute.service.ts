import { ConfigService } from '@config/config.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { Observable, BehaviorSubject, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { EventEmitter, Injectable } from '@angular/core';
import { JJService, JJDispute, JJDisputeStatus, JJDisputeRemark, UserRepresentation } from 'app/api';
import { AuthService } from './auth.service';
import { cloneDeep } from 'lodash';

@Injectable({
  providedIn: 'root',
})
export class JJDisputeService {
  private _JJDisputes: BehaviorSubject<JJDisputeView[]> = new BehaviorSubject<JJDisputeView[]>(null);
  private _JJDispute: BehaviorSubject<JJDisputeView> = new BehaviorSubject<JJDisputeView>(null);
  public jjDisputeStatusesSorted: JJDisputeStatus[] = [JJDisputeStatus.New, JJDisputeStatus.HearingScheduled, JJDisputeStatus.Review, JJDisputeStatus.InProgress, JJDisputeStatus.Confirmed, JJDisputeStatus.RequireCourtHearing, JJDisputeStatus.RequireMoreInfo, JJDisputeStatus.DataUpdate, JJDisputeStatus.Accepted];
  public JJDisputeStatusEditable: JJDisputeStatus[] = [JJDisputeStatus.New, JJDisputeStatus.Review, JJDisputeStatus.InProgress, JJDisputeStatus.HearingScheduled];
  public JJDisputeStatusComplete: JJDisputeStatus[] = [JJDisputeStatus.Confirmed, JJDisputeStatus.RequireCourtHearing, JJDisputeStatus.RequireMoreInfo];
  public refreshDisputes: EventEmitter<any> = new EventEmitter();
  public jjList: Array<UserRepresentation>;
  public vtcList: Array<UserRepresentation>;

  constructor(
    private toastService: ToastService,
    private logger: LoggerService,
    private configService: ConfigService,
    private jjApiService: JJService,
    private authService: AuthService
  ) {
    this.authService.getUsersInGroup("judicial-justice").subscribe(results => {
      this.jjList = results;
      this.jjList = this.jjList
        .sort((a: UserRepresentation, b: UserRepresentation) => {
          if (this.authService.getFullName(a) < this.authService.getFullName(b)) { return -1; } else { return 1 }
        })
    });
    this.authService.getUsersInGroup("vtc-staff").subscribe(results => {
      this.vtcList = results;
    });
  }

  /**
     * Get the JJ disputes from RSI
     *
     * @param none
     */
  public getJJDisputes(): Observable<JJDisputeView[]> {
    return this.jjApiService.apiJjDisputesGet()
      .pipe(
        map((response: JJDisputeView[]) => {
          this.logger.info('jj-DisputeService::getJJDisputes', response);
          response.map(jJDispute => this.toDisplay(jJDispute));
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
  public getJJDisputesByIDIR(idir: string): Observable<JJDisputeView[]> {
    return this.jjApiService.apiJjDisputesGet(idir)
      .pipe(
        map((response: JJDisputeView[]) => {
          this.logger.info('jj-DisputeService::getJJDisputes', response);
          response.map(jJDispute => this.toDisplay(jJDispute));
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
  public putJJDispute(ticketNumber: string, jjDispute: JJDisputeView, checkVTC: boolean, remarks?: string): Observable<JJDispute> {
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

  public get JJDisputes$(): Observable<JJDisputeView[]> {
    return this._JJDisputes.asObservable();
  }

  public get JJDisputes(): JJDisputeView[] {
    return this._JJDisputes.value;
  }

  /**
   * Get the dispute from RSI by Id.
   *
   * @param disputeId
   */
  public getJJDispute(disputeId: string, assignVTC: boolean): Observable<JJDisputeView> {
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

  public get JJDispute$(): Observable<JJDisputeView> {
    return this._JJDispute.asObservable();
  }

  public get JJDispute(): JJDisputeView {
    return this._JJDispute.value;
  }

  public addRemarks(jJDispute: JJDisputeView, remarksText: string): JJDisputeView {
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

  private toDisplay(jJDispute: JJDisputeView): JJDisputeView {
    jJDispute.isEditable = this.JJDisputeStatusEditable.indexOf(jJDispute.status) > -1;
    jJDispute.isCompleted = this.JJDisputeStatusComplete.indexOf(jJDispute.status) > -1;
    return jJDispute;
  }
}

export interface JJDisputeView extends JJDispute {
  jjAssignedToName?: string;
  bulkAssign?: boolean;
  appearanceTs?: Date;
  duration?: number;
  room?: string;
  isEditable?: boolean;
  isCompleted?: boolean;
}
