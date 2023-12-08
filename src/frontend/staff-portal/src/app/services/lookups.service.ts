import { ConfigService } from '@config/config.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { Statute as StatuteBase, LookupService, Language, Agency, Province } from 'app/api';
import { Observable, BehaviorSubject, forkJoin } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';

export interface ILookupsService {
  statutes$: Observable<Statute[]>;
  statutes: Statute[];
  courthouseAgencies$: Observable<Agency[]>;
  courthouseAgencies: Agency[];
  languages$: Observable<Language[]>;
  languages: Language[];
  provinces$: Observable<Province[]>;
  provinces: Province[];
  getStatutes(): Observable<Statute[]>;
  getLanguages(): Observable<Language[]>;
  getProvinces(): Observable<Province[]>;
  getCourthouseAgencies(): Observable<Agency[]>;
  getLanguageDescription(code: string): string;
  init(): Observable<any>;
}

@Injectable({
  providedIn: 'root',
})
export class LookupsService implements ILookupsService {
  private _statutes: BehaviorSubject<Statute[]> = new BehaviorSubject<Statute[]>([]);;
  private _languages: BehaviorSubject<Language[]> = new BehaviorSubject<Language[]>([]);;
  private _courthouseAgencies: BehaviorSubject<Agency[]> = new BehaviorSubject<Agency[]>([]);;
  private _provinces: BehaviorSubject<Province[]> = new BehaviorSubject<Province[]>([]);;

  constructor(
    private toastService: ToastService,
    private logger: LoggerService,
    private configService: ConfigService,
    private lookupService: LookupService
  ) {
  }

  init(): Observable<any> {
    let observables = {
      courtLocations: this.getCourthouseAgencies(),
      statutes: this.getStatutes(),
      languages: this.getLanguages(),
      provinces: this.getProvinces()
    };
    return forkJoin(observables).pipe(
      map(results => {
        this._courthouseAgencies.next(results.courtLocations);
        this._statutes.next(results.statutes);
        this._languages.next(results.languages);
        this._provinces.next(results.provinces);
      }
      ));
  }

  /**
     * Get the statutes from Redis.
     *
     * @param none
     */
  public getStatutes(): Observable<Statute[]> {

    return this.lookupService.apiLookupStatutesGet()
      .pipe(
        map((response: Statute[]) =>
          response ? response : []
        ),
        map((statutes: Statute[]) => {
          statutes.forEach(resp => { resp.__statuteString = this.getStatuteString(resp) });
          statutes.sort((a, b) => { if (a.__statuteString < b.__statuteString) return -1; })
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

  public get statutes$(): Observable<Statute[]> {
    return this._statutes.asObservable();
  }

  public get statutes(): Statute[] {
    return this._statutes.value;
  }

  public getStatuteString(statute: Statute): string {
    return `${statute.actCode} ${statute.code} ${statute.descriptionText}`;
  }

  /**
     * Get the language from Redis.
     *
     * @param none
     */
  public getLanguages(): Observable<Language[]> {

    return this.lookupService.apiLookupLanguagesGet()
      .pipe(
        map((response: Language[]) =>
          response ? response : []
        ),
        map((languages: Language[]) => {
          languages.sort((a, b) => { if (a.description < b.description) return -1; })
          return languages;
        }),
        tap((languages) =>
          this.logger.info('LookupsService::getLanguages', languages.length)
        ),
        catchError((error: any) => {
          this.toastService.openErrorToast(this.configService.language_error);
          this.logger.error(
            'LookupsService::getLanguages error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  public get languages$(): Observable<Language[]> {
    return this._languages.asObservable();
  }

  public get languages(): Language[] {
    return this._languages.value;
  }

  /**
     * Get the provinces from Redis.
     *
     * @param none
     */
  public getProvinces(): Observable<Province[]> {

    return this.lookupService.apiLookupProvinceGet()
      .pipe(
        map((response: Province[]) =>
          response ? response : []
        ),
        map((provinces: Province[]) => {
          provinces.sort((a, b) => { if (a.provNm < b.provNm) return -1; })
          return provinces;
        }),
        tap((provinces) =>
          this.logger.info('LookupsService::getProvinces', provinces.length)
        ),
        catchError((error: any) => {
          this.toastService.openErrorToast(this.configService.province_error);
          this.logger.error(
            'LookupsService::getProvinces error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  public get provinces$(): Observable<Province[]> {
    return this._provinces.asObservable();
  }

  public get provinces(): Province[] {
    return this._provinces.value;
  }

  public getLanguageDescription(code: string): string {
    let found = this.languages?.filter(x => x.code?.trim().toLowerCase() === code?.trim().toLowerCase()).shift();
    if (found) return found.description;
    else return code;
  }

  /**
      * Get the courthouse Agencies from Redis.
      *
      * @param none
      */
  public getCourthouseAgencies(): Observable<Agency[]> {

    return this.lookupService.apiLookupAgenciesGet()
      .pipe(
        map((response: Agency[]) =>
          response ? response : []
        ),
        map((agencies: Agency[]) => {
          agencies.sort((a, b) => { if (a.name < b.name) return -1; })
          return agencies.filter(a => a.typeCode === "CTH");
        }),
        tap((agencies) =>
          this.logger.info('LookupsService::getAgencies', agencies.length)
        ),
        catchError((error: any) => {
          this.toastService.openErrorToast(this.configService.agency_error);
          this.logger.error(
            'LookupsService::getAgencies error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  public get courthouseTeams(): CourthouseTeam[] {
    return this.configService.courthouses;
  }

  public get courthouseAgencies$(): Observable<Agency[]> {
    return this._courthouseAgencies.asObservable();
  }

  public get courthouseAgencies(): Agency[] {
    return this._courthouseAgencies.value;
  }

  public getCourtHouseName(id: string): string {
    let found = this.courthouseAgencies?.filter(x => x.id?.trim().toLowerCase() === id?.trim().toLowerCase()).shift();
    if (found) return found.name;
    else return id;
  }
}

export interface Statute extends StatuteBase {
  __statuteString?: string;
}

export interface CourthouseTeam extends Agency {
  __team?: string;
}
