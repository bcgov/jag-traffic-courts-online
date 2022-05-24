import { ConfigService } from '@config/config.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { Statute } from 'app/api/model/statute.model';
import { LookupService } from 'app/api/api/lookup.service';
import { Observable, BehaviorSubject } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';

export interface ILookupsService {
  statutes$: BehaviorSubject<Statute[]>;
  statutes: Statute[];
  getStatutes(): Observable<Statute[]>;
}

@Injectable({
  providedIn: 'root',
})
export class LookupsService implements ILookupsService {
  private _statutes: BehaviorSubject<Statute[]>;
  

  constructor(
    private toastService: ToastService,
    private logger: LoggerService,
    private configService: ConfigService,
    private lookupService: LookupService
  ) { 
    this._statutes = new BehaviorSubject<Statute[]>(null);
    this.getStatutes();
  }

/**
   * Get the statutes from Redis.
   *
   * @param none
   */
 public getStatutes(): Observable<StatuteView[]> {

    return this.lookupService.apiLookupGet()
      .pipe(
        map((response: Statute[]) =>
          response ? response : null
        ),
        map((statutes: StatuteView[]) => {
          statutes.forEach(resp => {resp.statuteString = this.getStatuteString(resp)});
          statutes.sort((a,b) => {if (a.statuteString < b.statuteString) return -1; })
          return statutes;
        }),
        tap((statutes) =>
          this.logger.info('LookupsService::getStatutes', statutes.length)
        ),
        catchError((error: any) => {
          this.toastService.openErrorToast(this.configService.statute_error);
          this.logger.error(
            'LookupsService::getStatutes error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  public get statutes$(): BehaviorSubject<StatuteView[]> {
    return this._statutes;
  }

  public get statutes(): StatuteView[] {
    return this._statutes.value;
  }

  public getStatuteString(statute: Statute): string {
    return statute.act + " " + statute.section + " " + statute.description;
  }
}

export interface StatuteView extends Statute {
  statuteString?: string;
}