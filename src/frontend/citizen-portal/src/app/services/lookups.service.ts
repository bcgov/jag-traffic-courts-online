import { ConfigService } from '@config/config.service';
import { LoggerService } from '@core/services/logger.service';
import { ToastService } from '@core/services/toast.service';
import { LookupService, Language } from 'app/api';
import { Observable, BehaviorSubject } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { Injectable } from '@angular/core';

export interface ILookupsService {
  languages$: Observable<Language[]>;
  languages: Language[];
  getLanguages(): Observable<Language[]>;
  getLanguageDescription(code: string): string;
}

@Injectable({
  providedIn: 'root',
})
export class LookupsService implements ILookupsService {
  private _languages: BehaviorSubject<Language[]> = new BehaviorSubject<Language[]>(null);

  constructor(
    private toastService: ToastService,
    private logger: LoggerService,
    private configService: ConfigService,
    private lookupService: LookupService
  ) {
    this.getLanguages().subscribe((response: Language[]) => {
      this._languages.next(response);
    });
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

  public get languages$(): Observable<Language[]> {
    return this._languages.asObservable();
  }

  public get languages(): Language[] {
    return this._languages.value;
  }

  public getLanguageDescription(code: string): string {
    let found = this.languages.filter(x => x.code.trim().toLowerCase() === code.trim().toLowerCase()).shift();
    if (found) return found.description;
    else return code;
  }
}
