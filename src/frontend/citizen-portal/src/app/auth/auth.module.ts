import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { EffectsModule } from '@ngrx/effects';
import { Store, StoreModule } from '@ngrx/store';
import { AuthModule as OidcModule, OidcSecurityService, StsConfigLoader } from 'angular-auth-oidc-client';
import { AuthConfig, AuthConfigLoader, AuthConfigService, AuthServiceInit } from './services/auth-config.service';
import { AuthHttpInterceptor } from './interceptors/auth-http.interceptor';
import { AuthStore } from './store';

@NgModule({
  imports: [
    OidcModule.forRoot({
      loader: {
        provide: StsConfigLoader,
        useFactory: AuthConfigLoader,
        deps: [AuthConfigService],
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
export class AuthModule {
  constructor(
    private authConfigService: AuthConfigService,
    private authConfig: AuthConfig
  ) {
    // Get from main.ts, no need to fetch again
    this.authConfigService.setAuthConfig(this.authConfig);
  }
}