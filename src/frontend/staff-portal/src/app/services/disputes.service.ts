import { ConfigService } from '@config/config.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { Dispute } from 'app/api/model/dispute.model';
import { DisputeService } from 'app/api/api/dispute.service';
import { Observable, BehaviorSubject } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';

export interface IDisputesService {
  disputes$: BehaviorSubject<Dispute[]>;
  disputes: Dispute[];
  getDisputes(): Observable<Dispute[]>;
}

@Injectable({
  providedIn: 'root',
})
export class DisputesService implements IDisputesService {
  private _disputes: BehaviorSubject<Dispute[]>;
  private _dispute: BehaviorSubject<Dispute>;
  

  constructor(
    private toastService: ToastService,
    private logger: LoggerService,
    private configService: ConfigService,
    private disputeService: DisputeService
  ) { 
    this._disputes = new BehaviorSubject<Dispute[]>(null);
  }

/**
   * Get the disputes from RSI.
   *
   * @param none
   */
 public getDisputes(): Observable<Dispute[]> {

    return this.disputeService.apiDisputeDisputesGet()
      .pipe(
        map((response: Dispute[]) =>
          response ? response : null
        ),
        map((disputes: Dispute[]) => {
          return disputes;
        }),
        tap((disputes) =>
          this.logger.info('DisputesService::getDisputes', disputes)
        ),
        catchError((error: any) => {
          this.toastService.openErrorToast(this.configService.dispute_error);
          this.logger.error(
            'DisputesService::getDisputes error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  public get disputes$(): BehaviorSubject<Dispute[]> {
    return this._disputes;
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

  return this.disputeService.apiDisputeDisputeIdGet(disputeId)
    .pipe(
      map((response: Dispute) =>
        response ? response : null
      ),
      map((dispute: Dispute) => {
        return dispute;
      }),
      tap((dispute) =>
        this.logger.info('DisputesService::getDispute', dispute)
      ),
      catchError((error: any) => {
        this.toastService.openErrorToast(this.configService.dispute_error);
        this.logger.error(
          'DisputesService::getDispute error has occurred: ',
          error
        );
        throw error;
      })
    );
}

public get dispute$(): BehaviorSubject<Dispute> {
  return this._dispute;
}

public get dispute(): Dispute {
  return this._dispute.value;
}
}

export type MoreDisputeStatus = 'New' | 'Processing' | 'Rejected' | 'Cancelled' | 'Alert' | 'Checked Out';
export const MoreDisputeStatus = {
  New: 'New' as MoreDisputeStatus,
  Processing: 'Processing' as MoreDisputeStatus,
  Rejected: 'Rejected' as MoreDisputeStatus,
  Cancelled: 'Cancelled' as MoreDisputeStatus,
  Alert: 'Alert' as MoreDisputeStatus,
  CheckedOut: 'Checked Out' as MoreDisputeStatus
}
export interface DisputeView extends Dispute {
  DateSubmitted?: Date,
  RedGreenAlert?: string,
  moreDisputeStatus: MoreDisputeStatus;
  FilingDate?: Date, // extends citizen portal, set in staff portal, initially undefined
  CourtHearing: boolean, // if at least one count requests court hearing
}