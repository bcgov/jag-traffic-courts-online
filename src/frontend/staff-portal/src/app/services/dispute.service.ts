import { ConfigService } from '@config/config.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { DisputeService as DisputeApiService, Dispute as DisputeBase, DisputeWithUpdates as DisputeWithUpdatesBase, DisputeUpdateRequest as DisputantUpdateRequestBase, DisputeUpdateRequestStatus2, DisputeListItem, PagedDisputeListItemCollection, DisputeStatus, GetDisputeCountResponse, SortDirection, ExcludeStatus } from 'app/api';
import { Observable, BehaviorSubject } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { EventEmitter, Injectable } from '@angular/core';
import { TableFilter } from '@shared/models/table-filter-options.model';
import { HttpClient, HttpContext, HttpHeaders, HttpResponse } from '@angular/common/http';
import { AuthService } from './auth.service';

export interface IDisputeService {
  disputes$: Observable<Dispute[]>;
  disputes: Dispute[];
  disputantUpdateRequests$: Observable<DisputantUpdateRequest[]>;
  disputantUpdateRequests: DisputantUpdateRequest[];
  getDisputes(sortBy: Array<string>, sortDirection: Array<SortDirection>, pageNumber: number, 
    filters?: TableFilter): Observable<PagedDisputeListItemCollection>;
  getDisputeStatusCount(status?: DisputeStatus): Observable<GetDisputeCountResponse>;
}

@Injectable({
  providedIn: 'root',
})
export class DisputeService implements IDisputeService {
  private _disputes: BehaviorSubject<Dispute[]>;
  private _disputantUpdateRequests: BehaviorSubject<DisputantUpdateRequest[]>;
  private _dispute: BehaviorSubject<Dispute>;
  public refreshDisputes: EventEmitter<any> = new EventEmitter();

  constructor(
    private toastService: ToastService,
    private logger: LoggerService,
    private configService: ConfigService,
    private disputeApiService: DisputeApiService,
    private http: HttpClient,
    private authService: AuthService
  ) {
    this._disputes = new BehaviorSubject<Dispute[]>(null);
  }

