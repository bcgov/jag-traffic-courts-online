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
      code: 'AB',
      countryCode: 'CA',
      name: 'Alberta',
    },
    {
      code: 'AK',
      countryCode: 'US',
      name: 'Alaska',
    },
    {
      code: 'AL',
      countryCode: 'US',
      name: 'Alabama',
    },
    {
      code: 'AR',
      countryCode: 'US',
      name: 'Arkansas',
    },
    {
      code: 'AS',
      countryCode: 'US',
      name: 'American Samoa',
    },
    {
      code: 'AZ',
      countryCode: 'US',
      name: 'Arizona',
    },
    {
      code: 'BC',
      countryCode: 'CA',
      name: 'British Columbia',
    },
    {
      code: 'CA',
      countryCode: 'US',
      name: 'California',
    },
    {
      code: 'CO',
      countryCode: 'US',
      name: 'Colorado',
    },
    {
      code: 'CT',
      countryCode: 'US',
      name: 'Connecticut',
    },
    {
      code: 'DC',
      countryCode: 'US',
      name: 'District of Columbia',
    },
    {
      code: 'DE',
      countryCode: 'US',
      name: 'Delaware',
    },
    {
      code: 'FL',
      countryCode: 'US',
      name: 'Florida',
    },
    {
      code: 'GA',
      countryCode: 'US',
      name: 'Georgia',
    },
    {
      code: 'GU',
      countryCode: 'US',
      name: 'Guam',
    },
    {
      code: 'HI',
      countryCode: 'US',
      name: 'Hawaii',
    },
    {
      code: 'IA',
      countryCode: 'US',
      name: 'Iowa',
    },
    {
      code: 'ID',
      countryCode: 'US',
      name: 'Idaho',
    },
    {
      code: 'IL',
      countryCode: 'US',
      name: 'Illinois',
    },
    {
      code: 'IN',
      countryCode: 'US',
      name: 'Indiana',
    },
    {
      code: 'KS',
      countryCode: 'US',
      name: 'Kansas',
    },
    {
      code: 'KY',
      countryCode: 'US',
      name: 'Kentucky',
    },
    {
      code: 'LA',
      countryCode: 'US',
      name: 'Louisiana',
    },
    {
      code: 'MA',
      countryCode: 'US',
      name: 'Massachusetts',
    },
    {
      code: 'MB',
      countryCode: 'CA',
      name: 'Manitoba',
    },
    {
      code: 'MD',
      countryCode: 'US',
      name: 'Maryland',
    },
    {
      code: 'ME',
      countryCode: 'US',
      name: 'Maine',
    },
    {
      code: 'MI',
      countryCode: 'US',
      name: 'Michigan',
    },
    {
      code: 'MN',
      countryCode: 'US',
      name: 'Minnesota',
    },
    {
      code: 'MO',
      countryCode: 'US',
      name: 'Missouri',
    },
    {
      code: 'MP',
      countryCode: 'US',
      name: 'Northern Mariana Islands',
    },
    {
      code: 'MS',
      countryCode: 'US',
      name: 'Mississippi',
    },
    {
      code: 'MT',
      countryCode: 'US',
      name: 'Montana',
    },
    {
      code: 'NB',
      countryCode: 'CA',
      name: 'New Brunswick',
    },
    {
      code: 'NC',
      countryCode: 'US',
      name: 'North Carolina',
    },
    {
      code: 'ND',
      countryCode: 'US',
      name: 'North Dakota',
    },
    {
      code: 'NE',
      countryCode: 'US',
      name: 'Nebraska',
    },
    {
      code: 'NH',
      countryCode: 'US',
      name: 'New Hampshire',
    },
    {
      code: 'NJ',
      countryCode: 'US',
      name: 'New Jersey',
    },
    {
      code: 'NL',
      countryCode: 'CA',
      name: 'Newfoundland and Labrador',
    },
    {
      code: 'NM',
      countryCode: 'US',
      name: 'New Mexico',
    },
    {
      code: 'NS',
      countryCode: 'CA',
      name: 'Nova Scotia',
    },
    {
      code: 'NT',
      countryCode: 'CA',
      name: 'Northwest Territories',
    },
    {
      code: 'NU',
      countryCode: 'CA',
      name: 'Nunavut',
    },
    {
      code: 'NV',
      countryCode: 'US',
      name: 'Nevada',
    },
    {
      code: 'NY',
      countryCode: 'US',
      name: 'New York',
    },
    {
      code: 'OH',
      countryCode: 'US',
      name: 'Ohio',
    },
    {
      code: 'OK',
      countryCode: 'US',
      name: 'Oklahoma',
    },
    {
      code: 'ON',
      countryCode: 'CA',
      name: 'Ontario',
    },
    {
      code: 'OR',
      countryCode: 'US',
      name: 'Oregon',
    },
    {
      code: 'PA',
      countryCode: 'US',
      name: 'Pennsylvania',
    },
    {
      code: 'PE',
      countryCode: 'CA',
      name: 'Prince Edward Island',
    },
    {
      code: 'PR',
      countryCode: 'US',
      name: 'Puerto Rico',
    },
    {
      code: 'QC',
      countryCode: 'CA',
      name: 'Quebec',
    },
    {
      code: 'RI',
      countryCode: 'US',
      name: 'Rhode Island',
    },
    {
      code: 'SC',
      countryCode: 'US',
      name: 'South Carolina',
    },
    {
      code: 'SD',
      countryCode: 'US',
      name: 'South Dakota',
    },
    {
      code: 'SK',
      countryCode: 'CA',
      name: 'Saskatchewan',
    },
    {
      code: 'TN',
      countryCode: 'US',
      name: 'Tennessee',
    },
    {
      code: 'TX',
      countryCode: 'US',
      name: 'Texas',
    },
    {
      code: 'UM',
      countryCode: 'US',
      name: 'United States Minor Outlying Islands',
    },
    {
      code: 'UT',
      countryCode: 'US',
      name: 'Utah',
    },
    {
      code: 'VA',
      countryCode: 'US',
      name: 'Virginia',
    },
    {
      code: 'VI',
      countryCode: 'US',
      name: 'Virgin Islands, U.S.',
    },
    {
      code: 'VT',
      countryCode: 'US',
      name: 'Vermont',
    },
    {
      code: 'WA',
      countryCode: 'US',
      name: 'Washington',
    },
    {
      code: 'WI',
      countryCode: 'US',
      name: 'Wisconsin',
    },
    {
      code: 'WV',
      countryCode: 'US',
      name: 'West Virginia',
    },
    {
      code: 'WY',
      countryCode: 'US',
      name: 'Wyoming',
    },
    {
      code: 'YT',
      countryCode: 'CA',
      name: 'Yukon',
    },
  ];

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

  private _countries = [
    "Afghanistan",
    "Albania",
    "Algeria",
    "Andorra",
    "Angola",
    "Antigua & Deps",
    "Argentina",
    "Armenia",
    "Australia",
    "Austria",
    "Azerbaijan",
    "Bahamas",
    "Bahrain",
    "Bangladesh",
    "Barbados",
    "Belarus",
    "Belgium",
    "Belize",
    "Benin",
    "Bhutan",
    "Bolivia",
    "Bosnia Herzegovina",
    "Botswana",
    "Brazil",
    "Brunei",
    "Bulgaria",
    "Burkina",
    "Burundi",
    "Cambodia",
    "Cameroon",
    "Canada",
    "Cape Verde",
    "Central African Rep",
    "Chad",
    "Chile",
    "China",
    "Colombia",
    "Comoros",
    "Congo",
    "Congo {Democratic Rep}",
    "Costa Rica",
    "Croatia",
    "Cuba",
    "Cyprus",
    "Czech Republic",
    "Denmark",
    "Djibouti",
    "Dominica",
    "Dominican Republic",
    "East Timor",
    "Ecuador",
    "Egypt",
    "El Salvador",
    "Equatorial Guinea",
    "Eritrea",
    "Estonia",
    "Ethiopia",
    "Fiji",
    "Finland",
    "France",
    "Gabon",
    "Gambia",
    "Georgia",
    "Germany",
    "Ghana",
    "Greece",
    "Grenada",
    "Guatemala",
    "Guinea",
    "Guinea-Bissau",
    "Guyana",
    "Haiti",
    "Honduras",
    "Hungary",
    "Iceland",
    "India",
    "Indonesia",
    "Iran",
    "Iraq",
    "Ireland {Republic}",
    "Israel",
    "Italy",
    "Ivory Coast",
    "Jamaica",
    "Japan",
    "Jordan",
    "Kazakhstan",
    "Kenya",
    "Kiribati",
    "Korea North",
    "Korea South",
    "Kosovo",
    "Kuwait",
    "Kyrgyzstan",
    "Laos",
    "Latvia",
    "Lebanon",
    "Lesotho",
    "Liberia",
    "Libya",
    "Liechtenstein",
    "Lithuania",
    "Luxembourg",
    "Macedonia",
    "Madagascar",
    "Malawi",
    "Malaysia",
    "Maldives",
    "Mali",
    "Malta",
    "Marshall Islands",
    "Mauritania",
    "Mauritius",
    "Mexico",
    "Micronesia",
    "Moldova",
    "Monaco",
    "Mongolia",
    "Montenegro",
    "Morocco",
    "Mozambique",
    "Myanmar, {Burma}",
    "Namibia",
    "Nauru",
    "Nepal",
    "Netherlands",
    "New Zealand",
    "Nicaragua",
    "Niger",
    "Nigeria",
    "Norway",
    "Oman",
    "Pakistan",
    "Palau",
    "Panama",
    "Papua New Guinea",
    "Paraguay",
    "Peru",
    "Philippines",
    "Poland",
    "Portugal",
    "Qatar",
    "Romania",
    "Russian Federation",
    "Rwanda",
    "St Kitts & Nevis",
    "St Lucia",
    "Saint Vincent & the Grenadines",
    "Samoa",
    "San Marno",
    "Sao Tome & Principe",
    "Saudi Arabia",
    "Senegal",
    "Serbia",
    "Seychelles",
    "Sierra Leone",
    "Singapore",
    "Slovakia",
    "Slovenia",
    "Solomon Islands",
    "Somalia",
    "South Africa",
    "South Sudan",
    "Spain",
    "Sri Lanka",
    "Sudan",
    "Suriname",
    "Swaziland",
    "Sweden",
    "Switzerland",
    "Syria",
    "Taiwan",
    "Tajikistan",
    "Tanzania",
    "Thailand",
    "Togo",
    "Tonga",
    "Trinidad & Tobago",
    "Tunisia",
    "Turkey",
    "Turkmenistan",
    "Tuvalu",
    "Uganda",
    "Ukraine",
    "United Arab Emirates",
    "United Kingdom",
    "United States",
    "Uruguay",
    "Uzbekistan",
    "Vanuatu",
    "Vatican City",
    "Venezuela",
    "Vietnam",
    "Yemen",
    "Zambia",
    "Zimbabwe"
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

  public get countries() {
    return this._countries;
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
