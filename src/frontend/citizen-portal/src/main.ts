import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AuthConfig } from 'app/auth/models/auth-config.model';
import { AppConfig } from 'app/services/app-config.service';
import { forkJoin } from 'rxjs';

import { AppModule } from './app/app.module';

forkJoin([
  fetch('/assets/app.config.json').then((response) => response.json()),
  fetch('/assets/auth.config.json').then((response) => response.json()),
  fetch('/assets/oidc.config.json').then((response) => response.json()),
]).subscribe(([appConfig, authConfig, authWellKnownDocument]) => {
  console.log('Is production?', appConfig.production);
  if (appConfig.production) {
    enableProdMode();
  }

  authConfig.redirectUrl = window.location.origin;
  authConfig.postLogoutRedirectUri = window.location.origin;

  // Note: Authority must NOT have trailing slash
  var authority = authConfig.authority;
  authConfig.authority = authority.endsWith("/") ? authority.substring(0, authority.length - 1) : authority;

  platformBrowserDynamic([
    { provide: AppConfig, useValue: appConfig },
    { provide: AuthConfig, useValue: { config: authConfig, authWellKnownDocument } },
  ])
    .bootstrapModule(AppModule)
    .catch((err) => console.error(err));
})
