import { ConfigService } from '@config/config.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { DisputeService as DisputeApiService, Dispute} from 'app/api';
import { Observable, BehaviorSubject } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { EventEmitter, Injectable } from '@angular/core';
import { DatePipe } from '@angular/common';

export interface IDisputeService {
  disputes$: Observable<DisputeExtended[]>;
  disputes: DisputeExtended[];
  getDisputes(): Observable<DisputeExtended[]>;
}

@Injectable({
  providedIn: 'root',
})
export class DisputeService implements IDisputeService {
  private _disputes: BehaviorSubject<DisputeExtended[]>;
  private _dispute: BehaviorSubject<DisputeExtended>;
  public refreshDisputes: EventEmitter<any> = new EventEmitter();


  constructor(
    private toastService: ToastService,
    private logger: LoggerService,
    private configService: ConfigService,
    private disputeApiService: DisputeApiService,
    private datePipe:DatePipe
  ) {
    this._disputes = new BehaviorSubject<DisputeExtended[]>(null);
  }

  /**
     * Get the disputes from RSI excluding CANCELLED
     *
     * @param none
     */
  public getDisputes(): Observable<DisputeExtended[]> {
    return this.disputeApiService.apiDisputeDisputesGet("CANCELLED")
      .pipe(
        map((response: DisputeExtended[]) => {
          this.logger.info('DisputeService::getDisputes', response);
          this._disputes.next(response);
          response.forEach(dispute => {
            dispute = this.joinGivenNames(dispute);
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

  public get disputes$(): Observable<DisputeExtended[]> {
    return this._disputes.asObservable();
  }

  public get disputes(): DisputeExtended[] {
    return this._disputes.value;
  }

  /**
   * Get the dispute from RSI by Id.
   *
   * @param disputeId
   */
  public getDispute(disputeId: number): Observable<DisputeExtended> {

    return this.disputeApiService.apiDisputeDisputeIdGet(disputeId)
      .pipe(
        map((response: DisputeExtended) => {
          this.logger.info('DisputeService::getDispute', response)
          if (response) {
            response = this.joinGivenNames(response);
            response = this.joinLawyerNames(response);
            response = this.joinAddressLines(response);
          }
          return response ? response : null
        }),
        catchError((error: any) => {
          var errorMsg = error.error.detail != null ? error.error.detail : this.configService.dispute_error;
          this.toastService.openErrorToast(errorMsg);
          this.logger.error(
            'DisputeService::getDispute error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  public get dispute$(): Observable<DisputeExtended> {
    return this._dispute.asObservable();
  }

  public get dispute(): DisputeExtended {
    return this._dispute.value;
  }

  /**
     * Put the dispute to RSI by Id.
     *
     * @param disputeId
     */
  public putDispute(disputeId: number, dispute: DisputeExtended): Observable<DisputeExtended> {

     dispute.disputantBirthdate = this.datePipe.transform(dispute?.disputantBirthdate, "yyyy-MM-dd");
     dispute.issuedDate = this.datePipe.transform(dispute?.issuedDate,"yyyy-MM-ddTHH:mm:ss");
     dispute = this.splitGivenNames(dispute);
     dispute = this.splitLawyerNames(dispute);
     dispute = this.splitAddressLines(dispute);
    //  dispute.violationTicket =
    return this.disputeApiService.apiDisputeDisputeIdPut(disputeId, dispute)
      .pipe(
        map((response: DisputeExtended) => {
          this.logger.info('DisputeService::putDispute', response)
          return response ? response : null
        }),
        catchError((error: any) => {
          var errorMsg = error.error.detail != null ? error.error.detail : this.configService.dispute_error;
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
  public cancelDispute(disputeId: number): Observable<DisputeExtended> {

    return this.disputeApiService.apiDisputeDisputeIdCancelPut(disputeId)
      .pipe(
        map((response: any) => {
          this.logger.info('DisputeService::cancelDispute', response)
          return response ? response : null
        }),
        catchError((error: any) => {
          var errorMsg = error.error.detail != null ? error.error.detail : this.configService.dispute_error;
          this.toastService.openErrorToast(errorMsg);
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error(
            'DisputeService::cancelDispute error has occurred: ',
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
  public validateDispute(disputeId: number): Observable<DisputeExtended> {

    return this.disputeApiService.apiDisputeDisputeIdValidatePut(disputeId)
      .pipe(
        map((response: any) => {
          this.logger.info('DisputeService::validateDispute', response)

          return response ? response : null
        }),
        catchError((error: any) => {
          var errorMsg = error.error.detail != null ? error.error.detail : this.configService.dispute_error;
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
  public rejectDispute(disputeId: number, rejectedReason: string): Observable<DisputeExtended> {

    return this.disputeApiService.apiDisputeDisputeIdRejectPut(disputeId, rejectedReason)
      .pipe(
        map((response: any) => {
          this.logger.info('DisputeService::rejectDispute', response)

          return response ? response : null
        }),
        catchError((error: any) => {
          var errorMsg = error.error.detail != null ? error.error.detail : this.configService.dispute_error;
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
  public submitDispute(disputeId: number): Observable<DisputeExtended> {

    return this.disputeApiService.apiDisputeDisputeIdSubmitPut(disputeId)
      .pipe(
        map((response: any) => {
          this.logger.info('DisputeService::submitDispute', response)
          return response ? response : null
        }),
        catchError((error: any) => {
          var errorMsg = error.error.detail != null ? error.error.detail : this.configService.dispute_error;
          this.toastService.openErrorToast(errorMsg);
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error(
            'DisputeService::submitDispute error has occurred: ',
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
  public resendEmailVerification(emailVerificationToken: string): Observable<any> {

    return this.disputeApiService.apiDisputeEmailUuidResendPut(emailVerificationToken)
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
  public splitGivenNames(disputeExtended: DisputeExtended):DisputeExtended {
    let dispute = disputeExtended;

    // split up where spaces occur and stuff in given names 1,2,3
    if (disputeExtended.disputantGivenNames) {
      let givenNames = disputeExtended.disputantGivenNames.split(" ");
      if (givenNames.length > 0) dispute.disputantGivenName1 = givenNames[0];
      if (givenNames.length > 1) dispute.disputantGivenName2 = givenNames[1];
      if (givenNames.length > 2) dispute.disputantGivenName3 = givenNames[2];
    }

    return dispute;
  }

  public joinGivenNames(disputeExtended: DisputeExtended):DisputeExtended {
    let dispute = disputeExtended;

    dispute.disputantGivenNames = disputeExtended.disputantGivenName1;
    if (disputeExtended.disputantGivenName2 ) dispute.disputantGivenNames = disputeExtended.disputantGivenNames + " " + disputeExtended.disputantGivenName2;
    if (disputeExtended.disputantGivenName3 ) dispute.disputantGivenNames = disputeExtended.disputantGivenNames + " " + disputeExtended.disputantGivenName3;

    return dispute;
  }

  public splitLawyerNames(disputeExtended: DisputeExtended):DisputeExtended {
    let dispute = disputeExtended;

    // split up where spaces occur and stuff in given names 1,2,3
    if (disputeExtended.lawyerFullName) {
      let lawyerNames = disputeExtended.lawyerFullName.split(" ");
      if (lawyerNames.length > 0) dispute.lawyerSurname = lawyerNames[lawyerNames.length - 1]; // last one
      if (lawyerNames.length > 1) dispute.lawyerGivenName1 = lawyerNames[0];
      if (lawyerNames.length > 2) dispute.lawyerGivenName2 = lawyerNames[1];
      if (lawyerNames.length > 3) dispute.lawyerGivenName3 = lawyerNames[2];
    }

    return dispute;
  }

  public joinLawyerNames(disputeExtended: DisputeExtended):DisputeExtended {
    let dispute = disputeExtended;

    dispute.lawyerFullName = dispute.lawyerGivenName1;
    if (dispute.lawyerGivenName2) dispute.lawyerFullName = dispute.lawyerFullName + " " + dispute.lawyerGivenName2;
    if (dispute.lawyerGivenName3) dispute.lawyerFullName = dispute.lawyerFullName + " " + dispute.lawyerGivenName3;
    if (dispute.lawyerSurname) dispute.lawyerFullName = dispute.lawyerFullName + " " + dispute.lawyerSurname;

    return dispute;
  }

  public splitAddressLines(disputeExtended: DisputeExtended):DisputeExtended {
    let dispute = disputeExtended;

    // split up where spaces occur and stuff in given names 1,2,3
    if (disputeExtended.address) {
      let addressLines = disputeExtended.address.split(",");
      if (addressLines.length > 0) dispute.addressLine1 = addressLines[0];
      if (addressLines.length > 1) dispute.addressLine2 = addressLines[1];
      if (addressLines.length > 2) dispute.addressLine3 = addressLines[2];
    }

    return dispute;
  }

  public joinAddressLines(disputeExtended: DisputeExtended):DisputeExtended {
    let dispute = disputeExtended;

    dispute.address = dispute.addressLine1;
    if (dispute.addressLine2) dispute.address = dispute.address + "," + dispute.addressLine2;
    if (dispute.addressLine3) dispute.address = dispute.address + "," + dispute.addressLine3;

    return dispute;
  }
}

export interface DisputeExtended extends Dispute {
  disputantGivenNames?: string;
  lawyerFullName?: string;
  address?: string;
  __DateSubmitted?: Date,
  __RedGreenAlert?: string,
  __FilingDate?: Date, // extends citizen portal, set in staff portal, initially undefined
  __CourtHearing: boolean, // if at least one count requests court hearing
  __UserAssignedTs?: Date,
}
