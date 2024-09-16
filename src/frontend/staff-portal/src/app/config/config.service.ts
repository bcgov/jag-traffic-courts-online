import { Injectable } from '@angular/core';
import { Config, Configuration, CountryCodeValue, ProvinceCodeValue } from '@config/config.model';
import { SortWeight, UtilsService } from '@core/services/utils.service';
import { AppConfigService } from 'app/services/app-config.service';
import { BehaviorSubject, catchError, first, map, Observable, of, tap, throwError } from 'rxjs';
import CanadaProvincesJSON from 'assets/canada_provinces_list.json';
import USStatesJSON from 'assets/us_states_list.json';
import CountriesListJSON from 'assets/countries_list.json';
import { CourthouseTeam } from 'app/services/lookups.service';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class ConfigService {
  protected configuration: Configuration;

  private disputeSubmitted: BehaviorSubject<string> =
    new BehaviorSubject<string>('');
  private disputeValidationError: BehaviorSubject<string> =
    new BehaviorSubject<string>('');
  private ticketError: BehaviorSubject<string> = new BehaviorSubject<string>(
    ''
  );
  private disputeError: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private keycloakError: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private historyError: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private disputeCreateError: BehaviorSubject<string> =
    new BehaviorSubject<string>('');
  private statuteError: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private languageError: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private provinceError: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private countryError: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private agencyError: BehaviorSubject<string> = new BehaviorSubject<string>('');

  private _countries: CountryCodeValue[] = [];
  private _provincesAndStates: ProvinceCodeValue[] = [];
  private _courthouses: CourthouseTeam[] = [];

  private _bcCodeValue: ProvinceCodeValue;
  private _canadaCodeValue: CountryCodeValue;
  private _usaCodeValue: CountryCodeValue;

  constructor(
    protected utilsService: UtilsService,
    protected appConfigService: AppConfigService,
    protected http: HttpClient
  ) {
    // import countries
    this._countries = [];
    CountriesListJSON.countryCodeValues.forEach(x => {
      this._countries.push(new CountryCodeValue(+x.ctryId, x.ctryLongNm));
    })
    this._countries = this._countries.sort((a, b) => a.ctryLongNm > b.ctryLongNm ? 1 : -1);

    // import provinces
    this._provincesAndStates = [];
    let i = 1;
    CanadaProvincesJSON.provinceCodeValues.forEach(x => {
      this._provincesAndStates.push(new ProvinceCodeValue(+x.ctryId, +x.provSeqNo, x.provNm, x.provAbbreviationCd, i));
      i++;
    })

    // Load courthouses
    this.loadCourthouseData();

    // import states
    USStatesJSON.provinceCodeValues.forEach(x => {
      this._provincesAndStates.push(new ProvinceCodeValue(+x.ctryId, +x.provSeqNo, x.provNm, x.provAbbreviationCd, i));
      i++;
    })
    this._provincesAndStates = this._provincesAndStates.sort((a, b) => a.provNm > b.provNm ? 1 : -1);

    this._bcCodeValue = this.getBCCodeValue();
    this._canadaCodeValue = this.getCanadaCodeValue();
    this._usaCodeValue = this.getUSACodeValue();
  }

  public get dispute_submitted$(): BehaviorSubject<string> {
    return this.disputeSubmitted;
  }

  public get dispute_submitted(): string {
    return this.disputeSubmitted.value;
  }

  public get dispute_validation_error$(): BehaviorSubject<string> {
    return this.disputeValidationError;
  }

  public get dispute_validation_error(): string {
    return this.disputeValidationError.value;
  }

  public get ticket_error$(): BehaviorSubject<string> {
    return this.ticketError;
  }

  public get dispute_error(): string {
    return this.disputeError.value;
  }

  public get keycloak_error(): string {
    return this.keycloakError.value;
  }

  public get history_error(): string {
    return this.historyError.value;
  }

  public get dispute_error$(): BehaviorSubject<string> {
    return this.disputeError;
  }

  public get history_error$(): BehaviorSubject<string> {
    return this.historyError;
  }

  public get ticket_error(): string {
    return this.ticketError.value;
  }

  public get statute_error$(): BehaviorSubject<string> {
    return this.statuteError;
  }
  public get statute_error(): string {
    return this.statuteError.value;
  }

  public get language_error$(): BehaviorSubject<string> {
    return this.languageError;
  }
  public get language_error(): string {
    return this.languageError.value;
  }

  public get province_error$(): BehaviorSubject<string> {
    return this.provinceError;
  }
  public get province_error(): string {
    return this.provinceError.value;
  }

  public get country_error(): string {
    return this.countryError.value;
  }

  public get agency_error$(): BehaviorSubject<string> {
    return this.agencyError;
  }
  public get agency_error(): string {
    return this.agencyError.value;
  }

  public get dispute_create_error$(): BehaviorSubject<string> {
    return this.disputeCreateError;
  }

  public get dispute_create_error(): string {
    return this.disputeCreateError.value;
  }

  public get provincesAndStates(): ProvinceCodeValue[] {
    return [...this._provincesAndStates];
  }

  public get countries(): CountryCodeValue[] {
    return [...this._countries];
  }

  public get bcCodeValue(): ProvinceCodeValue {
    return this._bcCodeValue;
  }

  public get canadaCodeValue(): CountryCodeValue {
    return this._canadaCodeValue;
  }

  public get usaCodeValue(): CountryCodeValue {
    return this._usaCodeValue;
  }

  public get statuses(): Config<number>[] {
    return [...this.configuration.statuses].sort(this.sortConfigByName());
  }

  /**
   * @description
   * Load the runtime configuration.
   */
  public load(): Observable<Configuration> {
    if (!this.configuration) {
      return this.appConfigService.loadAppConfig()
    }

    return of({ ...this.configuration });
  }

  /**
   * @description
   * Get the configuration for bootstrapping the application.
   */
  private getConfiguration(): Observable<Configuration> {
    return null;
  }

  private getBCCodeValue(): ProvinceCodeValue {
    return this._provincesAndStates.filter(x => x.provAbbreviationCd === "BC").shift();
  }

  private getCanadaCodeValue(): CountryCodeValue {
    return this._countries.filter(x => x.ctryLongNm === "Canada").shift();
  }

  private getUSACodeValue(): CountryCodeValue {
    return this._countries.filter(x => x.ctryLongNm === "USA").shift();
  }

  public getCtryLongNm(ctryId: number) {
    let countriesFound = this._countries.filter(x => x.ctryId === ctryId).shift();
    return countriesFound?.ctryLongNm;
  }

  public getProvNm(ctryId: number, provSeqNo: number) {
    let provincesFound = this._provincesAndStates.filter(x => x.ctryId === ctryId && x.provSeqNo === provSeqNo).shift();
    return provincesFound?.provNm;
  }

  public getProvAbbreviationCd(ctryId: number, provSeqNo: number) {
    let provincesFound = this._provincesAndStates.filter(x => x.ctryId === ctryId && x.provSeqNo === provSeqNo).shift();
    return provincesFound?.provAbbreviationCd;
  }

  /**
   * @description
   * Sort the configuration by name.
   */
  private sortConfigByName(): (
    a: Config<number | string>,
    b: Config<number | string>
  ) => SortWeight {
    return (a: Config<number | string>, b: Config<number | string>) =>
      this.utilsService.sortByKey<Config<number | string>>(a, b, 'name');
  }

  private loadCourthouseData(): void {
    this.http.get<{ courthouses: CourthouseTeam[] }>('/assets/courthouse-data.json')
      .pipe(
        first(),
        map(response => response.courthouses),
        tap(courthouses => this._courthouses = courthouses),
        catchError(error => {
          console.error('Error loading courthouse data:', error);
          return throwError(() => new Error(error));
        })
      )
      .subscribe();
  }

  public get courthouses(): CourthouseTeam[] {
    return this._courthouses;
  }
}
