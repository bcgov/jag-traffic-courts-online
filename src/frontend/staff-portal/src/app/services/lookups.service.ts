import { ConfigService } from '@config/config.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { Statute as StatuteBase, LookupService, Language, Agency, Province, Country, DisputeCaseFileStatus } from 'app/api';
import { Observable, BehaviorSubject, forkJoin, of } from 'rxjs';
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
  countries$: Observable<Country[]>;
  countries: Country[];
  getStatutes(): Observable<Statute[]>;
  getLanguages(): Observable<Language[]>;
  getProvinces(): Observable<Province[]>;
  getCountries(): Observable<Country[]>;
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
  private _countries: BehaviorSubject<Country[]> = new BehaviorSubject<Country[]>([]);
  private _disputeStatus: BehaviorSubject<DisputeCaseFileStatus[]> = new BehaviorSubject<DisputeCaseFileStatus[]>([]);

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
      provinces: this.getProvinces(),
      countries: this.getCountries(),
      disputeStatus: this.getDisputeStatus(),
    };
    return forkJoin(observables).pipe(
      map(results => {
        this._courthouseAgencies.next(results.courtLocations);
        this._statutes.next(results.statutes);
        this._languages.next(results.languages);
        this._provinces.next(results.provinces);
        this._countries.next(results.countries);
        this._disputeStatus.next(results.disputeStatus);
      }
      ));
  }

  /**
     * Get the statutes from Redis.
     *
     * @param none
     */
  public getStatutes(): Observable<Statute[]> {
    if (this._statutes.value?.length > 0) {
      return of(this._statutes.value);
    }

    return this.lookupService.apiLookupStatutesV2Get()
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
    return `${statute.actCode} ${statute.code} ${statute.shortDescriptionText}`;
  }

  /**
     * Get the language from Redis.
     *
     * @param none
     */
  public getLanguages(): Observable<Language[]> {
    if (this._languages.value?.length > 0) {
      return of(this._languages.value);
    }
    
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
    if (this._provinces.value?.length > 0) {
      return of(this._provinces.value);
    }

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

  /**
     * Get the countries from Redis.
     *
     * @param none
     */
  public getCountries(): Observable<Country[]> {
    if (this._countries.value?.length > 0) {
      return of(this._countries.value);
    }

    return this.lookupService.apiLookupCountryGet()
      .pipe(
        map((response: Country[]) =>
          response ? response : []
        ),
        map((countries: Country[]) => {
          countries.sort((a, b) => { if (a.ctryLongNm < b.ctryLongNm) return -1; })
          return countries;
        }),
        tap((countries) =>
          this.logger.info('LookupsService::getCountries', countries.length)
        ),
        catchError((error: any) => {
          this.toastService.openErrorToast(this.configService.country_error);
          this.logger.error(
            'LookupsService::getCountries error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  public get countries$(): Observable<Country[]> {
    return this._countries.asObservable();
  }

  public get countries(): Country[] {
    return this._countries.value;
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
    if (this._courthouseAgencies.value?.length > 0) {
      return of(this._courthouseAgencies.value);
    }

    return this.lookupService.apiLookupAgenciesV2Get()
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

  /**
     * Get the dispute statuses.
     *
     * @param none
     */
  public getDisputeStatus(): Observable<DisputeCaseFileStatus[]> {
    if (this._disputeStatus.value?.length > 0) {
      return of(this._disputeStatus.value);
    }

    return this.lookupService.apiLookupDisputeCaseFileStatusesV2Get()
      .pipe(
        map((response: DisputeCaseFileStatus[]) =>
          response ? response : []
        ),
        map((disputeStatus: DisputeCaseFileStatus[]) => {
          return disputeStatus;
        }),
        tap((disputeStatus) =>
          this.logger.info('LookupsService::getDisputeStatus', disputeStatus.length)
        ),
        catchError((error: any) => {
          this.toastService.openErrorToast("Dispute status not loaded");
          this.logger.error(
            'LookupsService::getDisputeStatus error has occurred: ',
            error
          );
          throw error;
        })
      );
  }

  public get disputeStatus$(): Observable<Language[]> {
    return this._disputeStatus.asObservable();
  }

  public get disputeStatus(): Language[] {
    return this._disputeStatus.value;
  }
}

export interface Statute extends StatuteBase {
  __statuteString?: string;
}

export interface CourthouseTeam extends Agency {
  __team?: string;
}
