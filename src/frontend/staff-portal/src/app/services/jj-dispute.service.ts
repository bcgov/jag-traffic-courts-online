import { ConfigService } from '@config/config.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { Observable, BehaviorSubject, forkJoin, Subscription } from 'rxjs';
import { catchError, map, mergeMap } from 'rxjs/operators';
import { EventEmitter, Injectable } from '@angular/core';
import { JJService, JJDispute as JJDisputeBase, JJDisputeStatus, JJDisputeRemark, JJDisputeCourtAppearanceRoP } from 'app/api';
import { AuthService, UserRepresentation } from './auth.service';
import { cloneDeep } from "lodash";
import { AppState } from 'app/store';
import { Store } from '@ngrx/store';
import * as JJDisputeStore from 'app/store/jj-dispute';

@Injectable({
  providedIn: 'root',
})
export class JJDisputeService {
  private _jjList: BehaviorSubject<UserRepresentation[]> = new BehaviorSubject<UserRepresentation[]>([]);
  private _vtcList: BehaviorSubject<UserRepresentation[]> = new BehaviorSubject<UserRepresentation[]>([]);
  public refreshDisputes: EventEmitter<any> = new EventEmitter();

  public jjDisputeStatusesSorted: JJDisputeStatus[] = [JJDisputeStatus.New, JJDisputeStatus.HearingScheduled, JJDisputeStatus.Review, JJDisputeStatus.InProgress, JJDisputeStatus.Confirmed, JJDisputeStatus.RequireCourtHearing, JJDisputeStatus.RequireMoreInfo, JJDisputeStatus.DataUpdate, JJDisputeStatus.Accepted];
  public jjDisputeStatusEditable: JJDisputeStatus[] = [JJDisputeStatus.New, JJDisputeStatus.Review, JJDisputeStatus.InProgress, JJDisputeStatus.HearingScheduled];
  public jjDisputeStatusComplete: JJDisputeStatus[] = [JJDisputeStatus.Confirmed, JJDisputeStatus.RequireCourtHearing, JJDisputeStatus.RequireMoreInfo];
  public jjDisputeStatusDisplay: JJDisputeStatus[] = [JJDisputeStatus.New, JJDisputeStatus.HearingScheduled, JJDisputeStatus.InProgress, JJDisputeStatus.Review, JJDisputeStatus.RequireMoreInfo];

