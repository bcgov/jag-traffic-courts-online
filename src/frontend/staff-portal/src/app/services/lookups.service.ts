import { ConfigService } from '@config/config.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { Statute as StatuteBase, LookupService, Language, Agency } from 'app/api';
import { Observable, BehaviorSubject } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';

export interface ILookupsService {
  statutes$: BehaviorSubject<Statute[]>;
  statutes: Statute[];
  courthouseAgencies$: BehaviorSubject<Agency[]>;
  courthouseAgencies: Agency[];
  languages$: BehaviorSubject<Language[]>;
  languages: Language[];
  getStatutes(): Observable<Statute[]>;
  getLanguages(): Observable<Language[]>;
  getCourthouseAgencies(): Observable<Agency[]>;
  getLanguageDescription(code: string): string;
}

@Injectable({
  providedIn: 'root',
})
export class LookupsService implements ILookupsService {
  private _statutes: BehaviorSubject<Statute[]>;
  private _languages: BehaviorSubject<Language[]>;
  private _courthouseAgencies: BehaviorSubject<Agency[]>;

  constructor(
    private toastService: ToastService,
    private logger: LoggerService,
    private configService: ConfigService,
    private lookupService: LookupService
  ) {
    this._statutes = new BehaviorSubject<Statute[]>(null);
    this._languages = new BehaviorSubject<Language[]>(null);
    this._courthouseAgencies = new BehaviorSubject<Agency[]>(null);
    this.getStatutes();
    this.getLanguages();
    this.getCourthouseAgencies();
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
          response ? response : null
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

  public get statutes$(): BehaviorSubject<Statute[]> {
    return this._statutes;
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
          response ? response : null
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

  public get languages$(): BehaviorSubject<Language[]> {
    return this._languages;
  }

  public get languages(): Language[] {
    return this._languages.value;
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
          response ? response : null
        ),
        map((agencies: Agency[]) => {
          agencies.sort((a, b) => { if (a.name < b.name) return -1; })
          return agencies.filter(a => a.typeCode=== "CTH");
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
    return [
      {
        id: "19227.0734",
        name: "100 Mile House Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "9393.0001",
        name: "Abbotsford Law Courts",
        typeCode: "CTH", __team: "C"
    },
    {
        id: "18817.0045",
        name: "Adjudicator Listing",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "105.0001",
        name: "Anahim Lake Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "22.0001",
        name: "Ashcroft Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "106.0001",
        name: "Atlin Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "19187.0734",
        name: "B.C. Court of Appeal",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10244.0001",
        name: "Bella Bella Law Courts",
        typeCode: "CTH", __team: "C"
    },
    {
        id: "10245.0001",
        name: "Bella Coola Law Courts",
        typeCode: "CTH", __team: "C"
    },
    {
        id: "8823.0001",
        name: "Burnaby Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "107.0001",
        name: "Burns Lake Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "9067.0001",
        name: "Campbell River Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "108.0001",
        name: "Cassiar Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "23.0001",
        name: "Castlegar Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "27.0001",
        name: "Chase Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "109.0001",
        name: "Chetwynd Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "8824.0001",
        name: "Chilliwack Law Courts",
        typeCode: "CTH", __team: "C"
    },
    {
        id: "28.0001",
        name: "Clearwater Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "19625.0734",
        name: "Court of Appeal of BC - Kamloops",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "19626.0734",
        name: "Court of Appeal of BC - Kelowna",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "19622.0734",
        name: "Court of Appeal of BC - Vancouver",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "19627.0734",
        name: "Court of Appeal of BC - Victoria",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "9068.0001",
        name: "Courtenay Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "29.0001",
        name: "Cranbrook Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "77.0001",
        name: "Creston Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10266.0001",
        name: "Daajing Giids Provincial Crt",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "110.0001",
        name: "Dawson Creek Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "111.0001",
        name: "Dease Lake Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "9066.0001",
        name: "Delta Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10231.0001",
        name: "Duncan Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "78.0001",
        name: "Fernie Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "112.0001",
        name: "Fort Nelson Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "113.0001",
        name: "Fort St. James Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "114.0001",
        name: "Fort St. John Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10257.0001",
        name: "Fort Ware Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "115.0001",
        name: "Fraser Lake Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10232.0001",
        name: "Ganges Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10233.0001",
        name: "Gold River Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "79.0001",
        name: "Golden Law Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "80.0001",
        name: "Grand Forks Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10253.0001",
        name: "Hope Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "116.0001",
        name: "Houston Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "117.0001",
        name: "Hudson's Hope Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "81.0001",
        name: "Invermere Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "19247.0734",
        name: "Justice Centre (Judicial)",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "82.0001",
        name: "Kamloops Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "83.0001",
        name: "Kelowna Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "84.0001",
        name: "Kimberley Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "118.0001",
        name: "Kitimat Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "19678.0734",
        name: "Kitsilano Secondary School",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "19307.0734",
        name: "Klemtu Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "19679.0734",
        name: "Kwantlen Polytechnic University",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10254.0001",
        name: "Langley Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10912.0026",
        name: "Leech Town Court House",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10264.0001",
        name: "Leech Town Court Registry",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "85.0001",
        name: "Lillooet Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "119.0001",
        name: "Lower Post Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "16164.0026",
        name: "Lytton Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "120.0001",
        name: "MacKenzie Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "8837.0001",
        name: "Maple Ridge Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "121.0001",
        name: "Masset Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "122.0001",
        name: "McBride Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "87.0001",
        name: "Merritt Law Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10255.0001",
        name: "Mission Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "88.0001",
        name: "Nakusp Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "8805.0001",
        name: "Nanaimo Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "19066.0002",
        name: "Nanaimo Law Courts NA10",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "19057.0002",
        name: "Nanaimo Law Courts NAO1",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "19058.0002",
        name: "Nanaimo Law Courts NAO2",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "19059.0002",
        name: "Nanaimo Law Courts NAO3",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "19060.0002",
        name: "Nanaimo Law Courts NAO4",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "19061.0002",
        name: "Nanaimo Law Courts NAO5",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "19062.0002",
        name: "Nanaimo Law Courts NAO6",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "19063.0002",
        name: "Nanaimo Law Courts NAO7",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "19064.0002",
        name: "Nanaimo Law Courts NAO8",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "19065.0002",
        name: "Nanaimo Law Courts NAO9",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "89.0001",
        name: "Nelson Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "123.0001",
        name: "New Aiyansh Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "124.0001",
        name: "New Hazelton Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "8839.0001",
        name: "New Westminster Law Courts",
        typeCode: "CTH", __team: "C"
    },
    {
        id: "9064.0001",
        name: "North Vancouver Court",
        typeCode: "CTH", __team: "B"
    },
    {
        id: "90.0001",
        name: "Oliver Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10234.0001",
        name: "Parksville Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10246.0001",
        name: "Pemberton Provincial Court",
        typeCode: "CTH", __team: "B"
    },
    {
        id: "91.0001",
        name: "Penticton Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10235.0001",
        name: "Port Alberni Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "8834.0001",
        name: "Port Coquitlam Court",
        typeCode: "CTH", __team: "C"
    },
    {
        id: "10236.0001",
        name: "Port Hardy Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10237.0001",
        name: "Powell River Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "8844.0001",
        name: "Prince George Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10265.0001",
        name: "Prince George Supreme Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "9075.0001",
        name: "Prince Rupert Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "92.0001",
        name: "Princeton Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "9074.0001",
        name: "Quesnel Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "93.0001",
        name: "Revelstoke Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10247.0001",
        name: "Richmond Court Small Claims and Family Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "9062.0001",
        name: "Richmond Provincial Court",
        typeCode: "CTH", __team: "B"
    },
    {
        id: "10250.0001",
        name: "Robson Square Provincial Court",
        typeCode: "CTH", __team: "A"
    },
    {
        id: "94.0001",
        name: "Rossland Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "95.0001",
        name: "Salmon Arm Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10248.0001",
        name: "Sechelt Provincial Court",
        typeCode: "CTH", __team: "B"
    },
    {
        id: "19614.0734",
        name: "Shared Resource",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "18886.0045",
        name: "Sherbrooke Courthouse",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10238.0001",
        name: "Sidney Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "9073.0001",
        name: "Smithers Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10256.0001",
        name: "Sparwood Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10249.0001",
        name: "Squamish Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10267.0001",
        name: "Stewart Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "8842.0001",
        name: "Surrey Family Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "8841.0001",
        name: "Surrey Provincial Court",
        typeCode: "CTH", __team: "B"
    },
    {
        id: "10239.0001",
        name: "Tahsis Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "9072.0001",
        name: "Terrace Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10240.0001",
        name: "Tofino Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10258.0001",
        name: "Tsay Keh Dene Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10268.0001",
        name: "Tumbler Ridge Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10241.0001",
        name: "Ucluelet Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "9144.0001",
        name: "Valemount Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "19228.0734",
        name: "Vancouver Law Courts",
        typeCode: "CTH", __team: "A"
    },
    {
        id: "8813.0001",
        name: "Vancouver Provincial Court",
        typeCode: "CTH", __team: "A"
    },
    {
        id: "10251.0001",
        name: "Vancouver Traffic Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "9071.0001",
        name: "Vanderhoof Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "96.0001",
        name: "Vernon Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10242.0001",
        name: "Victoria Family and Youth Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "8807.0001",
        name: "Victoria Law Courts",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "19635.0734",
        name: "Violation Ticket Centre",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10252.0001",
        name: "West Vancouver Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "10243.0001",
        name: "Western Communities Provincial Court",
        typeCode: "CTH", __team: "D"
    },
    {
        id: "9070.0001",
        name: "Williams Lake Law Courts",
        typeCode: "CTH", __team: "D"
    }
    ]
  }

  public get courthouseAgencies$(): BehaviorSubject<Agency[]> {
    return this._courthouseAgencies;
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
