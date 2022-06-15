import { ConfigService } from '@config/config.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { DisputeService as DisputeApiService, Dispute as DisputeApiModel } from 'app/api';
import { Observable, BehaviorSubject } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Injectable } from '@angular/core';

export interface IDisputeService {
  disputes$: Observable<Dispute[]>;
  disputes: DisputeApiModel[];
  getDisputes(): Observable<Dispute[]>;
}

@Injectable({
  providedIn: 'root',
})
export class DisputeService implements IDisputeService {
  private _disputes: BehaviorSubject<Dispute[]>;
  private _dispute: BehaviorSubject<Dispute>;


  constructor(
    private toastService: ToastService,
    private logger: LoggerService,
    private configService: ConfigService,
    private disputeApiService: DisputeApiService
  ) {
    this._disputes = new BehaviorSubject<Dispute[]>(null);
  }

  /**
     * Get the disputes from RSI excluding CANCELLED
     *
     * @param none
     */
  public getDisputes(): Observable<Dispute[]> {
    return this.disputeApiService.apiDisputeDisputesGet("CANCELLED")
      .pipe(
        map((response: Dispute[]) => {
          this.logger.info('DisputeService::getDisputes', response);
          this._disputes.next(response);
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
  public getDispute(disputeId: string): Observable<Dispute> {

    return this.disputeApiService.apiDisputeDisputeIdGet(disputeId)
      .pipe(
        map((response: Dispute) => {
          this.logger.info('DisputeService::getDispute', response)
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
  public putDispute(disputeId: string, dispute: Dispute): Observable<Dispute> {

    return this.disputeApiService.apiDisputeDisputeIdPut(disputeId, dispute)
      .pipe(
        map((response: Dispute) => {
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
  public cancelDispute(disputeId: string): Observable<Dispute> {

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
  public validateDispute(disputeId: string): Observable<Dispute> {

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
  public rejectDispute(disputeId: string, rejectedReason: string): Observable<Dispute> {

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
  public submitDispute(disputeId: string): Observable<Dispute> {

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
}

export interface Dispute extends DisputeApiModel {
  __DateSubmitted?: Date,
  __RedGreenAlert?: string,
  __FilingDate?: Date, // extends citizen portal, set in staff portal, initially undefined
  __CourtHearing: boolean, // if at least one count requests court hearing
  __AssignedTs?: Date,
}