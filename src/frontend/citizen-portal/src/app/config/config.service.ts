import { Injectable } from '@angular/core';
import { Config, Configuration, CountryCodeValue, ProvinceCodeValue } from '@config/config.model';
import { SortWeight, UtilsService } from '@core/services/utils.service';
import { AppConfigService } from 'app/services/app-config.service';
import { BehaviorSubject, Observable, of } from 'rxjs';
import CanadaProvincesJSON from 'assets/canada_provinces_list.json';
import USStatesJSON from 'assets/us_states_list.json';
import CountriesListJSON from 'assets/countries_list.json';

@Injectable({
  providedIn: 'root',
})
export class ConfigService {
  protected configuration: Configuration;

  private disputeSubmitted: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private disputeValidationError: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private ticketError: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private disputeCreateError: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private languageError: BehaviorSubject<string> = new BehaviorSubject<string>('');

  private _countries: CountryCodeValue[] = [];
  private _provincesAndStates: ProvinceCodeValue[] = [];

  constructor(
    protected utilsService: UtilsService,
    protected appConfigService: AppConfigService
  ) {

    // import countries
    this._countries = [];
    CountriesListJSON.countryCodeValues.forEach(x => {
      this._countries.push({ ctryId: +x.ctryId, ctryLongNm: x.ctryLongNm});
    })
    this._countries = this._countries.sort((a,b)=> a.ctryLongNm > b.ctryLongNm ? 1 : -1);

    // import provinces
    this._provincesAndStates = [];
    let i = 1;
    CanadaProvincesJSON.provinceCodeValues.forEach(x => {
      this._provincesAndStates.push({provId: i, ctryId: +x.ctryId, provSeqNo: +x.provSeqNo, provNm: x.provNm, provAbbreviationCd: x.provAbbreviationCd});
      i++;
    })

    // import states
    USStatesJSON.provinceCodeValues.forEach(x => {
      this._provincesAndStates.push({provId: i, ctryId: +x.ctryId, provSeqNo: +x.provSeqNo, provNm: x.provNm, provAbbreviationCd: x.provAbbreviationCd});
      i++;
    })
    this._provincesAndStates = this._provincesAndStates.sort((a,b)=> a.provNm > b.provNm ? 1 : -1);
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

  public get ticket_error(): string {
    return this.ticketError.value;
  }

  public get dispute_create_error$(): BehaviorSubject<string> {
    return this.disputeCreateError;
  }

  public get dispute_create_error(): string {
    return this.disputeCreateError.value;
  }

  public get language_error$(): BehaviorSubject<string> {
    return this.languageError;
  }
  public get language_error(): string {
    return this.languageError.value;
  }

  public get provincesAndStates() {
    return this._provincesAndStates;
  }

  public get countries() {
    return this._countries;
  }

  public getCtryLongNm(ctryId: number) {
    let countriesFound = this._countries.filter(x => x.ctryId === ctryId);
    if (countriesFound?.length > 0) return countriesFound[0].ctryLongNm;
    else return "";
  }

  public getProvNm(ctryId: number, provSeqNo: number) {
    let provincesFound = this._provincesAndStates.filter(x => x.ctryId === ctryId && x.provSeqNo === provSeqNo);
    if (provincesFound?.length > 0) return provincesFound[0].provNm;
    else return
  }

  public getProvAbbreviationCd(ctryId: number, provSeqNo: number) {
    let provincesFound = this._provincesAndStates.filter(x => x.ctryId === ctryId && x.provSeqNo === provSeqNo);
    if (provincesFound?.length > 0) return provincesFound[0].provAbbreviationCd;
    else return "";
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
   * Sort the configuration by name.
   */
  private sortConfigByName(): (
    a: Config<number | string>,
    b: Config<number | string>
  ) => SortWeight {
    return (a: Config<number | string>, b: Config<number | string>) =>
      this.utilsService.sortByKey<Config<number | string>>(a, b, 'name');
  }
}
