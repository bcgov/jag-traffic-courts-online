import { ConfigService } from '@config/config.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { HttpClient, HttpContext, HttpHeaders, HttpResponse } from '@angular/common/http';
import { EventEmitter, Injectable } from '@angular/core';
import { DatePipe } from '@angular/common';
import { JJService, JJDispute as JJDisputeBase, JJDisputeStatus, JJDisputeRemark, DocumentType } from 'app/api';
import { AuthService } from './auth.service';
import { cloneDeep } from "lodash";
import { AppState } from 'app/store';
import { Store } from '@ngrx/store';
import * as JJDisputeStore from 'app/store/jj-dispute';
import { LookupsService } from './lookups.service';

@Injectable({
  providedIn: 'root',
})
export class JJDisputeService {
  public refreshDisputes: EventEmitter<any> = new EventEmitter();
  public datepipe: DatePipe = new DatePipe('en-US');

  public jjDisputeStatusesSorted: JJDisputeStatus[] = [JJDisputeStatus.New, JJDisputeStatus.HearingScheduled, JJDisputeStatus.Review, JJDisputeStatus.InProgress, JJDisputeStatus.Confirmed, JJDisputeStatus.RequireCourtHearing, JJDisputeStatus.RequireMoreInfo, JJDisputeStatus.DataUpdate, JJDisputeStatus.Accepted];
  public jjDisputeStatusEditable: JJDisputeStatus[] = [JJDisputeStatus.New, JJDisputeStatus.Review, JJDisputeStatus.InProgress, JJDisputeStatus.HearingScheduled];
  public jjDisputeStatusComplete: JJDisputeStatus[] = [JJDisputeStatus.Confirmed, JJDisputeStatus.RequireCourtHearing, JJDisputeStatus.RequireMoreInfo];
  public jjDisputeStatusDisplay: JJDisputeStatus[] = [JJDisputeStatus.New, JJDisputeStatus.HearingScheduled, JJDisputeStatus.InProgress, JJDisputeStatus.Review, JJDisputeStatus.RequireMoreInfo];

