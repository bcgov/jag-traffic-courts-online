import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { EffectsModule } from '@ngrx/effects';
import { Store, StoreModule } from '@ngrx/store';
import { AuthModule as OidcModule, OidcSecurityService, StsConfigLoader, StsConfigStaticLoader } from 'angular-auth-oidc-client';
import { AuthHttpInterceptor } from './interceptors/auth-http.interceptor';
import { AuthStore } from './store';
import { AuthConfig } from './models/auth-config.model';

export function AuthServiceInit(oidcSecurityService: OidcSecurityService, store: Store) {
  return () => {
    return oidcSecurityService.checkAuth().subscribe(({ isAuthenticated, accessToken }) => {
      store.dispatch(AuthStore.Actions.Authorized({ payload: { isAuthenticated, accessToken } }));
      return;
    })
  };
}

export const AuthConfigLoader = (authConfig: AuthConfig) => {
  return new StsConfigStaticLoader(authConfig.config);
};

@NgModule({
  imports: [
    OidcModule.forRoot({
      loader: {
        provide: StsConfigLoader,
        useFactory: AuthConfigLoader,
        deps: [AuthConfig],
      },
    }),
    StoreModule.forFeature(AuthStore.StoreName, AuthStore.Reducer),
    EffectsModule.forFeature([AuthStore.Effects])
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: AuthServiceInit,
      deps: [OidcSecurityService, Store],
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthHttpInterceptor,
      multi: true
    },
  ],
  exports: [OidcModule],
})
export class AuthModule { }