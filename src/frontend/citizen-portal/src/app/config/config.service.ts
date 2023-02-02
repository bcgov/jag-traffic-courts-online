import { Injectable } from '@angular/core';
import { CountryCodeValue, ProvinceCodeValue } from '@config/config.model';
import { UtilsService } from '@core/services/utils.service';
import { AppConfigService } from 'app/services/app-config.service';
import { BehaviorSubject } from 'rxjs';
import CanadaProvincesJSON from 'assets/canada_provinces_list.json';
import USStatesJSON from 'assets/us_states_list.json';
import CountriesListJSON from 'assets/countries_list.json';

@Injectable({
  providedIn: 'root',
})
export class ConfigService {
  private disputeSubmitted: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private disputeValidationError: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private ticketError: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private disputeCreateError: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private languageError: BehaviorSubject<string> = new BehaviorSubject<string>('');

  private _countries: CountryCodeValue[] = [];
  private _provincesAndStates: ProvinceCodeValue[] = [];

  private _bcCodeValue: ProvinceCodeValue;
  private _canadaCodeValue: CountryCodeValue;
  private _usaCodeValue: CountryCodeValue;

  constructor(
    protected utilsService: UtilsService,
    protected appConfigService: AppConfigService
  ) {
    // import countries
    this._countries = [];
    CountriesListJSON.countryCodeValues.forEach(x => {
      this._countries.push(new CountryCodeValue(+x.ctryId, x.ctryLongNm));
    })
    this._countries = this._countries.sort((a,b)=> a.ctryLongNm > b.ctryLongNm ? 1 : -1);

    // import provinces
    this._provincesAndStates = [];
    let i = 1;
    CanadaProvincesJSON.provinceCodeValues.forEach(x => {
      this._provincesAndStates.push(new ProvinceCodeValue(+x.ctryId, +x.provSeqNo, x.provNm, x.provAbbreviationCd, i));
      i++;
    })

    // import states
    USStatesJSON.provinceCodeValues.forEach(x => {
      this._provincesAndStates.push(new ProvinceCodeValue(+x.ctryId, +x.provSeqNo, x.provNm, x.provAbbreviationCd, i));
      i++;
    })
    this._provincesAndStates = this._provincesAndStates.sort((a,b)=> a.provNm > b.provNm ? 1 : -1);

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

  private getBCCodeValue(): ProvinceCodeValue {
    return this._provincesAndStates.filter(x => x.provAbbreviationCd === "BC").shift();
  }

  private getCanadaCodeValue(): CountryCodeValue {
    return this._countries.filter(x => x.ctryLongNm === "Canada").shift();
  }

  private getUSACodeValue(): CountryCodeValue {
    return this._countries.filter(x => x.ctryLongNm === "USA").shift();
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
}
