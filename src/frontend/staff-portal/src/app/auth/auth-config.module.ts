/* eslint-disable arrow-body-style */
import { HttpClient } from '@angular/common/http';
import { NgModule, isDevMode } from '@angular/core';
import { AuthModule, LogLevel, StsConfigHttpLoader, StsConfigLoader } from 'angular-auth-oidc-client';
import { map } from 'rxjs/operators';
import { environment } from 'environments/environment';

export const httpLoaderFactory = (httpClient: HttpClient) => {
  const config$ = httpClient.get<any>(`assets/config/oidc.json`).pipe(
    map((customConfig: any) => {
      return {
        triggerAuthorizationResultEvent: true,
        postLoginRoute: window.location.origin + customConfig.postLoginRoute,
        logLevel: environment.production ? LogLevel.None : LogLevel.Debug,
        eagerLoadAuthWellKnownEndpoints: false,
        authority: customConfig.authority,
        redirectUrl: window.location.origin,
        postLogoutRedirectUri: window.location.origin,
        clientId: customConfig.clientId,
        scope: customConfig.scope,
        responseType: customConfig.responseType,
        silentRenew: true,
        // silentRenewUrl: `${window.location.origin}/silent-renew.html`, //option 1 - with CORS issue
        useRefreshToken: true,
        renewTimeBeforeTokenExpiresInSeconds: 60,
        autoUserInfo: true,
        unauthorizedRoute: '/unauthorized',
        forbiddenRoute: '/unauthorized', // used for wrong roles
        ignoreNonceAfterRefresh: true,
        secureRoutes: ['/api'],
        customParamsAuthRequest: {
          prompt: customConfig.prompt,
          kc_idp_hint: customConfig.kc_idp_hint,
        },
        historyCleanupOff: true,
      };
    })
  );

  return new StsConfigHttpLoader(config$);
};
@NgModule({
  imports: [
    AuthModule.forRoot({
      loader: {
        provide: StsConfigLoader,
        useFactory: httpLoaderFactory,
        deps: [HttpClient],
      },
    }),
  ],
  exports: [AuthModule],
})
export class AuthConfigModule {}