  /**
   * Get the disputes with pending update requests
   *
   * @param none
   */
  public getDisputesWithPendingUpdates(): Observable<DisputeWithUpdates[]> {
    return this.disputeApiService.apiDisputeDisputeswithupdaterequestsGet()
      .pipe(
        map((response: DisputeWithUpdates[]) => {
          this.logger.info('DisputeService::getDisputesWithPendingUpdates', response);
          response.forEach(x => {
            x.disputantGivenNames = `${x.disputantGivenName1}${x.disputantGivenName2 ? ' ' + x.disputantGivenName2 : ''}${x.disputantGivenName3 ? ' ' + x.disputantGivenName3 : ''}`;
          });
          return response;
        }),
        catchError((error: any) => {
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error(
            'DisputeService::getDisputesWithPendingUpdates error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  /**
     * Get the dispute update requests for a particular dispute
     *
     * @param none
     */
  public getDisputeUpdateRequests(disputeId: number): Observable<DisputantUpdateRequest[]> {
    return this.disputeApiService.apiDisputeDisputeIdDisputeupdaterequestsGet(disputeId)
      .pipe(
        map((response: DisputantUpdateRequest[]) => {
          this.logger.info('DisputeService::getDisputeUpdateRequests', response);
          this._disputantUpdateRequests?.next(response);
          response.forEach(disputantUpdateRequest => {
            disputantUpdateRequest.newStatus = disputantUpdateRequest.status;
          });
          return response;
        }),
        catchError((error: any) => {
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error(
            'DisputeService::getDisputeUpdateRequests error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  /**
     * Get the dispute status count from RSI
     *
     * @param status
     */
  public getDisputeStatusCount(status?: DisputeStatus): Observable<GetDisputeCountResponse> {
    return this.disputeApiService.apiDisputeDisputesCountGet(status)
      .pipe(
        map((response: GetDisputeCountResponse) => {
          this.logger.info('DisputeService::getDisputeStatusCount', response);
          return response;
        }),
        catchError((error: any) => {
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error(
            'DisputeService::getDisputeStatusCount error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  /**
     * Get the disputes from RSI
     *
     * @param none
     */
  public getDisputes(sortBy: Array<string>, sortDirection: Array<SortDirection>, pageNumber: number, 
    filters?: TableFilter): Observable<PagedDisputeListItemCollection> {
    return this.disputeApiService.apiDisputeDisputesGet(filters.status ? undefined : [ExcludeStatus.Cancelled, 
      ExcludeStatus.Processing, ExcludeStatus.Rejected, ExcludeStatus.Concluded], filters.ticketNumber, filters.disputantSurname, 
      filters.status ? [filters.status] : [DisputeStatus.New, DisputeStatus.Validated], filters.dateSubmittedFrom, 
      filters.dateSubmittedTo, undefined, sortBy, sortDirection, undefined, pageNumber, 25)
      .pipe(
        map((response: PagedDisputeListItemCollection) => {
          this.logger.info('DisputeService::getDisputes', response);
          this._disputes.next(response.items);
          response.items.forEach(dispute => {
            dispute = this.joinDisputantGivenNames(dispute);
            dispute = this.joinContactGivenNames(dispute);
            dispute = this.joinLawyerNames(dispute);
            dispute = this.joinAddressLines(dispute);
          });

          return response;
        }),
        catchError((error: any) => {
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error(
            'DisputeService::getDisputes error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  public get disputantUpdateRequests$(): Observable<DisputantUpdateRequest[]> {
    return this._disputantUpdateRequests.asObservable();
  }

  public get disputantUpdateRequests(): DisputantUpdateRequest[] {
    return this._disputantUpdateRequests.value;
  }

  public get disputes$(): Observable<Dispute[]> {
    return this._disputes.asObservable();
  }

  public get disputes(): Dispute[] {
    return this._disputes.value;
  }

  /**
   * Get the dispute from RSI by Id.
   *
   * @param disputeId
   */
  public getDispute(disputeId: number): Observable<Dispute> {

    return this.disputeApiService.apiDisputeDisputeIdGet(disputeId)
      .pipe(
        map((response: Dispute) => {
          this.logger.info('DisputeService::getDispute', response)
          if (response) {
            response = this.joinDisputantGivenNames(response);
            response = this.joinContactGivenNames(response);
            response = this.joinLawyerNames(response);
            response = this.joinAddressLines(response);
          }
          return response ? response : null
        }),
        catchError((error: any) => {
          var errorMsg = error?.error?.detail != null ? error.error.detail : this.configService.dispute_error;
          this.toastService.openErrorToast(errorMsg);
          this.logger.error(
            'DisputeService::getDispute error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  public get dispute$(): Observable<Dispute> {
    return this._dispute.asObservable();
  }

  public get dispute(): Dispute {
    return this._dispute.value;
  }

  /**
     * Put the dispute to RSI by Id.
     *
     * @param disputeId
     */
  public putDispute(disputeId: number, staffComment?: string, dispute?: Dispute): Observable<Dispute> {
    dispute.disputantBirthdate = "2001-01-01"; // TODO: remove this after disputant birthdate gone from schema
    dispute = this.splitDisputantGivenNames(dispute);
    dispute = this.splitContactGivenNames(dispute);
    dispute = this.splitLawyerNames(dispute);
    dispute = this.splitAddressLines(dispute);

    return this.disputeApiService.apiDisputeDisputeIdPut(disputeId, staffComment, dispute)
      .pipe(
        map((response: Dispute) => {
          this.logger.info('DisputeService::putDispute', response)
          return response ? response : null
        }),
        catchError((error: any) => {
          var errorMsg = error?.error?.detail != null ? error.error.detail : this.configService.dispute_error;
          this.toastService.openErrorToast(errorMsg);
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error(
            'DisputeService::putDispute error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  /**
     * Put the dispute to RSI by Id.
     *
     * @param disputeId
     */
  public cancelDispute(disputeId: number, cancelledReason: string): Observable<Dispute> {
    return this.disputeApiService.apiDisputeDisputeIdCancelPut(disputeId, cancelledReason)
      .pipe(
        map((response: any) => {
          this.logger.info('DisputeService::cancelDispute', response)
          return response ? response : null
        }),
        catchError((error: any) => {
          var errorMsg = error?.error?.detail != null ? error.error.detail : this.configService.dispute_error;
          this.toastService.openErrorToast(errorMsg);
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error('DisputeService::cancelDispute error has occurred: ', error);
          throw error;
        })
      );
  }

  /**
   * Put the dispute to RSI by Id.
   *
   * @param disputeId
   */
  public validateDispute(disputeId: number, dispute?: Dispute): Observable<Dispute> {

    dispute.disputantBirthdate = "2001-01-01"; // TODO: remove this after disputant birthdate gone from schema
    dispute = this.splitDisputantGivenNames(dispute);
    dispute = this.splitContactGivenNames(dispute);
    dispute = this.splitLawyerNames(dispute);
    dispute = this.splitAddressLines(dispute);

    return this.disputeApiService.apiDisputeDisputeIdValidatePut(disputeId, dispute)
      .pipe(
        map((response: any) => {
          this.logger.info('DisputeService::validateDispute', response)

          return response ? response : null
        }),
        catchError((error: any) => {
          var errorMsg = error?.error?.detail != null ? error.error.detail : this.configService.dispute_error;
          this.toastService.openErrorToast(errorMsg);
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error(
            'DisputeService::validateDispute error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  /**
   * Put the dispute to RSI by Id.
   *
   * @param disputeId
   */
  public rejectDispute(disputeId: number, rejectedReason: string): Observable<Dispute> {

    return this.disputeApiService.apiDisputeDisputeIdRejectPut(disputeId, rejectedReason)
      .pipe(
        map((response: any) => {
          this.logger.info('DisputeService::rejectDispute', response)

          return response ? response : null
        }),
        catchError((error: any) => {
          var errorMsg = error?.error?.detail != null ? error.error.detail : this.configService.dispute_error;
          this.toastService.openErrorToast(errorMsg);
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error(
            'DisputeService::rejectDispute error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  /**
 * Put the dispute to RSI by Id.
 *
 * @param disputeId
 */
  public submitDispute(disputeId: number): Observable<Dispute> {

    return this.disputeApiService.apiDisputeDisputeIdSubmitPut(disputeId)
      .pipe(
        map((response: any) => {
          this.logger.info('DisputeService::submitDispute', response)
          return response ? response : null
        }),
        catchError((error: any) => {
          var errorMsg = this.configService.dispute_error;
          error?.error?.errors?.forEach(error => { // Review this is correct or not. Seems not working well
            errorMsg += " " + error;
          });
          this.toastService.openErrorToast(errorMsg);
          this.logger.error(
            'DisputeService::submitDispute error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  /**
  * Accept the dispute update request
  *
  * @param disputeId
  */
  public acceptDisputeUpdateRequest(updateStatusId: number): Observable<number> {

    return this.disputeApiService.apiDisputeUpdaterequestUpdateStatusIdAcceptPut(updateStatusId)
      .pipe(
        map((response: number) => {
          this.logger.info('DisputeService::acceptDisputeUpdateRequest', response)
          return response ? response : null
        }),
        catchError((error: any) => {
          var errorMsg = error?.error?.detail != null ? error.error.detail : this.configService.dispute_error;
          this.toastService.openErrorToast(errorMsg);
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error(
            'DisputeService::acceptDisputeUpdateRequest error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  /**
  * Accept the dispute update request
  *
  * @param disputeId
  */
  public rejectDisputeUpdateRequest(updateStatusId: number): Observable<any> {

    return this.disputeApiService.apiDisputeUpdaterequestUpdateStatusIdRejectPut(updateStatusId)
      .pipe(
        map((response: any) => {
          this.logger.info('DisputeService::rejectDisputeUpdateRequest', response)
          return response ? response : null
        }),
        catchError((error: any) => {
          var errorMsg = error?.error?.detail != null ? error.error.detail : this.configService.dispute_error;
          this.toastService.openErrorToast(errorMsg);
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error(
            'DisputeService::rejectDisputeUpdateRequest error has occurred: ',
            error
          );
          throw error;
        })
      );
  }


  /**
  * Put to Resend Email Verification
  *
  * @param emailVerificationToken
  */
  public resendEmailVerification(disputeId: number): Observable<string> {
    return this.disputeApiService.apiDisputeDisputeIdResendemailverifyPut(disputeId)
      .pipe(
        map((response: any) => {
          this.logger.info('DisputeService::resendEmailVerification', response)
          return response ? response : null
        }),
        catchError((error: any) => {
          var errorMsg = error.error.detail != null ? error.error.detail : this.configService.dispute_error;
          this.toastService.openErrorToast(errorMsg);
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error(
            'DisputeService::resendEmailVerification error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  public splitDisputantGivenNames(Dispute: Dispute): Dispute {
    let dispute = Dispute;

    // split up where spaces occur and stuff in given names 1,2,3
    if (Dispute.disputantGivenNames) {
      let givenNames = Dispute.disputantGivenNames.split(" ");
      if (givenNames.length > 0) dispute.disputantGivenName1 = givenNames[0];
      if (givenNames.length > 1) dispute.disputantGivenName2 = givenNames[1];
      if (givenNames.length > 2) dispute.disputantGivenName3 = givenNames[2];
    }

    return dispute;
  }

  public splitContactGivenNames(Dispute: Dispute): Dispute {
    let dispute = Dispute;

    // split up where spaces occur and stuff in given names 1,2,3
    if (Dispute.contactGivenNames) {
      let givenNames = Dispute.contactGivenNames.split(" ");
      if (givenNames.length > 0) dispute.contactGiven1Nm = givenNames[0];
      if (givenNames.length > 1) dispute.contactGiven2Nm = givenNames[1];
      if (givenNames.length > 2) dispute.contactGiven3Nm = givenNames[2];
    }

    return dispute;
  }


  public joinDisputantGivenNames(Dispute: Dispute): Dispute {
    let dispute = Dispute;

    dispute.disputantGivenNames = Dispute.disputantGivenName1;
    if (Dispute.disputantGivenName2) dispute.disputantGivenNames = Dispute.disputantGivenNames + " " + Dispute.disputantGivenName2;
    if (Dispute.disputantGivenName3) dispute.disputantGivenNames = Dispute.disputantGivenNames + " " + Dispute.disputantGivenName3;

    return dispute;
  }

  public joinContactGivenNames(Dispute: Dispute): Dispute {
    let dispute = Dispute;

    dispute.contactGivenNames = Dispute.contactGiven1Nm;
    if (Dispute.contactGiven2Nm) dispute.contactGivenNames = Dispute.contactGivenNames + " " + Dispute.contactGiven1Nm;
    if (Dispute.contactGiven2Nm) dispute.contactGivenNames = Dispute.contactGivenNames + " " + Dispute.contactGiven3Nm;

    return dispute;
  }

  public splitLawyerNames(Dispute: Dispute): Dispute {
    let dispute = Dispute;

    // split up where spaces occur and stuff in given names 1,2,3
    if (Dispute.lawyerFullName) {
      let lawyerNames = Dispute.lawyerFullName.split(" ");
      if (lawyerNames.length > 0) dispute.lawyerSurname = lawyerNames[lawyerNames.length - 1]; // last one
      if (lawyerNames.length > 1) dispute.lawyerGivenName1 = lawyerNames[0];
      if (lawyerNames.length > 2) dispute.lawyerGivenName2 = lawyerNames[1];
      if (lawyerNames.length > 3) dispute.lawyerGivenName3 = lawyerNames[2];
    }

    return dispute;
  }

  public joinLawyerNames(Dispute: Dispute): Dispute {
    let dispute = Dispute;

    dispute.lawyerFullName = dispute.lawyerGivenName1;
    if (dispute.lawyerGivenName2) dispute.lawyerFullName = dispute.lawyerFullName + " " + dispute.lawyerGivenName2;
    if (dispute.lawyerGivenName3) dispute.lawyerFullName = dispute.lawyerFullName + " " + dispute.lawyerGivenName3;
    if (dispute.lawyerSurname) dispute.lawyerFullName = dispute.lawyerFullName + " " + dispute.lawyerSurname;

    return dispute;
  }

  public splitAddressLines(Dispute: Dispute): Dispute {
    let dispute = Dispute;

    // split up where commas occur and stuff in address lines 1,2,3
    if (Dispute.address) {
      let addressLines = Dispute.address.split(",");
      if (addressLines.length > 0) dispute.addressLine1 = addressLines[0].length > 100 ? addressLines[0].substring(0, 100) : addressLines[0];
      if (addressLines.length > 1) dispute.addressLine2 = addressLines[1].length > 100 ? addressLines[1].substring(0, 100) : addressLines[1];
      if (addressLines.length > 2) dispute.addressLine3 = addressLines[2].length > 100 ? addressLines[2].substring(0, 100) : addressLines[2];
    }

    return dispute;
  }

  public joinAddressLines(Dispute: Dispute): Dispute {
    let dispute = Dispute;

    dispute.address = dispute.addressLine1;
    if (dispute.addressLine2) dispute.address = dispute.address + "," + dispute.addressLine2;
    if (dispute.addressLine3) dispute.address = dispute.address + "," + dispute.addressLine3;

    return dispute;
  }

  public apiTicketValidationPrintGet(disputeId: number, timeZone: string = Intl.DateTimeFormat().resolvedOptions().timeZone): Observable<any> {
    return this.http
    .get(`/api/dispute/${disputeId}/print?timeZone=${timeZone}`, {
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
}

export interface Dispute extends DisputeBase {
  disputantGivenNames?: string;
  contactGivenNames?: string;
  lawyerFullName?: string;
  address?: string;
  __RedGreenAlert?: string,
}

export interface Dispute extends DisputeListItem {
  disputantGivenNames?: string;
  contactGivenNames?: string;
  lawyerFullName?: string;
  address?: string;
  __RedGreenAlert?: string,
}

export interface DisputeWithUpdates extends DisputeWithUpdatesBase {
  disputantGivenNames?: string;
}

export interface DisputantUpdateRequest extends DisputantUpdateRequestBase {
  newStatus: DisputeUpdateRequestStatus2;
}