  constructor(
    private toastService: ToastService,
    private logger: LoggerService,
    private configService: ConfigService,
    private jjApiService: JJService,
    private http: HttpClient,
    private authService: AuthService,
    private store: Store<AppState>,
    private lookupsService: LookupsService,
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
  public putJJDispute(ticketNumber: string, disputeId: number, jjDispute: JJDispute, checkVTC: boolean, remarks?: string): Observable<JJDispute> {
    let input = cloneDeep(jjDispute);
    if (remarks) {
      this.addRemarks(input, remarks);
    }
    console.log("just before put", input.jjDisputeCourtAppearanceRoPs[0].noAppTs);
    return this.jjApiService.apiJjTicketNumberPut(ticketNumber, disputeId, checkVTC, input)
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
  public apiJjRequireCourtHearingPut(ticketNumber: string, disputeId: number, remarks?: string): Observable<any> {
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

  public apiJjDisputeIdReviewPut(ticketNumber: string, checkVTC: boolean, remarks?: string): Observable<any> {
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

  // oracle To Date Format converts 2023-04-05T16:00:00+00:00 to MM/DD/YYYY HH:MM where using military 24hr
  // OR 2023-04-05 to 04/05/2023
  public toDateFormat(oracleDate: string): string {
    if (oracleDate.length < 10) return oracleDate;

    let formattedDate = oracleDate.substring(5,7) + "/" + oracleDate.substring(8,10) + "/" + oracleDate.substring(0,4);
    if (oracleDate.length == 11) {
      return formattedDate;
    } else if (oracleDate.length >= 16 ) {
      formattedDate = formattedDate + " " + oracleDate.substring(11,16);
      return formattedDate;
    } else return formattedDate;
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

  /**
   * Get the dispute from RSI by Id.
   *
   * @param disputeId
   */
  public getJJDispute(disputeId: number, ticketNumber: string, assignVTC: boolean): Observable<JJDispute> {
    return this.jjApiService.apiJjJjDisputeIdGet(disputeId, ticketNumber, assignVTC)
      .pipe(
        map((response: JJDispute) => {
          this.logger.info('jj-DisputeService::getJJDispute', response)
          return response ? this.toDisplay(response) : null
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

  public getFileBlob(fileId: string) {
    return this.http
      .get(`/api/document?fileId=${fileId}`, {
        observe: 'response',
        responseType: 'blob',
        context: new HttpContext(),
        withCredentials: true,
        headers: new HttpHeaders(
          {
            'Authorization': 'Bearer ' + this.authService.token,
            'Accept': '*/*',
            'Access-Control-Allow-Origin': ''
          }),
      }).pipe(
        map((result: HttpResponse<Blob>) => {
          return result.body;
        }));
  }

  public getJustinDocument(ticketNumber: string, documentType: DocumentType) {
    return this.http.get(`api/jj/ticketimage/${ticketNumber}/${documentType.toString()}`, {
      observe: 'response',
      responseType: 'blob',
      context: new HttpContext(),
      withCredentials: true,
      headers: new HttpHeaders(
        {
          'Authorization': 'Bearer ' + this.authService.token,
          'Accept': '*/*',
          'Access-Control-Allow-Origin': ''
        }),
    }).pipe(
      map((result: HttpResponse<Blob>) => {
        return result.body;
      }));
  }

  private toDisplay(jjDispute: JJDispute): JJDispute {
    jjDispute.contactName = jjDispute.contactSurname + (jjDispute.contactGivenName1 || jjDispute.contactGivenName2 || jjDispute.contactGivenName3 ? "," : "") + (jjDispute.contactGivenName1 ? " " + jjDispute.contactGivenName1 : "") + (jjDispute.contactGivenName2 ? " " + jjDispute.contactGivenName2 : "") + (jjDispute.contactGivenName3 ? " " + jjDispute.contactGivenName3 : "");
    jjDispute.contactGivenNames = jjDispute.contactGivenName1 + (jjDispute.contactGivenName2 ? " " + jjDispute.contactGivenName2 : "") + (jjDispute.contactGivenName3 ? " " + jjDispute.contactGivenName3 : "");
    jjDispute.occamDisputantName = jjDispute.occamDisputantSurnameNm + (jjDispute.occamDisputantGiven1Nm || jjDispute.occamDisputantGiven2Nm || jjDispute.occamDisputantGiven3Nm ? "," : "") + (jjDispute.occamDisputantGiven1Nm ? " " + jjDispute.occamDisputantGiven1Nm : "") + (jjDispute.occamDisputantGiven2Nm ? " " + jjDispute.occamDisputantGiven2Nm : "") + (jjDispute.occamDisputantGiven3Nm ? " " + jjDispute.occamDisputantGiven3Nm : "");
    jjDispute.occamDisputantGivenNames = jjDispute.occamDisputantGiven1Nm + (jjDispute.occamDisputantGiven2Nm ? " " + jjDispute.occamDisputantGiven2Nm : "") + (jjDispute.occamDisputantGiven3Nm ? " " + jjDispute.occamDisputantGiven3Nm : "");
    jjDispute.isEditable = this.jjDisputeStatusEditable.indexOf(jjDispute.status) > -1;
    jjDispute.isCompleted = this.jjDisputeStatusComplete.indexOf(jjDispute.status) > -1;
    jjDispute.bulkAssign = false;
    jjDispute.jjAssignedToName = this.authService.jjList?.filter(y => y.idir === jjDispute.jjAssignedTo?.toUpperCase())[0]?.fullName;
    if (jjDispute.jjAssignedTo?.trim() && !jjDispute.jjAssignedToName) jjDispute.jjAssignedToName = jjDispute.jjAssignedTo;
    jjDispute.vtcAssignedToName = this.authService.vtcList?.filter(y => y.idir === jjDispute.vtcAssignedTo?.toUpperCase())[0]?.fullName;
    if (jjDispute.vtcAssignedTo?.trim() && !jjDispute.vtcAssignedToName) jjDispute.vtcAssignedToName = jjDispute.vtcAssignedTo;
    jjDispute.address = jjDispute.addressLine1
      + (jjDispute.addressLine2 ? ", " + jjDispute.addressLine2 : "")
      + (jjDispute.addressLine3 ? ", " + jjDispute.addressLine3 : "")
      + (jjDispute.addressCity ? ", " + jjDispute.addressCity : "")
      + (jjDispute.addressProvince ? ", " + jjDispute.addressProvince : "")
      + (jjDispute.addressCountry ? ", " + jjDispute.addressCountry : "")
      + (jjDispute.addressPostalCode ? ", " + jjDispute.addressPostalCode : "")

    // lookup courthouse location
    if (jjDispute.courtAgenId && !jjDispute.courthouseLocation) {
      let courtFound = this.lookupsService.courthouseAgencies?.filter(x => x.id === jjDispute.courtAgenId);
      if (courtFound?.length > 0) jjDispute.courthouseLocation = courtFound[0].name;
      else jjDispute.courthouseLocation = jjDispute.courtAgenId;
    }

    jjDispute.jjDisputedCounts?.forEach(jjDisputedCount => {
      if (jjDisputedCount.description?.length === 5 && Number.isInteger(Number.parseInt(jjDisputedCount.description))) {
        let statute = this.lookupsService.statutes.filter(i => i.id === jjDisputedCount.description).shift();
        if (statute) {
          jjDisputedCount.description = [statute.actCode, statute.code, statute.shortDescriptionText].join(" ");
        } else {
          jjDisputedCount.description = jjDisputedCount.description + " - statute could not be found";
        }
      }
    });

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
  contactName?: string;
  contactGivenNames?: string;
  occamDisputantName?: string;
  occamDisputantGivenNames?: string;
  address?: string;
}
