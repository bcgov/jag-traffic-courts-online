import { Injectable } from '@angular/core';
import { Config, Configuration, ProvinceConfig } from '@config/config.model';
import { ApiHttpResponse } from '@core/models/api-http-response.model';
import { SortWeight, UtilsService } from '@core/services/utils.service';
import { AppConfigService } from 'app/services/app-config.service';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class ConfigService {
  protected configuration: Configuration;

  private disputeSubmitted: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private disputeValidationError: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private ticketError: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private disputeCreateError: BehaviorSubject<string> = new BehaviorSubject<string>('');

  private _provinces = [
    {
      "name": "Alberta",
      "abbreviation": "AB"
    },
    {
      "name": "British Columbia",
      "abbreviation": "BC"
    },
    {
      "name": "Manitoba",
      "abbreviation": "MB"
    },
    {
      "name": "New Brunswick",
      "abbreviation": "NB"
    },
    {
      "name": "Newfoundland and Labrador",
      "abbreviation": "NL"
    },
    {
      "name": "Northwest Territories",
      "abbreviation": "NT"
    },
    {
      "name": "Nova Scotia",
      "abbreviation": "NS"
    },
    {
      "name": "Nunavut",
      "abbreviation": "NU"
    },
    {
      "name": "Ontario",
      "abbreviation": "ON"
    },
    {
      "name": "Prince Edward Island",
      "abbreviation": "PE"
    },
    {
      "name": "Quebec",
      "abbreviation": "QC"
    },
    {
      "name": "Saskatchewan",
      "abbreviation": "SK"
    },
    {
      "name": "Yukon Territory",
      "abbreviation": "YT"
    }
  ]

  private _languages = [
    "American Sign Language (ASL)",
    "Communication Realtime Translation (CART)",
    "Afghani-Dari",
    "Albanian",
    "Amharic",
    "Arabic",
    "Azerbaijani",
    "Azerbaijan-Turkish",
    "Bengali",
    "Bosnian",
    "Bulgarian",
    "Burmese",
    "Cambodian (Khmer)",
    "Cantonese",
    "Cebuano",
    "Chiu Chow (Swatow)",
    "Croatian",
    "Czech",
    "Dari",
    "Dinka",
    "Dutch",
    "Farsi",
    "Farsi-Persian",
    "Fiji-Hindi",
    "Filipino",
    "French",
    "Fukien",
    "Fuqing",
    "Fuzhou",
    "German",
    "Greek",
    "Gujarati",
    "Hakha Chin",
    "Hakka",
    "Hebrew",
    "Hindi",
    "Hungarian",
    "Igbo",
    "Ilocano",
    "Indonesian",
    "Italian",
    "Japanese",
    "Karen",
    "Kinyarwanda",
    "Kirundi",
    "Korean",
    "Kurdish",
    "Kurdish (Kurmanji)",
    "Kurdish (Sorani)",
    "Laotian",
    "Lithuanian",
    "Malay",
    "Malayalam",
    "Mandarin",
    "Mongolian",
    "Nepali",
    "Oromo",
    "Pashto",
    "Polish",
    "Portuguese",
    "Punjabi",
    "Romanian",
    "Russian",
    "Serbian",
    "Shanghainese",
    "Sinhalese",
    "Slovak",
    "Somali",
    "Spanish",
    "Sudanese",
    "Swahili",
    "Tagalog",
    "Tamil",
    "Teochew",
    "Thai",
    "Thai",
    "Tigri(gna) (yna)",
    "Turkish",
    "Ukrainian",
    "Urdu",
    "Vietnamese",
    "Xinhui"
  ];

  constructor(
    protected utilsService: UtilsService,
    protected appConfigService: AppConfigService
  ) { }

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

  public get provinces() {
    return this._provinces;
  }

  public get courtLocations(): Config<string>[] {
    return [...this.configuration.courtLocations].sort(this.sortConfigByName());
  }

  public get policeLocations(): Config<string>[] {
    return [...this.configuration.policeLocations].sort(
      this.sortConfigByName()
    );
  }

  public get languages() {
    return this._languages;
  }

  public get countries(): Config<string>[] {
    return [...this.configuration.countries].sort(this.sortConfigByName());
  }

  public get statuses(): Config<number>[] {
    return [...this.configuration.statuses].sort(this.sortConfigByName());
  }

  public get statutes(): Config<number>[] {
    return [...this.configuration.statutes].sort(this.sortConfigByName());
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
