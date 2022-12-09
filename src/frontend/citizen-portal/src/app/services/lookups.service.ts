import { ConfigService } from '@config/config.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { LookupService, Language} from 'app/api';
import { Observable, BehaviorSubject } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';

export interface ILookupsService {
  languages$: BehaviorSubject<Language[]>;
  languages: Language[];
  getLanguages(): Observable<Language[]>;
  getLanguageDescription(code: string): string;
}

@Injectable({
  providedIn: 'root',
})
export class LookupsService implements ILookupsService {
  private _languages: BehaviorSubject<Language[]>;

  constructor(
    private toastService: ToastService,
    private logger: LoggerService,
    private configService: ConfigService,
    private lookupService: LookupService
  ) {
    this._languages = new BehaviorSubject<Language[]>(null);
    this.getLanguages();
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
    let found = this.languages.filter(x => x.code.trim().toLowerCase() === code.trim().toLowerCase());
    if (found && found.length > 0) return found[0].description;
    else return code;
}
}