  constructor(
    private toastService: ToastService,
    private logger: LoggerService,
    private configService: ConfigService,
    private jjApiService: JJService,
    private authService: AuthService,
    private store: Store<AppState>
  ) {
    let observables = {
      jjList: this.authService.getUsersInGroup("judicial-justice"),
      vtcList: this.authService.getUsersInGroup("vtc-staff"),
    };
    this.authService.isLoggedIn$.subscribe(isLoggedIn => {
      if (isLoggedIn) {
        forkJoin(observables).subscribe({
          next: results => {
            this._jjList.next(results.jjList
              .map(u => {
                u.jjDisplayName = u.fullName ? "JJ " + u.fullName : "";
                return u;
              })
              .sort((a, b) => {
                if (a.fullName < b.fullName) { return -1; }
                else { return 1 }
              }));
            this._vtcList.next(results.vtcList);
            this.store.dispatch(JJDisputeStore.Actions.Get());
          },
          error: err => {
            this.logger.error("JJDisputeService: Load jjList and vtcList failed")
          }
        });
      }
    });
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
          response.map(jJDispute => this.toDisplay(jJDispute));
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
          this.store.dispatch(JJDisputeStore.Actions.Get());
          return response;
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

  /**
     * Require a court hearing
     *
     * @param ticketNumber, jjDispute
     */
   public apiJjRequireCourtHearingPut(ticketNumber: string, remarks?: string): Observable<JJDispute> {
    return this.jjApiService.apiJjTicketNumberRequirecourthearingPut(ticketNumber, remarks)
      .pipe(
        map((response: any) => {
          this.logger.info('jj-DisputeService::apiJjRequireCourtHearingPut', response)
          this.store.dispatch(JJDisputeStore.Actions.Get());
          return response;
        }),
        catchError((error: any) => {
          var errorMsg = error?.error?.detail != null ? error.error.detail : this.configService.dispute_error;
          this.toastService.openErrorToast(errorMsg);
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error(
            'jj-DisputeService::apiJjRequireCourtHearingPut error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  public apiJjTicketNumberReviewPut(ticketNumber: string, checkVTC: boolean, remarks?: string): Observable<any> {
    return this.jjApiService.apiJjTicketNumberReviewPut(ticketNumber, checkVTC, remarks)
      .pipe(
        map((response: any) => {
          this.logger.info('jj-DisputeService::apiJjTicketNumberReviewPut', response)
          this.store.dispatch(JJDisputeStore.Actions.Get());
          return response;
        }),
        catchError((error: any) => {
          var errorMsg = error?.error?.detail != null ? error.error.detail : this.configService.dispute_error;
          this.toastService.openErrorToast(errorMsg);
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error(
            'jj-DisputeService::apiJjTicketNumberReviewPut error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  public apiJjAssignPut(ticketNumbers: Array<string>, username: string): Observable<any> {
    return this.jjApiService.apiJjAssignPut(ticketNumbers, username)
      .pipe(
        map((response: any) => {
          this.logger.info('jj-DisputeService::apiJjAssignPut', response)
          this.store.dispatch(JJDisputeStore.Actions.Get());
          return response;
        }),
        catchError((error: any) => {
          var errorMsg = error?.error?.detail != null ? error.error.detail : this.configService.dispute_error;
          this.toastService.openErrorToast(errorMsg);
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error(
            'jj-DisputeService::apiJjAssignPut error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  public apiJjTicketNumberAcceptPut(ticketNumber: string, checkVTC: boolean): Observable<any> {
    return this.jjApiService.apiJjTicketNumberAcceptPut(ticketNumber, checkVTC)
      .pipe(
        map((response: any) => {
          this.logger.info('jj-DisputeService::apiJjTicketNumberAcceptPut', response)
          this.store.dispatch(JJDisputeStore.Actions.Get());
          return response;
        }),
        catchError((error: any) => {
          var errorMsg = error?.error?.detail != null ? error.error.detail : this.configService.dispute_error;
          this.toastService.openErrorToast(errorMsg);
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error(
            'jj-DisputeService::apiJjTicketNumberAcceptPut error has occurred: ',
            error
          );
          throw error;
        })
      );
  }


  public apiJjTicketNumberConfirmPut(ticketNumber: string): Observable<any> {
    return this.jjApiService.apiJjTicketNumberUpdatecourtappearanceConfirmPut(ticketNumber)
      .pipe(
        map((response: any) => {
          this.logger.info('jj-DisputeService::apiJjTicketNumberConfirmPut', response)
          this.store.dispatch(JJDisputeStore.Actions.Get());
          return response;
        }),
        catchError((error: any) => {
          var errorMsg = error?.error?.detail != null ? error.error.detail : this.configService.dispute_error;
          this.toastService.openErrorToast(errorMsg);
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error(
            'jj-DisputeService::apiJjTicketNumberConfirmPut error has occurred: ',
            error
          );
          throw error;
        })
      );
  }
  public get jjList$(): Observable<UserRepresentation[]> {
    return this._jjList.asObservable();
  }

  public get jjList(): UserRepresentation[] {
    return this._jjList.value;
  }

  public get vtcList$(): Observable<UserRepresentation[]> {
    return this._vtcList.asObservable();
  }

  public get vtcList(): UserRepresentation[] {
    return this._vtcList.value;
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

  public addRemarks(jJDispute: JJDispute, remarksText: string): JJDispute {
    if (!jJDispute.remarks) {
      jJDispute.remarks = [];
    }
    let remarks: JJDisputeRemark = {
      userFullName: this.authService.userProfile.fullName,
      note: remarksText
    }
    jJDispute.remarks.push(remarks);
    return jJDispute;
  }

  private toDisplay(jjDispute: JJDispute): JJDispute {
    jjDispute.fullName = jjDispute.surname + ", " + (jjDispute.contactGivenName1 + jjDispute.contactGivenName2 ? " " + jjDispute.contactGivenName2 : "") + (jjDispute.contactGivenName3 ? " " + jjDispute.contactGivenName3 : "");
    jjDispute.isEditable = this.jjDisputeStatusEditable.indexOf(jjDispute.status) > -1;
    jjDispute.isCompleted = this.jjDisputeStatusComplete.indexOf(jjDispute.status) > -1;
    jjDispute.bulkAssign = false;
    jjDispute.jjAssignedToName = this.jjList?.filter(y => y.idir === jjDispute.jjAssignedTo?.toUpperCase())[0]?.fullName;
    jjDispute.vtcAssignedToName = this.vtcList?.filter(y => y.idir === jjDispute.vtcAssignedTo?.toUpperCase())[0]?.fullName;

    if (jjDispute.jjDisputeCourtAppearanceRoPs?.length > 0) {
      let mostRecentCourtAppearance = jjDispute.jjDisputeCourtAppearanceRoPs.sort((a, b) => { if (a.appearanceTs > b.appearanceTs) { return -1; } else { return 1 } })[0];
      jjDispute.room = mostRecentCourtAppearance.room;
      jjDispute.duration = mostRecentCourtAppearance.duration;
      jjDispute.appearanceTs = new Date(mostRecentCourtAppearance.appearanceTs);
    }

    return jjDispute;
  }
}

export interface JJDispute extends JJDisputeBase {
  vtcAssignedToName?: string;
  jjAssignedToName?: string;
  bulkAssign?: boolean;
  appearanceTs?: Date;
  duration?: number;
  room?: string;
  isEditable?: boolean;
  isCompleted?: boolean;
  fullName?: string;
}
