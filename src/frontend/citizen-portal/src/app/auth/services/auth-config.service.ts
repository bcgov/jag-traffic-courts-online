import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { AuthWellKnownEndpoints, OidcSecurityService, OpenIdConfiguration, StsConfigHttpLoader } from 'angular-auth-oidc-client';
import { Store } from '@ngrx/store';
import { AuthStore } from '../store';

export function AuthServiceInit(authConfigService: AuthConfigService, oidcSecurityService: OidcSecurityService, store: Store) {
  return () => {
    return authConfigService.loadAuthWellKnownDocument()
      .pipe(
        map(() => {
          oidcSecurityService.preloadAuthWellKnownDocument().subscribe(res => {
            authConfigService.authWellKnownEndpoints = res;
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
  authWellKnownDocument: any;
  authWellKnownEndpoints: AuthWellKnownEndpoints;

  constructor(
    private http: HttpClient,
    private store: Store
  ) {
  }

  private loadConfig(): Observable<OpenIdConfiguration> {
    return this.http.get("/assets/auth.config.json").pipe(
      map((config: OpenIdConfiguration) => {
        config.redirectUrl = window.location.origin;
        config.postLogoutRedirectUri = window.location.origin;
        this._config.next(config);
        return config;
      })
    );
  }

  loadAuthWellKnownDocument(): Observable<any> {
    return this.http.get("/assets/oidc.config.json").pipe(
      map((res: any) => {
        this.authWellKnownDocument = res;
      })
    );
  }

  get config(): OpenIdConfiguration {
    return this._config.value;
  }

  get config$(): Observable<OpenIdConfiguration> {
    if (this._config.value) {
      return of(this.config);
    } else {
      return this.loadConfig();
    }
  }
}
