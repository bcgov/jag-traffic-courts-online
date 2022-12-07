import { Injectable } from '@angular/core';
import { Config, Configuration, ProvinceConfig } from '@config/config.model';
import { SortWeight, UtilsService } from '@core/services/utils.service';
import { AppConfigService } from 'app/services/app-config.service';
import { BehaviorSubject, Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ConfigService {
  private configuration: Configuration;

  private disputeSubmitted: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private disputeValidationError: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private ticketError: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private disputeCreateError: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private languageError: BehaviorSubject<string> = new BehaviorSubject<string>('');

  private _provinces: ProvinceConfig[] = [
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

  private _countries: Config<string>[] = [
    { code: "AF", name: "Afghanistan" },
    { code: "AX", name: "Åland Islands" },
    { code: "AL", name: "Albania" },
    { code: "DZ", name: "Algeria" },
    { code: "AS", name: "American Samoa" },
    { code: "AD", name: "Andorra" },
    { code: "AO", name: "Angola" },
    { code: "AI", name: "Anguilla" },
    { code: "AQ", name: "Antarctica" },
    { code: "AG", name: "Antigua and Barbuda" },
    { code: "AR", name: "Argentina" },
    { code: "AM", name: "Armenia" },
    { code: "AW", name: "Aruba" },
    { code: "AU", name: "Australia" },
    { code: "AT", name: "Austria" },
    { code: "AZ", name: "Azerbaijan" },
    { code: "BS", name: "Bahamas" },
    { code: "BH", name: "Bahrain" },
    { code: "BD", name: "Bangladesh" },
    { code: "BB", name: "Barbados" },
    { code: "BY", name: "Belarus" },
    { code: "BE", name: "Belgium" },
    { code: "BZ", name: "Belize" },
    { code: "BJ", name: "Benin" },
    { code: "BM", name: "Bermuda" },
    { code: "BT", name: "Bhutan" },
    { code: "BO", name: "Bolivia" },
    { code: "BA", name: "Bosnia and Herzegovina" },
    { code: "BW", name: "Botswana" },
    { code: "BV", name: "Bouvet Island" },
    { code: "BR", name: "Brazil" },
    { code: "IO", name: "British Indian Ocean Territory" },
    { code: "BN", name: "Brunei Darussalam" },
    { code: "BG", name: "Bulgaria" },
    { code: "BF", name: "Burkina Faso" },
    { code: "BI", name: "Burundi" },
    { code: "KH", name: "Cambodia" },
    { code: "CM", name: "Cameroon" },
    { code: "CA", name: "Canada" },
    { code: "CV", name: "Cape Verde" },
    { code: "KY", name: "Cayman Islands" },
    { code: "CF", name: "Central African Republic" },
    { code: "TD", name: "Chad" },
    { code: "CL", name: "Chile" },
    { code: "CN", name: "China" },
    { code: "CX", name: "Christmas Island" },
    { code: "CC", name: "Cocos (Keeling) Islands" },
    { code: "CO", name: "Colombia" },
    { code: "KM", name: "Comoros" },
    { code: "CG", name: "Congo" },
    { code: "CD", name: "Congo, The Democratic Republic of The" },
    { code: "CK", name: "Cook Islands" },
    { code: "CR", name: "Costa Rica" },
    { code: "CI", name: "Cote D'ivoire" },
    { code: "HR", name: "Croatia" },
    { code: "CU", name: "Cuba" },
    { code: "CY", name: "Cyprus" },
    { code: "CZ", name: "Czechia" },
    { code: "DK", name: "Denmark" },
    { code: "DJ", name: "Djibouti" },
    { code: "DM", name: "Dominica" },
    { code: "DO", name: "Dominican Republic" },
    { code: "EC", name: "Ecuador" },
    { code: "EG", name: "Egypt" },
    { code: "SV", name: "El Salvador" },
    { code: "GQ", name: "Equatorial Guinea" },
    { code: "ER", name: "Eritrea" },
    { code: "EE", name: "Estonia" },
    { code: "ET", name: "Ethiopia" },
    { code: "FK", name: "Falkland Islands (Malvinas)" },
    { code: "FO", name: "Faroe Islands" },
    { code: "FJ", name: "Fiji" },
    { code: "FI", name: "Finland" },
    { code: "FR", name: "France" },
    { code: "GF", name: "French Guiana" },
    { code: "PF", name: "French Polynesia" },
    { code: "TF", name: "French Southern Territories" },
    { code: "GA", name: "Gabon" },
    { code: "GM", name: "Gambia" },
    { code: "GE", name: "Georgia" },
    { code: "DE", name: "Germany" },
    { code: "GH", name: "Ghana" },
    { code: "GI", name: "Gibraltar" },
    { code: "GR", name: "Greece" },
    { code: "GL", name: "Greenland" },
    { code: "GD", name: "Grenada" },
    { code: "GP", name: "Guadeloupe" },
    { code: "GU", name: "Guam" },
    { code: "GT", name: "Guatemala" },
    { code: "GG", name: "Guernsey" },
    { code: "GN", name: "Guinea" },
    { code: "GW", name: "Guinea-bissau" },
    { code: "GY", name: "Guyana" },
    { code: "HT", name: "Haiti" },
    { code: "HM", name: "Heard Island and Mcdonald Islands" },
    { code: "VA", name: "Holy See (Vatican City State)" },
    { code: "HN", name: "Honduras" },
    { code: "HK", name: "Hong Kong" },
    { code: "HU", name: "Hungary" },
    { code: "IS", name: "Iceland" },
    { code: "IN", name: "India" },
    { code: "ID", name: "Indonesia" },
    { code: "IR", name: "Iran, Islamic Republic of" },
    { code: "IQ", name: "Iraq" },
    { code: "IE", name: "Ireland" },
    { code: "IM", name: "Isle of Man" },
    { code: "IL", name: "Israel" },
    { code: "IT", name: "Italy" },
    { code: "JM", name: "Jamaica" },
    { code: "JP", name: "Japan" },
    { code: "JE", name: "Jersey" },
    { code: "JO", name: "Jordan" },
    { code: "KZ", name: "Kazakhstan" },
    { code: "KE", name: "Kenya" },
    { code: "KI", name: "Kiribati" },
    { code: "KP", name: "Korea, Democratic People's Republic of" },
    { code: "KR", name: "Korea, Republic of" },
    { code: "KW", name: "Kuwait" },
    { code: "KG", name: "Kyrgyzstan" },
    { code: "LA", name: "Lao People's Democratic Republic" },
    { code: "LV", name: "Latvia" },
    { code: "LB", name: "Lebanon" },
    { code: "LS", name: "Lesotho" },
    { code: "LR", name: "Liberia" },
    { code: "LY", name: "Libyan Arab Jamahiriya" },
    { code: "LI", name: "Liechtenstein" },
    { code: "LT", name: "Lithuania" },
    { code: "LU", name: "Luxembourg" },
    { code: "MO", name: "Macao" },
    { code: "MK", name: "Macedonia, The Former Yugoslav Republic of" },
    { code: "MG", name: "Madagascar" },
    { code: "MW", name: "Malawi" },
    { code: "MY", name: "Malaysia" },
    { code: "MV", name: "Maldives" },
    { code: "ML", name: "Mali" },
    { code: "MT", name: "Malta" },
    { code: "MH", name: "Marshall Islands" },
    { code: "MQ", name: "Martinique" },
    { code: "MR", name: "Mauritania" },
    { code: "MU", name: "Mauritius" },
    { code: "YT", name: "Mayotte" },
    { code: "MX", name: "Mexico" },
    { code: "FM", name: "Micronesia, Federated States of" },
    { code: "MD", name: "Moldova, Republic of" },
    { code: "MC", name: "Monaco" },
    { code: "MN", name: "Mongolia" },
    { code: "ME", name: "Montenegro" },
    { code: "MS", name: "Montserrat" },
    { code: "MA", name: "Morocco" },
    { code: "MZ", name: "Mozambique" },
    { code: "MM", name: "Myanmar" },
    { code: "NA", name: "Namibia" },
    { code: "NR", name: "Nauru" },
    { code: "NP", name: "Nepal" },
    { code: "NL", name: "Netherlands" },
    { code: "AN", name: "Netherlands Antilles" },
    { code: "NC", name: "New Caledonia" },
    { code: "NZ", name: "New Zealand" },
    { code: "NI", name: "Nicaragua" },
    { code: "NE", name: "Niger" },
    { code: "NG", name: "Nigeria" },
    { code: "NU", name: "Niue" },
    { code: "NF", name: "Norfolk Island" },
    { code: "MP", name: "Northern Mariana Islands" },
    { code: "NO", name: "Norway" },
    { code: "OM", name: "Oman" },
    { code: "PK", name: "Pakistan" },
    { code: "PW", name: "Palau" },
    { code: "PS", name: "Palestinian Territory, Occupied" },
    { code: "PA", name: "Panama" },
    { code: "PG", name: "Papua New Guinea" },
    { code: "PY", name: "Paraguay" },
    { code: "PE", name: "Peru" },
    { code: "PH", name: "Philippines" },
    { code: "PN", name: "Pitcairn" },
    { code: "PL", name: "Poland" },
    { code: "PT", name: "Portugal" },
    { code: "PR", name: "Puerto Rico" },
    { code: "A", name: "Qatar" },
    { code: "RE", name: "Reunion" },
    { code: "RO", name: "Romania" },
    { code: "RU", name: "Russian Federation" },
    { code: "RW", name: "Rwanda" },
    { code: "SH", name: "Saint Helena" },
    { code: "KN", name: "Saint Kitts and Nevis" },
    { code: "LC", name: "Saint Lucia" },
    { code: "PM", name: "Saint Pierre and Miquelon" },
    { code: "VC", name: "Saint Vincent and The Grenadines" },
    { code: "WS", name: "Samoa" },
    { code: "SM", name: "San Marino" },
    { code: "ST", name: "Sao Tome and Principe" },
    { code: "SA", name: "Saudi Arabia" },
    { code: "SN", name: "Senegal" },
    { code: "RS", name: "Serbia" },
    { code: "SC", name: "Seychelles" },
    { code: "SL", name: "Sierra Leone" },
    { code: "SG", name: "Singapore" },
    { code: "SK", name: "Slovakia" },
    { code: "SI", name: "Slovenia" },
    { code: "SB", name: "Solomon Islands" },
    { code: "SO", name: "Somalia" },
    { code: "ZA", name: "South Africa" },
    { code: "GS", name: "South Georgia and The South Sandwich Islands" },
    { code: "ES", name: "Spain" },
    { code: "LK", name: "Sri Lanka" },
    { code: "SD", name: "Sudan" },
    { code: "SR", name: "Suriname" },
    { code: "SJ", name: "Svalbard and Jan Mayen" },
    { code: "SZ", name: "Swaziland" },
    { code: "SE", name: "Sweden" },
    { code: "CH", name: "Switzerland" },
    { code: "SY", name: "Syrian Arab Republic" },
    { code: "TW", name: "Taiwan, Province of China" },
    { code: "TJ", name: "Tajikistan" },
    { code: "TZ", name: "Tanzania, United Republic of" },
    { code: "TH", name: "Thailand" },
    { code: "TL", name: "Timor-leste" },
    { code: "TG", name: "Togo" },
    { code: "TK", name: "Tokelau" },
    { code: "TO", name: "Tonga" },
    { code: "TT", name: "Trinidad and Tobago" },
    { code: "TN", name: "Tunisia" },
    { code: "TR", name: "Turkey" },
    { code: "TM", name: "Turkmenistan" },
    { code: "TC", name: "Turks and Caicos Islands" },
    { code: "TV", name: "Tuvalu" },
    { code: "UG", name: "Uganda" },
    { code: "UA", name: "Ukraine" },
    { code: "AE", name: "United Arab Emirates" },
    { code: "GB", name: "United Kingdom" },
    { code: "US", name: "United States" },
    { code: "UM", name: "United States Minor Outlying Islands" },
    { code: "UY", name: "Uruguay" },
    { code: "UZ", name: "Uzbekistan" },
    { code: "VU", name: "Vanuatu" },
    { code: "VE", name: "Venezuela" },
    { code: "VN", name: "Viet Nam" },
    { code: "VG", name: "Virgin Islands, British" },
    { code: "VI", name: "Virgin Islands, U.S." },
    { code: "WF", name: "Wallis and Futuna" },
    { code: "EH", name: "Western Sahara" },
    { code: "YE", name: "Yemen" },
    { code: "ZM", name: "Zambia" },
    { code: "ZW", name: "Zimbabwe" }
  ];

  constructor(
    private utilsService: UtilsService,
    private appConfigService: AppConfigService
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

  public get language_error$(): BehaviorSubject<string> {
    return this.languageError;
  }
  public get language_error(): string {
    return this.languageError.value;
  }

  public get provinces(): ProvinceConfig[] {
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

  public get countries(): Config<string>[] {
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
