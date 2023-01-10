import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { OidcSecurityService, OpenIdConfiguration, StsConfigHttpLoader } from 'angular-auth-oidc-client';
import { AuthStore } from '../store';
import { Store } from '@ngrx/store';

export class AuthConfig {
  config: OpenIdConfiguration;
  authWellKnownDocument: any;
}

export function AuthServiceInit(oidcSecurityService: OidcSecurityService, store: Store) {
  return () => {
    return oidcSecurityService.checkAuth().subscribe(({ isAuthenticated, accessToken }) => {
      store.dispatch(AuthStore.Actions.Authorized({ payload: { isAuthenticated, accessToken } }));
      return;
    })
  };
}

export const AuthConfigLoader = (authConfigService: AuthConfigService) => {
  return new StsConfigHttpLoader(authConfigService.config$);
};

@Injectable({
  providedIn: 'root',
})
export class AuthConfigService {
  private _authConfig: BehaviorSubject<AuthConfig> = new BehaviorSubject<AuthConfig>(null);

  setAuthConfig(authConfig: AuthConfig): void {
    authConfig.config.redirectUrl = window.location.origin;
    authConfig.config.postLogoutRedirectUri = window.location.origin;
    this._authConfig.next(authConfig);
  }

  get config(): OpenIdConfiguration {
    return this._authConfig.value?.config;
  }

  get config$(): Observable<OpenIdConfiguration> {
    return this._authConfig.pipe(map(i => i?.config));
  }

  get authWellKnownDocument(): string {
    return this._authConfig.value?.authWellKnownDocument;
  }
}
