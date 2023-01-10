import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { BehaviorSubject, forkJoin, Observable } from 'rxjs';
import { OidcSecurityService, OpenIdConfiguration, StsConfigHttpLoader } from 'angular-auth-oidc-client';
import { Store } from '@ngrx/store';
import { AuthStore } from '../store';

export function AuthServiceInit(authConfigService: AuthConfigService, oidcSecurityService: OidcSecurityService, store: Store) {
  return () => {
    let observables = [
      authConfigService.loadConfig(),
      authConfigService.loadAuthWellKnownDocument()
    ];

    return forkJoin(observables)
      .pipe(
        map(() => {
          oidcSecurityService.preloadAuthWellKnownDocument().subscribe(res => {
            oidcSecurityService.checkAuth().subscribe(({ isAuthenticated, accessToken }) => {
              store.dispatch(AuthStore.Actions.Authorized({ payload: { isAuthenticated, accessToken } }));
              return;
            })
          })
        })
      );
  };
}

export const AuthConfigLoader = (authConfigService: AuthConfigService) => {
  return new StsConfigHttpLoader(authConfigService.config$);
};

@Injectable({
  providedIn: 'root',
})
export class AuthConfigService {
  private _config: BehaviorSubject<OpenIdConfiguration> = new BehaviorSubject<OpenIdConfiguration>(null);
  private _authWellKnownDocument: BehaviorSubject<string> = new BehaviorSubject<string>(null);

  constructor(
    private http: HttpClient,
  ) {
  }

  loadConfig(): Observable<void> {
    return this.http.get("/assets/auth.config.json").pipe(
      map((config: OpenIdConfiguration) => {
        config.redirectUrl = window.location.origin;
        config.postLogoutRedirectUri = window.location.origin;
        this._config.next(config);
      })
    );
  }

  loadAuthWellKnownDocument(): Observable<void> {
    return this.http.get("/assets/oidc.config.json").pipe(
      map((res: string) => {
        this._authWellKnownDocument.next(res);
      })
    );
  }

  get config(): OpenIdConfiguration {
    return this._config.value;
  }

  get config$(): Observable<OpenIdConfiguration> {
    return this._config.asObservable();
  }

  get authWellKnownDocument(): string {
    return this._authWellKnownDocument.value;
  }
}
